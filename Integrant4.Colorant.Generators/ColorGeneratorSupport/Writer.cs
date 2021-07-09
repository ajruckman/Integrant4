using System.Collections.Generic;
using System.IO;
using System.Linq;
using Integrant4.Colorant.Generators.Schema;

namespace Integrant4.Colorant.Generators.ColorGeneratorSupport
{
    public static class Writer
    {
        private const string I = "    ";
        private const string C = I + I + "public const string ";

        public static void Write(ThemeDefinition theme, string assembly)
        {
            var constLines = new List<string>();

            foreach (Block block in theme.Blocks)
            {
                foreach (string id in block.IDs)
                {
                    string n = block.Name.Replace("-", "_");
                    
                    constLines.Add(
                        C + $"{n}_{id} = \"var(--I4C-{theme.Name}-{block.Name}-{id})\";");

                    if (!block.CreateDisplayTextVariables) continue;

                    constLines.Add(
                        C + $"{n}_{id}_Text = \"var(--I4C-{theme.Name}-{block.Name}-{id}-Text)\";");
                }
            }

            File.WriteAllLines($"../{assembly}/Themes/{theme.Name}/Constants.cs", new List<string>
            {
                $"namespace {assembly}.Themes.{theme.Name}",
                "{",
                I + "public static class Constants",
                I + "{",
                string.Join('\n', constLines),
                I + "}",
                "}",
            });

            //

            var variantLines = new List<string>();

            foreach (Variant variant in theme.Variants)
            {
                variantLines.Add(I + I + $"{variant.Name},");
            }

            File.WriteAllLines($"../{assembly}/Themes/{theme.Name}/Variants.cs", new List<string>
            {
                $"namespace {assembly}.Themes.{theme.Name}",
                "{",
                I + "public enum Variants",
                I + "{",
                I + I + "Undefined,",
                string.Join('\n', variantLines),
                I + "}",
                "}",
            });

            //

            string variants = "\"" + string.Join("\", \"", theme.Variants.Select(v => v.Name)) + "\"";

            File.WriteAllLines($"../{assembly}/Themes/{theme.Name}/Theme.cs", new List<string>
            {
                "using System.Collections.Generic;",
                $"namespace {assembly}.Themes.{theme.Name}",
                "{",
                I + "public class Theme : ITheme",
                I + "{",
                I + I + $"public string Assembly {{ get; }} = \"{assembly}\";",
                I + I + $"public string Name {{ get; }} = \"{theme.Name}\";",
                I + I + $"public IEnumerable<string> Variants {{ get; }} = new [] {{ {variants} }};",
                I + "}",
                "}",
            });

            //

            foreach (Variant variant in theme.Variants)
            {
                if (variant.Colors == null) continue;

                var cssLines = new List<string>();
                cssLines.Add(":root {");

                foreach (Block block in theme.Blocks)
                {
                    if (!variant.Colors.ContainsKey(block.Name)) continue;

                    foreach (var (id, color) in variant.Colors[block.Name])
                    {
                        cssLines.Add(
                            $"\t--I4C-{theme.Name}-{block.Name}-{id}: {color};");
                    }
                }

                cssLines.Add("}");

                Directory.CreateDirectory($"../{assembly}/wwwroot/css/{theme.Name}");

                File.WriteAllLines($"../{assembly}/wwwroot/css/{theme.Name}/{variant.Name}.css", cssLines);
            }
        }
    }
}