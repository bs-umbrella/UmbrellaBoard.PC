using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using IPA.Utilities;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace UmbrellaBoard
{
    internal class Config
    {
        public string communitiesDiscoveryURL { get => $"file://{UnityGame.UserDataPath}\\UmbrellaBoard\\communities.json"; }

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
