using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace UmbrellaBoard
{
    internal class Config
    {
        internal string communitiesDiscoveryURL = "";
        internal List<Community> enabledCommunities;
        internal List<Community> disabledCommunities;
    }

    struct Community
    {
        internal string communityName;
        internal string communityBackgroundURL;
        internal string communityPageURL;
    }
}
