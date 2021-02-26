using System.Collections.Generic;
using Superset.Web.Resources;

namespace Web
{
    internal static class Configuration
    {
        internal static readonly ResourceSet ResourceSet;

        static Configuration()
        {
            ResourceSet = new ResourceSet(nameof(Web), nameof(Configuration),
                dependencies: new List<ResourceSet>
                {
                    Integrant4.Element.ResourceSets.Inputs.Interop,
                });
        }
    }
}