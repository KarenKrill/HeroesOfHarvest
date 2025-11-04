using UnityEngine;
using Zenject;

using HeroesOfHarvest.Movement;

namespace HeroesOfHarvest
{
    public class MainSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UnitMover>()
                .FromInstance(_unitMover)
                .AsSingle()
                .OnInstantiated((ctx, target) =>
                {
                    var playerSession = ctx.Container.Resolve<PlayerSession>();
                    playerSession.ActiveUnit = _unitBehaviour;
                })
                .NonLazy();
        }

        [SerializeField]
        private UnitMover _unitMover;
        [SerializeField]
        private UnitInteractor _unitBehaviour;
    }
}
