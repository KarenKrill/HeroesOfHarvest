#nullable enable

using UnityEngine;

using KarenKrill.UniCore.Input.Abstractions;
using KarenKrill.UniCore.StateSystem.Abstractions;

namespace HeroesOfHarvest.GameStates
{
    using Abstractions;

    public class GameplayState : PresentableStateHandlerBase<GameState>, IStateHandler<GameState>
    {
        public override GameState State => GameState.Gameplay;

        public GameplayState(ILogger logger,
            IGameFlow gameFlow,
            IBasicActionsProvider actionsProvider,
            IBasicPlayerActionsProvider playerActionsProvider)
        {
            _logger = logger;
            _gameFlow = gameFlow;
            _actionsProvider = actionsProvider;
            _playerActionsProvider = playerActionsProvider;
        }
        public override void Enter(GameState prevState, object? context = null)
        {
            base.Enter(prevState);

            _playerActionsProvider.Pause += OnPause;
            _actionsProvider.SetActionMap(ActionMap.Player);

            _logger.Log($"{nameof(GameplayState)}.{nameof(Enter)}()");
        }
        public override void Exit(GameState nextState)
        {
            base.Exit(nextState);

            _playerActionsProvider.Pause -= OnPause;
            _actionsProvider.SetActionMap(ActionMap.UI);
            _logger.Log($"{nameof(GameplayState)}.{nameof(Exit)}()");
        }

        private readonly ILogger _logger;
        private readonly IGameFlow _gameFlow;

        private readonly IBasicActionsProvider _actionsProvider;
        private readonly IBasicPlayerActionsProvider _playerActionsProvider;

        private void OnPause()
        {
            _gameFlow.Pause();
        }
    }
    public class GameplayStateContext
    {
        public bool FirstStart;
        public GameplayStateContext(bool firstStart)
        {
            FirstStart = firstStart;
        }
    }
}