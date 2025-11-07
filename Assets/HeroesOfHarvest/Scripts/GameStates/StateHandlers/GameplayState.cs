#nullable enable

using UnityEngine;

using KarenKrill.UniCore.Input.Abstractions;
using KarenKrill.UniCore.StateSystem.Abstractions;
using KarenKrill.UniCore.UI.Presenters.Abstractions;

using HeroesOfHarvest.UI.Views.Abstractions;
using HeroesOfHarvest.Abstractions;
using HeroesOfHarvest.UI.Presenters.Abstractions;

namespace HeroesOfHarvest.GameStates
{
    public class GameplayState : PresentableStateHandlerBase<GameState>, IStateHandler<GameState>
    {
        public override GameState State => GameState.Gameplay;

        public GameplayState(ILogger logger,
            IGameFlow gameFlow,
            IBasicActionsProvider actionsProvider,
            IBasicPlayerActionsProvider playerActionsProvider,
            IPresenter<IResourceManagerView> resourceManagerPresenter,
            IInGameMenuPresenter inGameMenuPresenter)
            : base(resourceManagerPresenter, inGameMenuPresenter)
        {
            _logger = logger;
            _gameFlow = gameFlow;
            _actionsProvider = actionsProvider;
            _playerActionsProvider = playerActionsProvider;
            _inGameMenuPresenter = inGameMenuPresenter;
        }
        public override void Enter(GameState prevState, object? context = null)
        {
            base.Enter(prevState);

            _playerActionsProvider.Pause += OnPause;
            _inGameMenuPresenter.Pause += OnPause;
            _actionsProvider.SetActionMap(ActionMap.Player);
            _logger.Log($"{nameof(GameplayState)}.{nameof(Enter)}()");
        }
        public override void Exit(GameState nextState)
        {
            base.Exit(nextState);

            _playerActionsProvider.Pause -= OnPause;
            _inGameMenuPresenter.Pause -= OnPause;
            _actionsProvider.SetActionMap(ActionMap.UI);
            _logger.Log($"{nameof(GameplayState)}.{nameof(Exit)}()");
        }

        private readonly ILogger _logger;
        private readonly IGameFlow _gameFlow;

        private readonly IBasicActionsProvider _actionsProvider;
        private readonly IBasicPlayerActionsProvider _playerActionsProvider;
        private readonly IInGameMenuPresenter _inGameMenuPresenter;

        private void OnPause()
        {
            _gameFlow.Pause();
        }
    }
    public class GameplayStateContext
    {
        /// <summary>
        /// First gameplay state enter after application start
        /// </summary>
        public bool FirstStart;
        public GameplayStateContext(bool firstStart)
        {
            FirstStart = firstStart;
        }
    }
}