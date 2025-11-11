#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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
            _ = UniTask.RunOnThreadPool(async () =>
            {
                try
                {
                    await _dataStorage.InitializeAsync();
                    await OnDataStorageInitializedAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(nameof(InitialState), $"{ex.GetType()} occured while trying to initialize data storage: {ex}");
                }
            });
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
        

        private async UniTask OnDataStorageInitializedAsync()
        {
            _saveSettingsData["Settings"] = _gameSettings;
            async UniTask SaveGameSettingsAsync() => await _dataStorage.SaveAsync(_saveSettingsData);
            _gameSettings.SettingsChanged += async () => await UniTask.RunOnThreadPool(SaveGameSettingsAsync);
            _gameSettings.ShowFpsChanged += OnShowFpsChanged;
            _gameSettings.MusicVolumeChanged += OnMusicVolumeChanged;
            _gameSettings.QualityLevelChanged += OnQualityLevelChanged;
            await UniTask.SwitchToMainThread();
            if (_gameSettings.ShowFps)
            {
                _diagnosticInfoPresenter.Enable();
            }
            _audioController.MasterVolume = _gameSettings.MusicVolume;
            await UniTask.SwitchToThreadPool();

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
                    await UniTask.SwitchToMainThread();
                    _gameSettings.FreezeSettingsChanged = true;
                    _gameSettings.ShowFps = settings.ShowFps;
                    _gameSettings.MusicVolume = Mathf.Clamp01(settings.MusicVolume);
                    _gameSettings.QualityLevel = settings.QualityLevel;
                    _gameSettings.FreezeSettingsChanged = false;
                    await UniTask.SwitchToThreadPool();
                }
                await LoadSavedDataAsync();
                await UniTask.SwitchToMainThread();
                _gameFlow.StartGameplay();
            }
            catch(Exception ex)
            {
                _logger.LogError(nameof(InitialState), $"Player data loading failed: {ex}");
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
            _ = UniTask.RunOnThreadPool(async () => await _dataStorage.SaveAsync(_saveSettingsData));
        }
        private void OnMusicVolumeChanged(float value)
        {
            _audioController.MasterVolume = value;
            _ = UniTask.RunOnThreadPool(async () => await _dataStorage.SaveAsync(_saveSettingsData));
        }
        private void OnQualityLevelChanged(Abstractions.QualityLevel qualityLevel)
        {
            _ = UniTask.RunOnThreadPool(async () => await _dataStorage.SaveAsync(_saveSettingsData));
        }
        private void OnResourceChanged(ResourceType type, int amount) => OnResourceDataChanged();
        private void OnResourceDataChanged()
        {
            _isResourceDataDirty = true;
            if (!_isResourcesSaveStarted)
            {
                _isResourcesSaveStarted = true;
                _ = UniTask.RunOnThreadPool(SaveResourcesDataAsync);
            }
        }
        private async Task SaveResourcesDataAsync()
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
                    await UniTask.SwitchToMainThread();
                    _saveResourceData["MapObjectRegistry"] = serializableMapObjectRegistry.ToSerializedString();
                    await UniTask.SwitchToThreadPool();
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

        private async Task LoadSavedDataAsync()
        {
            var resourcesData = await _dataStorage.LoadAsync(_loadResourceMetadata);
            if (_mapObjectRegistry is IStringSerializable serializableMapObjectRegistry)
            {
                if (resourcesData.TryGetValue("MapObjectRegistry", out var mapRegistryData) &&
                    mapRegistryData is string mapRegistryStr &&
                    !string.IsNullOrEmpty(mapRegistryStr))
                {
                    serializableMapObjectRegistry.FromSerializedString(mapRegistryStr);
                }
            }
            else
            {
                _logger.LogWarning(nameof(InitialState), $"Can't load {nameof(IMapObjectRegistry)} since it doesn't implement {nameof(IStringSerializable)} interface");
            }

            if (resourcesData.TryGetValue("ResourceManagerData", out var resourceManagerData) &&
                resourceManagerData is Dictionary<ResourceType, int> resources)
            {
                await UniTask.SwitchToMainThread();
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
        }
    }
}