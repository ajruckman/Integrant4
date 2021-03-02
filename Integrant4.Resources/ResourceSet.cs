using System.Collections.Generic;
using Integrant4.Fundament;

namespace Integrant4.Resources
{
    public static class Resources
    {
        public static class Fonts
        {
            public static class SansSerif
            {
                public static readonly ResourceSet Inter = new
                (
                    $"{nameof(Integrant4)}.{nameof(Resources)}",
                    $"{nameof(Fonts)}.{nameof(SansSerif)}.{nameof(Inter)}",
                    stylesheetsInternal: new HashSet<string>
                    {
                        "Fonts/Inter/Inter.css"
                    }
                );
            }

            public static class Monospace
            {
                public static readonly ResourceSet JetBrainsMono = new
                (
                    $"{nameof(Integrant4)}.{nameof(Resources)}",
                    $"{nameof(Fonts)}.{nameof(Monospace)}.{nameof(JetBrainsMono)}",
                    stylesheetsInternal: new HashSet<string>
                    {
                        "Fonts/JetBrainsMono/JetBrainsMono.css"
                    }
                );
            }
        }

        public static class Icons
        {
            public static readonly ResourceSet Bootstrap = new ResourceSet
            (
                $"{nameof(Integrant4)}.{nameof(Resources)}",
                $"{nameof(Icons)}.{nameof(Bootstrap)}",
                stylesheetsInternal: new HashSet<string>
                {
                    "Icons/Icons.css",
                    "Icons/Bootstrap/bootstrap-icons.css"
                }
            );
        }
    }
}