using System.Collections.Generic;
using Integrant4.Fundament;

namespace Integrant4.Element
{
    public static class ResourceSets
    {
        public static class Inputs
        {
            public static readonly ResourceSet Interop = new ResourceSet
            (
                $"{nameof(Integrant4)}.{nameof(Element)}",
                $"{nameof(Inputs)}.{nameof(Interop)}",
                scriptsInternal: new HashSet<string> {"js/Inputs.js"}
            );
        }

        public static readonly ResourceSet Constructs = new
        (
            $"{nameof(Integrant4)}.{nameof(Element)}",
            $"{nameof(Constructs)}",
            stylesheetsInternal: new HashSet<string>
            {
                "css/Constructs/Constructs.css", "css/Constructs/Constructs.{{ThemeVariant}}.css",
            }
        );

        public static readonly ResourceSet Bits = new
        (
            $"{nameof(Integrant4)}.{nameof(Element)}",
            $"{nameof(Bits)}",
            stylesheetsInternal: new HashSet<string>
            {
                "css/Bits/Bits.css", "css/Bits/Bits.{{ThemeVariant}}.css",
            },
            scriptsInternal: new HashSet<string>
            {
                "js/Dropdowns.js",
                "js/Tooltips.js",
            }
        );

        public static class Overrides
        {
            public static readonly ResourceSet MiniBar = new
            ($"{nameof(Integrant4)}.{nameof(Element)}",
                $"{nameof(Overrides)}.{nameof(MiniBar)}",
                dependencies: new[]
                {
                    Resources.Resources.Libraries.MiniBar,
                },
                stylesheetsInternal: new HashSet<string> {"css/Overrides/MiniBar.css"}
            );

            public static readonly ResourceSet Tippy = new
            ($"{nameof(Integrant4)}.{nameof(Element)}",
                $"{nameof(Overrides)}.{nameof(Tippy)}",
                dependencies: new[]
                {
                    Resources.Resources.Libraries.Tippy,
                },
                stylesheetsInternal: new HashSet<string> {"css/Overrides/Tippy.css"}
            );
        }
    }
}