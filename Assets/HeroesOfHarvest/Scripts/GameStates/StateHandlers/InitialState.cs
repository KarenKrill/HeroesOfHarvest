#nullable enable

using HeroesOfHarvest.Abstractions;
using HeroesOfHarvest.UI.Views.Abstractions;
using KarenKrill.UniCore.StateSystem.Abstractions;
using KarenKrill.UniCore.UI.Presenters.Abstractions;
using UnityEngine;

namespace HeroesOfHarvest.GameStates
{
    public class InitialState : IStateHandler<GameState>
    {
        public GameState State => GameState.Initial;

        public InitialState(ILogger logger,
            ISaveService saveService,
            IGameFlow gameFlow,
            IMapObjectRegistry mapObjectRegistry,
            IPresenter<IDiagnosticInfoView> diagnosticInfoPresenter,
            IAudioController audioController,
            IPlayerSession playerSession,
            GameSettings gameSettings)
        {
            _logger = logger;
            _saveService = saveService;
            _gameFlow = gameFlow;
            _mapObjectRegistry = mapObjectRegistry;
            _diagnosticInfoPresenter = diagnosticInfoPresenter;
            _audioController = audioController;
            _playerSession = playerSession;
            _gameSettings = gameSettings;
            _gameSettings.ShowFpsChanged += OnShowFpsChanged;
            _gameSettings.MusicVolumeChanged += OnMusicVolumeChanged;
        }
        public void Enter(GameState prevState, object? context = null)
        {
            _logger.Log($"{nameof(InitialState)}.{nameof(Enter)}()");
            if (_gameSettings.ShowFps)
            {
                _logger.LogWarning(nameof(InitialState), "Enable diagnostics");
                _diagnosticInfoPresenter.Enable();
            }
            _audioController.MasterVolume = _gameSettings.MusicVolume;
            LoadSavedData();
            _saveService.RunInBackground();
            _gameFlow.StartGameplay();
        }
        public void Exit(GameState nextState)
        {
            _logger.Log($"{nameof(InitialState)}.{nameof(Exit)}()");
        }

        private readonly ILogger _logger;
        private readonly ISaveService _saveService;
        private readonly IGameFlow _gameFlow;
        private readonly GameSettings _gameSettings;
        private readonly IMapObjectRegistry _mapObjectRegistry;
        private readonly IPresenter<IDiagnosticInfoView> _diagnosticInfoPresenter;
        private readonly IAudioController _audioController;
        private readonly IPlayerSession _playerSession;

        private void OnShowFpsChanged(bool state)
        {
            if (state)
            {
                _logger.LogWarning(nameof(InitialState), "Enable diagnostics");
                _diagnosticInfoPresenter.Enable();
            }
            else
            {
                _logger.LogWarning(nameof(InitialState), "Disable diagnostics");
                _diagnosticInfoPresenter.Disable();
            }
        }
        private void OnMusicVolumeChanged(float value)
        {
            _audioController.MasterVolume = value;
        }

        private void LoadSavedData()
        {
            // load map objects to the registry
            if (_mapObjectRegistry is IStringSerializable serializableMapObjectRegistry)
            {
                var serializedMapObjectRegistry = PlayerPrefs.GetString("MapObjectRegistry");
                if (!string.IsNullOrEmpty(serializedMapObjectRegistry))
                {
                    serializableMapObjectRegistry.FromSerializedString(serializedMapObjectRegistry);
                }
            }
            else
            {
                Debug.LogWarning($"Can't load {nameof(IMapObjectRegistry)} since it doesn't implement {nameof(IStringSerializable)} interface");
            }

            var resNames = System.Enum.GetNames(typeof(ResourceType));
            foreach (var resName in resNames)
            {
                var resAmount = PlayerPrefs.GetInt($"PlayerSession.{nameof(IPlayerSession.ResourceManager)}.{resName}", -1);
                if (resAmount > 0 && System.Enum.TryParse(typeof(ResourceType), resName, out var resType))
                {
                    _playerSession.ResourceManager.AddResource((ResourceType)resType, resAmount);
                }
            }
        }
    }
}