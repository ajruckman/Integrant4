using Integrant4.Colorant.Themes.Solids;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Bits
{
    public class Asterisk : IBit
    {
        private readonly ushort _size;

        public Asterisk(ushort size = 10)
        {
            _size = size;
        }

        public RenderFragment Renderer() => builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "span");
            builder.AddAttribute(++seq, "class", "I4E-Bit-Asterisk");
            builder.OpenComponent<BootstrapIcon>(++seq);
            builder.AddAttribute(++seq, "ID", "asterisk");
            builder.AddAttribute(++seq, "Color", Constants.Red_5);
            builder.AddAttribute(++seq, "Size", _size);
            builder.CloseComponent();
            builder.CloseElement();
        };
    }
}