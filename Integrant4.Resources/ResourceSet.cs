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
                        "Fonts/Inter/Inter.css",
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
                        "Fonts/JetBrainsMono/JetBrainsMono.css",
                    }
                );
            }
        }

        public static class Icons
        {
            public static readonly ResourceSet Bootstrap = new(
                $"{nameof(Integrant4)}.{nameof(Resources)}",
                $"{nameof(Icons)}.{nameof(Bootstrap)}",
                stylesheetsInternal: new HashSet<string>
                {
                    "Icons/Icons.css",
                    "Icons/Bootstrap/bootstrap-icons.css",
                }
            );
        }

        public static class Libraries
        {
            public static readonly ResourceSet Dropzone = new
            (
                $"{nameof(Integrant4)}.{nameof(Resources)}",
                $"{nameof(Libraries)}.{nameof(Dropzone)}",
                stylesheetsInternal: new HashSet<string>
                {
                    // "Libraries/Dropzone/basic.min.css",
                    "Libraries/Dropzone/dropzone.min.css",
                },
                scriptsInternal: new HashSet<string>
                {
                    "Libraries/Dropzone/dropzone.min.js",
                }
            );

            public static readonly ResourceSet MiniBar = new
            (
                $"{nameof(Integrant4)}.{nameof(Resources)}",
                $"{nameof(Libraries)}.{nameof(MiniBar)}",
                stylesheetsInternal: new HashSet<string>
                {
                    "Libraries/MiniBar/minibar.min.css",
                },
                scriptsInternal: new HashSet<string>
                {
                    "Libraries/MiniBar/minibar.min.js",
                }
            );

            public static readonly ResourceSet SimpleBar = new
            (
                $"{nameof(Integrant4)}.{nameof(Resources)}",
                $"{nameof(Libraries)}.{nameof(SimpleBar)}",
                stylesheetsInternal: new HashSet<string>
                {
                    "Libraries/SimpleBar/simplebar.min.css",
                },
                scriptsInternal: new HashSet<string>
                {
                    "Libraries/SimpleBar/simplebar.min.js",
                }
            );

            public static readonly ResourceSet OverlayScrollbars = new
            (
                $"{nameof(Integrant4)}.{nameof(Resources)}",
                $"{nameof(Libraries)}.{nameof(OverlayScrollbars)}",
                stylesheetsInternal: new HashSet<string>
                {
                    "Libraries/OverlayScrollbars/OverlayScrollbars.min.css",
                },
                scriptsInternal: new HashSet<string>
                {
                    "Libraries/OverlayScrollbars/OverlayScrollbars.min.js",
                }
            );

            public static readonly ResourceSet Popper = new
            (
                $"{nameof(Integrant4)}.{nameof(Resources)}",
                $"{nameof(Libraries)}.{nameof(Popper)}",
                scriptsInternal: new HashSet<string>
                {
                    "Libraries/Popper/popper.min.js",
                }
            );

            public static readonly ResourceSet Tippy = new
            (
                $"{nameof(Integrant4)}.{nameof(Resources)}",
                $"{nameof(Libraries)}.{nameof(Tippy)}",
                scriptsInternal: new HashSet<string>
                {
                    "Libraries/Tippy/tippy-bundle.umd.min.js",
                },
                dependencies: new[]
                {
                    Popper,
                }
            );

            public static readonly ResourceSet TUIEditor = new
            (
                $"{nameof(Integrant4)}.{nameof(Resources)}",
                $"{nameof(Libraries)}.{nameof(TUIEditor)}",
                stylesheetsInternal: new HashSet<string>
                {
                    "Libraries/TUIEditor/toastui-editor.min.css",
                    "Libraries/TUIEditor/toastui-editor-dark.min.css",
                },
                scriptsInternal: new HashSet<string>
                {
                    "Libraries/TUIEditor/toastui-editor-all.min.js",
                }
            );
        }
    }
}