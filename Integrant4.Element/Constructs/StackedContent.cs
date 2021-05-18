using Integrant4.API;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Constructs
{
    public class StackedContent : IConstruct
    {
        public enum Align
        {
            Start,
            Center,
            End,
        }

        private readonly Callbacks.BitContents     _contents;
        private readonly Callbacks.Callback<Align> _align;

        public StackedContent(Callbacks.BitContents contents, Callbacks.Callback<Align>? align = null)
        {
            _contents = contents;
            _align    = align ?? (() => Align.Center);
        }

        public RenderFragment Renderer() => builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class",
                "I4E-Construct-StackedContent "        +
                "I4E-Construct-StackedContent--Align-" + _align.Invoke());

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