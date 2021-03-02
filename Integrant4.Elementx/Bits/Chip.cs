using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    public class Chip
    {
        private readonly Spec _spec;

        public Chip(Spec spec)
        {
            _spec = spec;
        }

        public RenderFragment Render()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;
                builder.OpenElement(++seq, "span");
                builder.AddAttribute(++seq, "width", _spec.PixelsWidth?.Invoke() ?? 5);
                builder.AddAttribute(++seq, "width", _spec.PixelsWidth?.Call() ?? 5);
                builder.AddContent(++seq, _spec.Content.Invoke());
                builder.CloseElement();
            }

            return Fragment;
        }

        public class Spec
        {
            public Spec(Callbacks.Content content)
            {
                Content = content;
            }

            public Callbacks.Content Content { get; }

            public Callbacks.PxWidth? PixelsWidth { get; init; }
        }
    }
}