#nullable enable

using UnityEngine;

using KarenKrill.UniCore.StateSystem.Abstractions;

using HeroesOfHarvest.Abstractions;

namespace HeroesOfHarvest.GameStates
{
    public class ExitState : IStateHandler<GameState>
    {
        public GameState State => GameState.Exit;

        public ExitState(ILogger logger, ISaveService saveService)
        {
            _logger = logger;
            _saveService = saveService;
        }
        public void Enter(GameState prevState, object? context = null)
        {
            _logger.Log($"{nameof(ExitState)}.{nameof(Enter)}()");
            _saveService.Stop();
        }
        public void Exit(GameState nextState) { }

        private readonly ILogger _logger;
        private readonly ISaveService _saveService;
    }
}