namespace HeroesOfHarvest.Abstractions
{
    public interface IPlayerSession
    {
        IUnit ActiveUnit { get; }
        IResourceManager ResourceManager { get; }
    }
}
