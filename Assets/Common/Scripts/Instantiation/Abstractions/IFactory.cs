namespace KarenKrill.Instantiation.Abstractions
{
    public interface IFactory { }

    public interface IFactory<out ProducedType> : IFactory
    {
        ProducedType Create();
    }

    public interface IFactory<out ProducedType, in ConsumedType> : IFactory
    {
        ProducedType Create(ConsumedType value);
    }

    public interface IBaseFactory<in ProducedTypeBase> : IFactory where ProducedTypeBase : class
    {
        ProducedType Create<ProducedType>() where ProducedType : ProducedTypeBase;
    }
}
