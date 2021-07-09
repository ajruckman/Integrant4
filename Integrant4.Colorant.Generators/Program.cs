using System.IO;
using Integrant4.Colorant.Generators.ColorGeneratorSupport;
using Integrant4.Colorant.Generators.Schema;
using Newtonsoft.Json;

namespace Integrant4.Colorant.Generators
{
    internal static class Program
    {
        private const string Target = "Integrant4.Colorant";

        private static void Main()
        {
            var themes = new[] { "Main", "Solids" };

            if (Directory.Exists($"../{Target}/Themes/"))
                Directory.Delete($"../{Target}/Themes/", true);

            Directory.CreateDirectory($"../{Target}/Themes/");

            foreach (string theme in themes)
            {
                Directory.CreateDirectory($"../{Target}/Themes/{theme}/");

                var t = JsonConvert.DeserializeObject<ThemeDefinition>
                (
                    File.ReadAllText($"./Definitions/{theme}.json")
                );

                Generator.Generate(t);

                string s = JsonConvert.SerializeObject(t, Formatting.Indented);

                File.WriteAllText($"../{Target}/Themes/{theme}/Compiled.json", s);

                //

                Writer.Write(t, Target);
            }
        }
    }
}