using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public class Filler : IBit
    {
        private readonly Callbacks.IsVisible? _isVisible;

        public Filler(Callbacks.IsVisible? isVisible = null)
        {
            _isVisible = isVisible;
        }

        public RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;
                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Bit I4E-Bit-Filler");
                if (_isVisible != null)
                    builder.AddAttribute(++seq, "hidden", _isVisible.Invoke() == false);
                builder.CloseElement();
            }

            return Fragment;
        }
    }
}