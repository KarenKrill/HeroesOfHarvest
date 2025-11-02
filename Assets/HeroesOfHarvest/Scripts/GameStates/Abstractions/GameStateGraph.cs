using System.Collections.Generic;

using KarenKrill.UniCore.StateSystem.Abstractions;

namespace HeroesOfHarvest.Abstractions
{
    public class GameStateGraph : IStateGraph<GameState>
    {
        public GameState InitialState => GameState.Initial;

        public IDictionary<GameState, IList<GameState>> Transitions => _transitions;

        private readonly IDictionary<GameState, IList<GameState>> _transitions = new Dictionary<GameState, IList<GameState>>()
        {
            { GameState.Initial, new List<GameState> { GameState.Gameplay } },
            { GameState.Gameplay, new List<GameState> { GameState.Pause } },
            { GameState.Pause, new List<GameState> { GameState.Gameplay, GameState.Exit } },
            { GameState.Exit, new List<GameState>() }
        };
    }
}
