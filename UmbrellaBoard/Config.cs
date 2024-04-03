using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace UmbrellaBoard
{
    internal class Config
    {
        public string communitiesDiscoveryURL = "file://S:\\Program Files (x86)\\Steam\\steamapps\\common\\Beat Saber\\UserData\\UmbrellaBoard\\communities.json";

        [UseConverter(typeof(ListConverter<Community>))]
        public List<Community> enabledCommunities = new();

        [UseConverter(typeof(ListConverter<Community>))]
        public List<Community> disabledCommunities = new();
    }

    struct Community
    {
        public string communityName;
        public string communityBackgroundURL;
        public string communityPageURL;
    }
}
