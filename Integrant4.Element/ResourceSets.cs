using System.Collections.Generic;
using Integrant4.Fundament;

namespace Integrant4.Element
{
    public static class ResourceSets
    {
        public static readonly ResourceSet I4App = new
        (
            $"{nameof(Integrant4)}.{nameof(Element)}",
            nameof(I4App),
            stylesheetsInternal: new HashSet<string>
            {
                "css/I4App.css",
            }
        );

        public static readonly ResourceSet Constructs = new
        (
            $"{nameof(Integrant4)}.{nameof(Element)}",
            nameof(Constructs),
            dependencies: new[]
            {
                Resources.Resources.Icons.Bootstrap,
                Resources.Resources.Libraries.Dropzone,
                // Resources.Resources.Libraries.MiniBar,
                // Resources.Resources.Libraries.SimpleBar,
                Resources.Resources.Libraries.OverlayScrollbars,
                Resources.Resources.Libraries.TUIEditor,
            },
            stylesheetsInternal: new HashSet<string>
            {
                "css/Constructs/Constructs.css",
            },
            scriptsInternal: new HashSet<string>
            {
                "js/Constructs.js",
                "js/MarkdownEditor.js",
            }
        );

        public static readonly ResourceSet Bits = new
        (
            $"{nameof(Integrant4)}.{nameof(Element)}",
            nameof(Bits),
            dependencies: new[]
            {
                Resources.Resources.Libraries.Tippy,
            },
            stylesheetsInternal: new HashSet<string> {"css/Bits/Bits.css"},
            scriptsInternal: new HashSet<string> {"js/Elements.js"}
        );

        public static readonly ResourceSet Inputs = new
        (
            $"{nameof(Integrant4)}.{nameof(Element)}",
            nameof(Inputs),
            stylesheetsInternal: new HashSet<string> {"css/Inputs/Inputs.css"},
            scriptsInternal: new HashSet<string> {"js/Elements.js", "js/Inputs.js"}
        );

        public static readonly ResourceSet Layouts = new ResourceSet
        (
            $"{nameof(Integrant4)}.{nameof(Element)}",
            nameof(Layouts),
            stylesheetsInternal: new HashSet<string> {"css/Layouts/Layouts.css"}
        );

        public static class Overrides
        {
            public static readonly ResourceSet Blazor = new
            ($"{nameof(Integrant4)}.{nameof(Element)}",
                $"{nameof(Overrides)}.{nameof(Blazor)}",
                dependencies: new[]
                {
                    Resources.Resources.Libraries.MiniBar,
                },
                stylesheetsInternal: new HashSet<string> {"css/Overrides/Blazor.css"}
            );

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

            public static readonly ResourceSet VariantLoader = new
            ($"{nameof(Integrant4)}.{nameof(Element)}",
                $"{nameof(Overrides)}.{nameof(VariantLoader)}",
                stylesheetsInternal: new HashSet<string> {"css/Overrides/VariantLoader.css"}
            );
        }
    }
}