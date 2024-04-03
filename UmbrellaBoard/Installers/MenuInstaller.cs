using UmbrellaBoard.UI.Views;
using Zenject;

namespace UmbrellaBoard.Installers
{
    internal class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<DownloaderUtility>().AsSingle();

            Container.BindInterfacesAndSelfTo<CommunitiesView>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<PageView>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<CommunityConfigurationView>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardViewController>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardSetup>().AsSingle().NonLazy();

        }
    }
}
