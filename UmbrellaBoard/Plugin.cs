using IPA;
using IPALogger = IPA.Logging.Logger;
using IPAConfig = IPA.Config.Config;
using SiraUtil.Zenject;
using UmbrellaBoard.Installers;
using IPA.Config.Stores;
using Zenject;

namespace UmbrellaBoard
{
    [Plugin(RuntimeOptions.SingleStartInit), NoEnableDisable]
    public class Plugin
    {
        [Init]
        public Plugin(IPALogger logger, IPAConfig ipaConfig, Zenjector zenjector)
        {
            Config config = ipaConfig.Generated<Config>();

            zenjector.UseLogger(logger);
            zenjector.Install<MenuInstaller>(Location.Menu);
            zenjector.Install<AppInstaller>(Location.App);
            zenjector.Install(Location.App, (Container) => Container.BindInstance(config));
        }
    }
}