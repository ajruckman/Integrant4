using System.Collections.Generic;
using Integrant4.Element;
using Integrant4.Fundament;
using Integrant4.Resources;

namespace Web
{
    internal static class Configuration
    {
        internal static readonly ResourceSet ResourceSet;

        static Configuration()
        {
            ResourceSet = new ResourceSet(nameof(Web), nameof(Configuration),
                stylesheetsExternal: new HashSet<string>
                {
                    "css/Local.css",
                    "css/Local.{{ThemeVariant}}.css",
                },
                dependencies: new List<ResourceSet>
                {
                    ResourceSets.Inputs.Interop,
                    ResourceSets.Bits,

                    Resources.Icons.Bootstrap,
                    Resources.Fonts.SansSerif.Inter,
                    Resources.Fonts.Monospace.JetBrainsMono,

                    // ResourceSets.LocalCSS,
                });
        }
    }
}