using System.Collections.Generic;
using Integrant4.Element;
using Integrant4.Fundament;
using Integrant4.Resources;

namespace Web
{
    public static class Configuration
    {
        public static readonly ResourceSet ResourceSet;

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
                    ResourceSets.I4App,

                    ResourceSets.Inputs,
                    ResourceSets.Bits,
                    ResourceSets.Charts,
                    ResourceSets.Constructs,
                    ResourceSets.Layouts,

                    Resources.Icons.Bootstrap,
                    Resources.Fonts.SansSerif.Inter,
                    Resources.Fonts.Monospace.JetBrainsMono,

                    ResourceSets.Overrides.Blazor,
                    ResourceSets.Overrides.MiniBar,
                    ResourceSets.Overrides.Tippy,
                    ResourceSets.Overrides.VariantLoader,
                    Resources.Libraries.Tippy,

                    // ResourceSets.LocalCSS,
                });
        }
    }
}