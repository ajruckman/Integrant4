using System.Collections.Generic;
using Integrant4.Fundament;

namespace Integrant4.Colorant
{
    public static class ResourceSets
    {
        public static readonly ResourceSet ThemeVariantSupport = new
        (
            $"{nameof(Integrant4)}.{nameof(Colorant)}",
            nameof(ThemeVariantSupport),
            scriptsInternal: new HashSet<string>
            {
                "js/Theme.js",
            }
        );
    }
}