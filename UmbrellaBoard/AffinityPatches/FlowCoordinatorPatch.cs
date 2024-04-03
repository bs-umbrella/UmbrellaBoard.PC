using HMUI;
using SiraUtil.Affinity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbrellaBoard.AffinityPatches
{
    internal class FlowCoordinatorPatch : IAffinity
    {
        [AffinityPatch(typeof(FlowCoordinator), nameof(FlowCoordinator.ProvideInitialViewControllers))]
        private void Prefix(FlowCoordinator __instance, ViewController mainViewController, ViewController leftScreenViewController, ViewController rightScreenViewController, ViewController bottomScreenViewController, ViewController topScreenViewController)
        {
            mainViewController = mainViewController ?? __instance._providedMainViewController;
            leftScreenViewController = leftScreenViewController ?? __instance._providedLeftScreenViewController;
            rightScreenViewController = rightScreenViewController ?? __instance._providedRightScreenViewController;
            topScreenViewController = topScreenViewController ?? __instance._providedTopScreenViewController;
            bottomScreenViewController = bottomScreenViewController ?? __instance._providedBottomScreenViewController;
        }
    }
}
