using System.Collections.Generic;
using Integrant4.Colorant.Schema;
namespace Integrant4.Colorant.Themes.Solids
{
    public class Theme : ITheme
    {
        public string Assembly { get; } = "Integrant4.Colorant";
        public string Name { get; } = "Solids";
        public IEnumerable<string> Variants { get; } = new [] { "Normal" };
    }
}
