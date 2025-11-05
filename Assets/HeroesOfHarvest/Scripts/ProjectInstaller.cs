using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

using KarenKrill.UniCore.Diagnostics;
using KarenKrill.UniCore.Interactions;
using KarenKrill.UniCore.Logging;
using KarenKrill.UniCore.StateSystem;
using KarenKrill.UniCore.StateSystem.Abstractions;
using KarenKrill.UniCore.UI.Presenters;
using KarenKrill.UniCore.UI.Presenters.Abstractions;
using KarenKrill.UniCore.UI.Views;
using KarenKrill.UniCore.Utilities;

namespace HeroesOfHarvest
{
    using Abstractions;
    using GameStates;
    using Input;

    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InstallSettings();
            InstallInput();
            InstallLogging();
            Container.BindInterfacesAndSelfTo<Canvas>().FromInstance(_worldUiRootCanvas).AsSingle();
            Container.BindInterfacesAndSelfTo<ResourceManager>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<ResourceIconRepository>().FromInstance(_resourceIconRepository).AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerSession>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<DiagnosticsProvider>().FromInstance(_diagnosticsProvider).AsSingle();
            Container.BindInterfacesAndSelfTo<InteractionTargetRegistry>().FromNew().AsSingle();
            Container.BindInterfacesAndSelfTo<GameFlow>().AsSingle();
            InstallGameStateMachine();
            InstallViewFactory();
            InstallPresenterBindings();
        }

        [SerializeField]
        private Canvas _uiRootCanvas;
        [SerializeField]
        private Canvas _worldUiRootCanvas;
        [SerializeField]
        private List<GameObject> _uiPrefabs;
        [SerializeField]
        private DiagnosticsProvider _diagnosticsProvider;
        [SerializeField]
        private ResourceIconRepository _resourceIconRepository;

        private void InstallLogging()
        {
#if DEBUG
            Container.Bind<ILogger>().To<Logger>().FromNew().AsSingle().WithArguments(new DebugLogHandler());
#else
            Container.Bind<ILogger>().To<StubLogger>().FromNew().AsSingle();
#endif
        }
        private void InstallSettings()
        {
            var qualityLevel = PlayerPrefs.GetInt("Settings.Graphics.QualityLevel", (int)QualityLevel.High);
            if (qualityLevel < 0 || qualityLevel > (int)QualityLevel.High)
            {
                qualityLevel = (int)QualityLevel.High;
            }
            var showFps = PlayerPrefs.GetInt("Settings.Diagnostic.ShowFps", 0);
            GameSettings gameSettings = new((QualityLevel)qualityLevel, showFps != 0);
            Container.Bind<GameSettings>().To<GameSettings>().FromInstance(gameSettings);
        }
        private void InstallInput()
        {
            Container.BindInterfacesAndSelfTo<InputActionService>().FromNew().AsSingle();
        }
        private void InstallGameStateMachine()
        {
            Container.Bind<IStateMachine<GameState>>()
                .To<StateMachine<GameState>>()
                .AsSingle()
                .WithArguments(new GameStateGraph())
                .OnInstantiated((context, instance) =>
                {
                    if (instance is IStateMachine<GameState> stateMachine)
                    {
                        context.Container.Bind<IStateSwitcher<GameState>>().FromInstance(stateMachine.StateSwitcher);
                    }
                })
                .NonLazy();
            var stateTypes = ReflectionUtilities.GetInheritorTypes(typeof(IStateHandler<GameState>), System.Type.EmptyTypes);
            foreach (var stateType in stateTypes)
            {
                Container.BindInterfacesTo(stateType).AsSingle();
            }
            Container.BindInterfacesTo<ManagedStateMachine<GameState>>().AsSingle().OnInstantiated((context, target) =>
            {
                if (target is ManagedStateMachine<GameState> managedStateMachine)
                {
                    managedStateMachine.Start();
                }
            }).NonLazy();
        }
        private void InstallViewFactory()
        {
            if (_uiRootCanvas == null)
            {
                _uiRootCanvas = FindFirstObjectByType<Canvas>(FindObjectsInactive.Exclude);
                if (_uiRootCanvas == null)
                {
                    var canvasGO = new GameObject(nameof(Canvas));
                    _uiRootCanvas = canvasGO.AddComponent<Canvas>();
                    _uiRootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasGO.AddComponent<CanvasScaler>();
                    canvasGO.AddComponent<GraphicRaycaster>();
                }
            }
            Container.BindInterfacesAndSelfTo<ViewFactory>().AsSingle().WithArguments(_uiRootCanvas.gameObject, _uiPrefabs);
        }
        private void InstallPresenterBindings()
        {
            Container.BindInterfacesAndSelfTo<PresenterNavigator>().AsTransient();
            var presenterTypes = ReflectionUtilities.GetInheritorTypes(typeof(IPresenter));
            foreach (var presenterType in presenterTypes)
            {
                Container.BindInterfacesTo(presenterType).FromNew().AsSingle();
            }
        }
    }
}
