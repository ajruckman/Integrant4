using System.Collections.Generic;
using Integrant4.Colorant.Schema;
namespace Integrant4.Colorant.Themes.Default
{
    public class Theme : ITheme
    {
        public string Assembly { get; } = "Integrant4.Colorant";
        public string Name { get; } = "Default";
        public IEnumerable<string> Variants { get; } = new [] { "Dark", "White", "Matrix", "Pink" };
    }
}
