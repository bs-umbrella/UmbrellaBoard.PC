using IPA;
using IPALogger = IPA.Logging.Logger;
using IPAConfig = IPA.Config.Config;
using SiraUtil.Zenject;
using UmbrellaBoard.Installers;
using UmbrellaBoard.UI.Tags;

using IPA.Config.Stores;
using Zenject;
using UmbrellaBoard.UI.TypeHandlers;

namespace UmbrellaBoard
{
    [Plugin(RuntimeOptions.SingleStartInit), NoEnableDisable]
    public class Plugin
    {
        [Init]
        public Plugin(IPALogger logger, IPAConfig ipaConfig, Zenjector zenjector)
        {
            Config config = ipaConfig.Generated<Config>();

            // register our tags
            BeatSaberMarkupLanguage.BSMLParser.instance.RegisterTag(new CarouselTag());

            BeatSaberMarkupLanguage.BSMLParser.instance.RegisterTag(new OpenPageButtonTag());
            BeatSaberMarkupLanguage.BSMLParser.instance.RegisterTag(new OpenPageClickableImageTag());
            BeatSaberMarkupLanguage.BSMLParser.instance.RegisterTag(new OpenPageClickableText());
            BeatSaberMarkupLanguage.BSMLParser.instance.RegisterTag(new OpenPagePageButtonTag());
            BeatSaberMarkupLanguage.BSMLParser.instance.RegisterTag(new OpenPagePrimaryButtonTag());
            
            BeatSaberMarkupLanguage.BSMLParser.instance.RegisterTypeHandler(new PageOpenerHandler());
            BeatSaberMarkupLanguage.BSMLParser.instance.RegisterTypeHandler(new CarouselHandler());

            zenjector.UseLogger(logger);
            zenjector.Install<MenuInstaller>(Location.Menu);
            zenjector.Install<AppInstaller>(Location.App);
            zenjector.Install(Location.App, (Container) => Container.BindInstance(config));
        }
    }
}