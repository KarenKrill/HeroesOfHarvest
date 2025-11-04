using HeroesOfHarvest.Abstractions;

namespace HeroesOfHarvest
{
    public class PlayerSession : IPlayerSession
    {
        public IUnit ActiveUnit { get; set; }
        public IResourceManager ResourceManager { get; }

        public PlayerSession(IResourceManager resourceManager)
        {
            ResourceManager = resourceManager;
        }
    }
}
