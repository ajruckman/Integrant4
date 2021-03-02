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
                scriptsInternal: new HashSet<string> {"Inputs.js"}
            );
        }

        public static readonly ResourceSet Overrides = new
        (
            $"{nameof(Integrant4)}.{nameof(Element)}",
            $"{nameof(Overrides)}",
            stylesheetsInternal: new HashSet<string> {"css/Overrides.css", "css/Overrides.{{ThemeVariant}}.css"}
        );
    }
}