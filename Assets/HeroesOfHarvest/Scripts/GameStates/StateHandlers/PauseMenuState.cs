#nullable enable

using UnityEngine;

using KarenKrill.UniCore.Input.Abstractions;
using KarenKrill.UniCore.StateSystem.Abstractions;

namespace HeroesOfHarvest.GameStates
{
    using Abstractions;
    using UI.Presenters.Abstractions;

    public class PauseMenuState : PresentableStateHandlerBase<GameState>, IStateHandler<GameState>
    {
        public override GameState State => GameState.Pause;

        public PauseMenuState(ILogger logger,
            IGameFlow gameFlow,
            IStateSwitcher<GameState> stateSwitcher,
            IBasicActionsProvider actionsProvider,
            IBasicUIActionsProvider uiActionsProvider,
            IPauseMenuPresenter pauseMenuPresenter)
            : base(pauseMenuPresenter)
        {
            _logger = logger;
            _gameFlow = gameFlow;
            _stateSwitcher = stateSwitcher;
            _actionsProvider = actionsProvider;
            _uiActionsProvider = uiActionsProvider;
            _pauseMenuPresenter = pauseMenuPresenter;

        }
        public override void Enter(GameState prevState, object? context = null)
        {
            base.Enter(prevState);
            _previousState = prevState;
            _previousTimeScale = Time.timeScale;
            Time.timeScale = 0;
            _logger.Log($"{GetType().Name}.{nameof(Enter)}()");
            _uiActionsProvider.Cancel += OnResumeRequested;
            _pauseMenuPresenter.Resume += OnResumeRequested;
            _pauseMenuPresenter.Exit += OnExit;
            _actionsProvider.SetActionMap(ActionMap.UI);
        }
        public override void Exit(GameState nextState)
        {
            _logger.Log($"{GetType().Name}.{nameof(Exit)}()");
            _uiActionsProvider.Cancel -= OnResumeRequested;
            _pauseMenuPresenter.Resume -= OnResumeRequested;
            Time.timeScale = _previousTimeScale;
            base.Exit(nextState);
        }

        private readonly ILogger _logger;
        private readonly IGameFlow _gameFlow;
        private readonly IStateSwitcher<GameState> _stateSwitcher;
        private GameState _previousState;
        private readonly IBasicActionsProvider _actionsProvider;
        private readonly IBasicUIActionsProvider _uiActionsProvider;
        private readonly IPauseMenuPresenter _pauseMenuPresenter;
        private float _previousTimeScale;

        private void OnResumeRequested()
        {
            _stateSwitcher.TransitTo(_previousState);
        }
        private void OnExit() => _gameFlow.Exit();
    }
}
