using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using IPA.Utilities;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace UmbrellaBoard
{
    internal class Config
    {
        // TODO: update default value to point at our repo
        public virtual string CommunitiesDiscoveryURL { get; set; } = $"file://{UnityGame.UserDataPath}\\UmbrellaBoard\\communities.json";

        [UseConverter(typeof(ListConverter<Community>))]
        public virtual List<Community> EnabledCommunities { get; set; } = new();

        [UseConverter(typeof(ListConverter<Community>))]
        public virtual List<Community> DisabledCommunities { get; set; } = new();
    }

    struct Community
    {
        public string communityName;
        public string communityBackgroundURL;
        public string communityPageURL;
    }
}
