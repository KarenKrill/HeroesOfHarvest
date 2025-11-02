namespace HeroesOfHarvest.Abstractions
{
    public interface IGameFlow
    {
        GameState State { get; }
        
        void StartGameplay();
        void Play();
        void Pause();
        void Exit();
    }
}
