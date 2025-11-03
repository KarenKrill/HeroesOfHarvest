using UnityEngine;
using Zenject;

namespace HeroesOfHarvest
{
    public class MainSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<NavMeshMoveBehaviour>().FromInstance(_playerUnitMover).AsSingle();
        }

        [SerializeField]
        private NavMeshMoveBehaviour _playerUnitMover;
    }
}
