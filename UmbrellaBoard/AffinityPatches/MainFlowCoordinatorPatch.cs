using HMUI;
using SiraUtil.Affinity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbrellaBoard.AffinityPatches
{
    internal class MainFlowCoordinatorPatch : IAffinity
    {
        [AffinityPatch(typeof(MainFlowCoordinator), nameof(MainFlowCoordinator.TopViewControllerWillChange))]
        private void Prefix(MainFlowCoordinator __instance, ViewController oldViewController, ViewController newViewController, ViewController.AnimationType animationType)
        {
            if (newViewController == __instance._mainMenuViewController)
            {
                __instance.SetLeftScreenViewController(__instance._providedLeftScreenViewController, animationType);
                __instance.SetRightScreenViewController(__instance._providedRightScreenViewController, animationType);
                __instance.SetBottomScreenViewController(__instance._providedBottomScreenViewController, animationType);
            }
            else
            {
                __instance.SetLeftScreenViewController(null, animationType);
                __instance.SetRightScreenViewController(null, animationType);
                __instance.SetBottomScreenViewController(null, animationType);
            }

            if (newViewController == __instance._playerOptionsViewController)
            {
                __instance.SetTitle(BGLib.Polyglot.Localization.Get("BUTTON_PLAYER_OPTIONS"), animationType);
                __instance.showBackButton = true;
                return;
            }

            if (newViewController == __instance._optionsViewController)
            {
                __instance.SetTitle(BGLib.Polyglot.Localization.Get("LABEL_OPTIONS"), animationType);
                __instance.showBackButton = true;
                return;
            }

            __instance.SetTitle("", animationType);
            __instance.showBackButton = false;
        }
    }
}
