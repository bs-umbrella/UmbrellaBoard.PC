using UmbrellaBoard.Views;
using Zenject;

namespace UmbrellaBoard.Installers
{
    internal class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CommunitiesView>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<PageView>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardSetup>().AsSingle().NonLazy();

            Container.Bind<DownloaderUtility>().AsSingle();
        }
    }
}
