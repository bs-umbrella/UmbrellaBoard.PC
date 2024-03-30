using IPA;
using IPALogger = IPA.Logging.Logger;
using SiraUtil.Zenject;

namespace UmbrellaBoard
{
    [Plugin(RuntimeOptions.SingleStartInit), NoEnableDisable]
    public class Plugin
    {
        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector)
        {
            zenjector.UseLogger(logger);
        }
    }
}
