#nullable enable

using UnityEngine;

using KarenKrill.UniCore.StateSystem.Abstractions;

using HeroesOfHarvest.Abstractions;

namespace HeroesOfHarvest.GameStates
{
    public class ExitState : IStateHandler<GameState>
    {
        public GameState State => GameState.Exit;

        public ExitState(ILogger logger)
        {
            _logger = logger;
        }
        public void Enter(GameState prevState, object? context = null)
        {
            _logger.Log($"{nameof(ExitState)}.{nameof(Enter)}()");
        }
        public void Exit(GameState nextState) { }

        private readonly ILogger _logger;
    }
}