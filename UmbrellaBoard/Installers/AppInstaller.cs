using System;
using UmbrellaBoard.AffinityPatches;
using Zenject;

namespace UmbrellaBoard.Installers
{
    internal class AppInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<MainFlowCoordinatorPatch>().AsSingle();
            Container.Bind<FlowCoordinatorPatch>().AsSingle();
        }
    }
}
