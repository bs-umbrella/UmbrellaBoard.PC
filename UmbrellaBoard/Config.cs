using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace UmbrellaBoard
{
    internal class Config
    {
        public string communitiesDiscoveryURL = "file://S:\\Program Files (x86)\\Steam\\steamapps\\common\\Beat Saber\\UserData\\UmbrellaBoard\\communities.json";

        public List<Community> _enabledCommunities = new();
        public List<Community> EnabledCommunities 
        {
            get => _enabledCommunities ?? (_enabledCommunities = new());
            private set => _enabledCommunities = value;
        }

        public List<Community> _disabledCommunities = new();
        public List<Community> DisabledCommunities 
        { 
            get => _disabledCommunities ?? (_disabledCommunities = new());
            private set => _disabledCommunities = value;
        }
    }

    struct Community
    {
        public string communityName;
        public string communityBackgroundURL;
        public string communityPageURL;
    }
}
