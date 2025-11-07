namespace HeroesOfHarvest.Abstractions
{
    public interface ISaveService
    {
        void RunInBackground();
        void Stop();
    }
}
