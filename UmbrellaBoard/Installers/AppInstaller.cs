using System;
using UmbrellaBoard.AffinityPatches;
using Zenject;

namespace UmbrellaBoard.Installers
{
    internal class AppInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MainFlowCoordinatorPatch>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<FlowCoordinatorPatch>().AsSingle().NonLazy();
        }
    }
}
