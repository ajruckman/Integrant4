using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Constructs
{
    public class LinearContent : IConstruct
    {
        private readonly DynamicContents       _contents;
        private readonly Callbacks.FlexJustify _justify;

        public LinearContent(DynamicContents contents, Callbacks.FlexJustify? justify = null)
        {
            _contents = contents;
            _justify  = justify ?? (() => FlexJustify.Start);
        }

        public RenderFragment Renderer() => builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct-LinearContent");
            builder.AddAttribute(++seq, "style", $"justify-content: {_justify.Invoke().Serialize()}");

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