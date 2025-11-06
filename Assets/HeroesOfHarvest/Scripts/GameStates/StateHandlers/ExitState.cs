#nullable enable

using UnityEngine;

using KarenKrill.UniCore.StateSystem.Abstractions;

using HeroesOfHarvest.Abstractions;

namespace HeroesOfHarvest.GameStates
{
    public class ExitState : IStateHandler<GameState>
    {
        public GameState State => GameState.Exit;

        public ExitState(ILogger logger, IMapObjectRegistry mapObjectRegistry, IPlayerSession playerSession)
        {
            _logger = logger;
            _mapObjectRegistry = mapObjectRegistry;
            _playerSession = playerSession;
        }
        public void Enter(GameState prevState, object? context = null)
        {
            _logger.Log($"{nameof(ExitState)}.{nameof(Enter)}()");

            if (_mapObjectRegistry is IStringSerializable serializableMapObjectRegistry)
            {
                var serializedMapObjectRegistry = serializableMapObjectRegistry.ToSerializedString();
                PlayerPrefs.SetString("MapObjectRegistry", serializedMapObjectRegistry);
                _logger.Log($"MapObjectRegistry saved as: {serializedMapObjectRegistry}");
            }
            else
            {
                _logger.LogWarning(nameof(ExitState), $"Can't save {nameof(IMapObjectRegistry)} since it doesn't implement {nameof(IStringSerializable)} interface");
            }

            var resources = _playerSession.ResourceManager.GetResources();
            foreach (var res in resources)
            {
                PlayerPrefs.SetInt($"PlayerSession.{nameof(IPlayerSession.ResourceManager)}.{res.Key}", res.Value);
            }
        }
        public void Exit(GameState nextState) { }

        private readonly ILogger _logger;
        private readonly IMapObjectRegistry _mapObjectRegistry;
        private readonly IPlayerSession _playerSession;
    }
}