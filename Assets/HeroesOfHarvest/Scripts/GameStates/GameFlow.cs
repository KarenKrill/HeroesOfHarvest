using UnityEngine;
using UnityEngine.SceneManagement;

using KarenKrill.UniCore.StateSystem.Abstractions;

namespace HeroesOfHarvest.GameStates
{
    using Abstractions;

    public class GameFlow : IGameFlow
    {
        public GameState State => _stateSwitcher.State;

        public GameFlow(IStateSwitcher<GameState> stateSwitcher)
        {
            _stateSwitcher = stateSwitcher;
        }
        public void StartGameplay()
        {
            _loadSceneAwaiter = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
            _loadSceneAwaiter.completed += OnGameplaySceneLoadCompleted;
        }
        public void Play()
        {
            _stateSwitcher.TransitTo(GameState.Gameplay);
        }
        public void Pause()
        {
            _stateSwitcher.TransitTo(GameState.Pause);
        }
        public void Exit()
        {
            _stateSwitcher.TransitTo(GameState.Exit);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else // для мобильных и веба не сработает, надо предусмотреть
            Application.Quit();
#endif
        }

        private readonly IStateSwitcher<GameState> _stateSwitcher;
        private AsyncOperation _loadSceneAwaiter;
        private void OnGameplaySceneLoadCompleted(AsyncOperation obj)
        {
            _stateSwitcher.TransitTo(GameState.Gameplay, new GameplayStateContext(true));
        }
    }
}
