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
        [AffinityPrefix]
        [AffinityPatch(typeof(FlowCoordinator), nameof(FlowCoordinator.ProvideInitialViewControllers))]
        private void Prefix(FlowCoordinator __instance, ref ViewController mainViewController, ref ViewController leftScreenViewController, ref ViewController rightScreenViewController, ref ViewController bottomScreenViewController, ref ViewController topScreenViewController)
        {
            mainViewController = mainViewController ?? __instance._providedMainViewController;
            leftScreenViewController = leftScreenViewController ?? __instance._providedLeftScreenViewController;
            rightScreenViewController = rightScreenViewController ?? __instance._providedRightScreenViewController;
            topScreenViewController = topScreenViewController ?? __instance._providedTopScreenViewController;
            bottomScreenViewController = bottomScreenViewController ?? __instance._providedBottomScreenViewController;
        }
    }
}
