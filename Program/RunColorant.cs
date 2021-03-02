using System.IO;
using Integrant4.Colorant;
using Integrant4.Colorant.Schema;
using Newtonsoft.Json;

namespace Program
{
    public static class RunColorant
    {
        public static void Run()
        {
            Directory.SetCurrentDirectory("../Integrant4.Colorant/");

            var generator = new Generator();

            var themes = new string[] {"Default", "Solids"};

            Directory.Delete("Themes/", true);
            Directory.CreateDirectory("Themes/");
            
            foreach (string theme in themes)
            {
                Directory.CreateDirectory($"Themes/{theme}/");
                
                var t = JsonConvert.DeserializeObject<ThemeDefinition>
                (
                    File.ReadAllText($"Definitions/{theme}.json")
                );

                generator.Generate(t);

                string s = JsonConvert.SerializeObject(t, Formatting.Indented);
         
                File.WriteAllText($"Themes/{theme}/Compiled.json", s);

                //

                Writer.Write(t, "Integrant4.Colorant");
            }
        }
    }
}