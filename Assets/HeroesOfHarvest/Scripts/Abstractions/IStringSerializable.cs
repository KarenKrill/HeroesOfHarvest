namespace HeroesOfHarvest.Abstractions
{
    public interface IStringSerializable
    {
        string ToSerializedString();
        void FromSerializedString(string serializedString);
    }
}
