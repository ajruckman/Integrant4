using Integrant4.API;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Constructs
{
    public partial class Header : IRenderable
    {
        private readonly Callbacks.BitContents _contents;

        public Header(Callbacks.BitContents contents)
        {
            _contents = contents;
        }
    }

    public partial class Header
    {
        public RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct I4E-Construct-Header");

                foreach (IRenderable renderable in _contents.Invoke())
                {
                    builder.OpenElement(++seq, "span");
                    builder.AddAttribute(++seq, "class", "I4E-Construct-Header-Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();
            }

            return Fragment;
        }
    }
}