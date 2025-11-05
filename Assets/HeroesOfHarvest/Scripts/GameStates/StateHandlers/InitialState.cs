#nullable enable

using UnityEngine;

using KarenKrill.UniCore.StateSystem.Abstractions;
using KarenKrill.UniCore.UI.Presenters.Abstractions;

using HeroesOfHarvest.UI.Views.Abstractions;
using HeroesOfHarvest.Abstractions;

namespace HeroesOfHarvest.GameStates
{
    public class InitialState : IStateHandler<GameState>
    {
        public GameState State => GameState.Initial;

        public InitialState(ILogger logger,
            IGameFlow gameFlow,
            IMapObjectRegistry mapObjectRegistry,
            IPresenter<IDiagnosticInfoView> diagnosticInfoPresenter,
            GameSettings gameSettings)
        {
            _logger = logger;
            _gameFlow = gameFlow;
            _mapObjectRegistry = mapObjectRegistry;
            _diagnosticInfoPresenter = diagnosticInfoPresenter;
            gameSettings.ShowFpsChanged += OnShowFpsChanged;
        }
        public void Enter(GameState prevState, object? context = null)
        {
            _logger.Log($"{nameof(InitialState)}.{nameof(Enter)}()");
            LoadSavedData();
            _gameFlow.StartGameplay();
        }
        public void Exit(GameState nextState)
        {
            _logger.Log($"{nameof(InitialState)}.{nameof(Exit)}()");
        }

        private readonly ILogger _logger;
        private readonly IGameFlow _gameFlow;
        private readonly IMapObjectRegistry _mapObjectRegistry;
        private readonly IPresenter<IDiagnosticInfoView> _diagnosticInfoPresenter;

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
                Debug.Log($"Loaded MapObjectRegistry: {serializedMapObjectRegistry}");
            }
            else
            {
                Debug.LogWarning($"Can't load {nameof(IMapObjectRegistry)} since it doesn't implement {nameof(IStringSerializable)} interface");
            }
        }
    }
}