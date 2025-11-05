namespace HeroesOfHarvest.Abstractions
{
    public interface IResourceFactoryMapObject : IMapObject
    {
        ResourceType ProducedResource { get; }
        int ProducedAmount { get; }
    }
}
