using Integrant4.API;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Constructs
{
    public class StackedContent : IConstruct
    {
        private readonly Callbacks.BitContents _contents;
        private readonly Callbacks.FlexAlign   _align;

        public StackedContent(Callbacks.BitContents contents, Callbacks.FlexAlign? align = null)
        {
            _contents = contents;
            _align    = align ?? (() => FlexAlign.Center);
        }

        public RenderFragment Renderer() => builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct-StackedContent");
            builder.AddAttribute(++seq, "style", $"align-items: {_align.Invoke().Serialize()}");

            foreach (IRenderable content in _contents.Invoke())
            {
                builder.OpenElement(++seq, "span");
                builder.AddContent(++seq, content.Renderer());
                builder.CloseElement();
            }

            builder.CloseElement();
        };
    }
}