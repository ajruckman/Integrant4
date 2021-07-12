using System.Collections.Generic;
namespace Integrant4.Colorant.Themes.Main
{
    public class Theme : ITheme
    {
        public string Assembly { get; } = "Integrant4.Colorant";
        public string Name { get; } = "Main";
        public IEnumerable<string> Variants { get; } = new [] { "Dark", "White", "Matrix", "Pink" };
    }
}
