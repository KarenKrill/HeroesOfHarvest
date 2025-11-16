#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;

using KarenKrill.DataStorage.Abstractions;
using KarenKrill.UniCore.StateSystem.Abstractions;
using KarenKrill.UniCore.UI.Presenters.Abstractions;

using HeroesOfHarvest.Abstractions;
using HeroesOfHarvest.UI.Views.Abstractions;

namespace HeroesOfHarvest.GameStates
{
    public class InitialState : IStateHandler<GameState>
    {
        public GameState State => GameState.Initial;

        public InitialState(ILogger logger,
            IGameFlow gameFlow,
            IMapObjectRegistry mapObjectRegistry,
            IPresenter<IDiagnosticInfoView> diagnosticInfoPresenter,
            IAudioController audioController,
            IPlayerSession playerSession,
            GameSettings gameSettings,
            IDataStorage dataStorage)
        {
            _logger = logger;
            _gameFlow = gameFlow;
            _mapObjectRegistry = mapObjectRegistry;
            _diagnosticInfoPresenter = diagnosticInfoPresenter;
            _audioController = audioController;
            _playerSession = playerSession;
            _dataStorage = dataStorage;
            _gameSettings = gameSettings;
        }
        public void Enter(GameState prevState, object? context = null)
        {
            _logger.Log($"{nameof(InitialState)}.{nameof(Enter)}()");
            InitializeDataStorageAsync().Forget();
        }
        public void Exit(GameState nextState)
        {
            _logger.Log($"{nameof(InitialState)}.{nameof(Exit)}()");
        }

        private readonly ILogger _logger;
        private readonly IGameFlow _gameFlow;
        private readonly GameSettings _gameSettings;
        private readonly IMapObjectRegistry _mapObjectRegistry;
        private readonly IPresenter<IDiagnosticInfoView> _diagnosticInfoPresenter;
        private readonly IAudioController _audioController;
        private readonly IPlayerSession _playerSession;
        private readonly IDataStorage _dataStorage;
        private readonly Dictionary<string, object?> _saveSettingsData = new() { { "Settings", null } };
        private readonly Dictionary<string, Type> _loadSettingsMetadata = new() { { "Settings", typeof(GameSettings) } };
        private readonly Dictionary<string, object?> _saveResourceData = new()
        {
            { "MapObjectRegistry", null },
            { "ResourceManagerData", null }
        };
        private readonly Dictionary<string, Type> _loadResourceMetadata = new()
        {
            { "MapObjectRegistry", typeof(string) },
            { "ResourceManagerData", typeof(Dictionary<ResourceType, int>) }
        };
        private bool _isResourcesSaveStarted = false;
        private bool _isResourceDataDirty = false;
        private bool _isInternalSettingsChange = false;

        private async UniTask OnDataStorageInitializedAsync()
        {
            _saveSettingsData["Settings"] = _gameSettings;
            _gameSettings.SettingsChanged += OnSettingsChanged;
            _gameSettings.ShowFpsChanged += OnShowFpsChanged;
            _gameSettings.MusicVolumeChanged += OnMusicVolumeChanged;
            _gameSettings.QualityLevelChanged += OnQualityLevelChanged;
#if !UNITY_WEBGL
            await UniTask.SwitchToMainThread();
#endif
            if (_gameSettings.ShowFps)
            {
                _diagnosticInfoPresenter.Enable();
            }
            _audioController.MasterVolume = _gameSettings.MusicVolume;
#if !UNITY_WEBGL
            await UniTask.SwitchToThreadPool();
#endif
            _saveResourceData["ResourceManagerData"] = _playerSession.ResourceManager.Resources;
            _playerSession.ResourceManager.ResourceChanged += OnResourceChanged;
            _mapObjectRegistry.Registred += (mapObject) =>
            {
                mapObject.StateChanged += OnResourceDataChanged;
            };
            _mapObjectRegistry.Unregistred += (mapObject) =>
            {
                mapObject.StateChanged -= OnResourceDataChanged;
            };

            try
            {
                var data = await _dataStorage.LoadAsync(_loadSettingsMetadata);
                if (data.TryGetValue("Settings", out var gameSettingsObj) && gameSettingsObj is GameSettings settings)
                {
#if !UNITY_WEBGL
                    await UniTask.SwitchToMainThread();
#endif
                    _isInternalSettingsChange = true;
                    _gameSettings.FreezeSettingsChanged = true;
                    _gameSettings.ShowFps = settings.ShowFps;
                    _gameSettings.MusicVolume = Mathf.Clamp01(settings.MusicVolume);
                    _gameSettings.QualityLevel = settings.QualityLevel;
                    _gameSettings.FreezeSettingsChanged = false;
                    _isInternalSettingsChange = false;
#if !UNITY_WEBGL
                    await UniTask.SwitchToThreadPool();
#endif
                }
                else
                {
                    _logger.LogWarning(nameof(InitialState), $"No saved data for {"Settings"}");
                }
                await LoadSavedDataAsync();
#if !UNITY_WEBGL
                await UniTask.SwitchToMainThread();
#endif
                _gameFlow.StartGameplay();
            }
            catch(Exception ex)
            {
                _logger.LogError(nameof(InitialState), $"Player data loading failed: {ex}");
            }
        }

        private void OnSettingsChanged()
        {
            if (!_isInternalSettingsChange)
            {
                _dataStorage.SaveAsync(_saveSettingsData).AsUniTask().Forget();
            }
        }

        private void OnShowFpsChanged(bool state)
        {
            if (state)
            {
                _diagnosticInfoPresenter.Enable();
            }
            else
            {
                _diagnosticInfoPresenter.Disable();
            }
        }
        private void OnMusicVolumeChanged(float value)
        {
            _audioController.MasterVolume = value;
        }
        private void OnQualityLevelChanged(Abstractions.QualityLevel qualityLevel)
        {
        }
        private void OnResourceChanged(ResourceType type, int amount) => OnResourceDataChanged();
        private void OnResourceDataChanged()
        {
            _isResourceDataDirty = true;
            if (!_isResourcesSaveStarted)
            {
                _isResourcesSaveStarted = true;
                SaveResourcesDataAsync().Forget();
            }
        }
        private async UniTask InitializeDataStorageAsync()
        {
            try
            {
                _logger.Log("Try storage initialize");
                await _dataStorage.InitializeAsync();
                _logger.Log("Try storage initialized");
                await OnDataStorageInitializedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(InitialState), $"{ex.GetType()} occured while trying to initialize data storage: {ex}");
            }
        }
        private async UniTask SaveResourcesDataAsync()
        {
            var stopwatch = new Stopwatch();
            var updatePeriodTime = .5f;
            while (_isResourceDataDirty)
            {
                _isResourceDataDirty = false;
                stopwatch.Restart();
                _saveResourceData["ResourceManagerData"] = _playerSession.ResourceManager.Resources;
                if (_mapObjectRegistry is IStringSerializable serializableMapObjectRegistry)
                {
#if !UNITY_WEBGL
                    await UniTask.SwitchToMainThread();
#endif
                    _saveResourceData["MapObjectRegistry"] = serializableMapObjectRegistry.ToSerializedString();
#if !UNITY_WEBGL
                    _logger.Log("Switch to thread pool");
                    await UniTask.SwitchToThreadPool();
#endif
                }
                await _dataStorage.SaveAsync(_saveResourceData);
                stopwatch.Stop();
                var elapsedTime = stopwatch.ElapsedMilliseconds / 1000f;
                if (elapsedTime < updatePeriodTime)
                {
                    await UniTask.WaitForSeconds(updatePeriodTime - elapsedTime);
                }
            }
            _isResourcesSaveStarted = false;
        }
        private async UniTask LoadSavedDataAsync()
        {
            _logger.Log("Loading resource data");
            var resourcesData = await _dataStorage.LoadAsync(_loadResourceMetadata);
            _logger.Log("Resource data loaded");
            if (_mapObjectRegistry is IStringSerializable serializableMapObjectRegistry)
            {
                if (resourcesData.TryGetValue("MapObjectRegistry", out var mapRegistryData) &&
                    mapRegistryData is string mapRegistryStr &&
                    !string.IsNullOrEmpty(mapRegistryStr))
                {
                    serializableMapObjectRegistry.FromSerializedString(mapRegistryStr);
                }
                else
                {
                    _logger.LogWarning(nameof(InitialState), $"No saved data for {"MapObjectRegistry"}");
                }
            }
            else
            {
                _logger.LogWarning(nameof(InitialState), $"Can't load {nameof(IMapObjectRegistry)} since it doesn't implement {nameof(IStringSerializable)} interface");
            }
            _logger.Log("Resource manager settings");
            if (resourcesData.TryGetValue("ResourceManagerData", out var resourceManagerData) &&
                resourceManagerData is Dictionary<ResourceType, int> resources)
            {
#if !UNITY_WEBGL
                await UniTask.SwitchToMainThread();
#endif
                _playerSession.ResourceManager.FreezeResourceChanged = true;
                try
                {
                    foreach (var resource in resources)
                    {
                        var resAmount = resource.Value;
                        if (resAmount > 0)
                        {
                            _playerSession.ResourceManager.AddResource(resource.Key, resAmount);
                        }
                    }
                }
                finally
                {
                    _playerSession.ResourceManager.FreezeResourceChanged = false;
                }
            }
            else
            {
                _logger.LogWarning(nameof(InitialState), $"No saved data for {"ResourceManagerData"}");
            }
        }
    }
}