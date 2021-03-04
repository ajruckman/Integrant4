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
                dependencies: new[]
                {
                    ResourceSets.Inputs.Interop,
                    ResourceSets.Bits,
                    ResourceSets.Constructs,

                    Resources.Icons.Bootstrap,
                    Resources.Fonts.SansSerif.Inter,
                    Resources.Fonts.Monospace.JetBrainsMono,

                    Integrant4.Element.ResourceSets.Overrides.MiniBar,
                    Integrant4.Element.ResourceSets.Overrides.Tippy,
                    Resources.Libraries.Tippy,

                    // ResourceSets.LocalCSS,
                });
        }
    }
}