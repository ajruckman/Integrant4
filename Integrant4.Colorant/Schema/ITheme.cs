using System.Collections.Generic;

namespace Integrant4.Colorant.Schema
{
    public interface ITheme
    {
        public string              Assembly       { get; }
        public string              Name           { get; }
        public IEnumerable<string> Variants       { get; }
    }
}