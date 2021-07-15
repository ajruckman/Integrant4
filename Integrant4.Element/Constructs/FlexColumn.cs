using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Constructs
{
    public class FlexColumn : IConstruct
    {
        private readonly DynamicContent         _content;
        private readonly Callbacks.FlexAlign?   _align;
        private readonly Callbacks.FlexJustify? _justify;

        public FlexColumn
        (
            DynamicContent content,
            Callbacks.FlexAlign?   align   = null,
            Callbacks.FlexJustify? justify = null
        )
        {
            _content = content;
            _align   = align;
            _justify = justify;
        }

        public RenderFragment Renderer() => builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct-FlexColumn");

            string style = "";
            if (_align != null)
                style += $"align-items: {_align.Invoke().Serialize()};";
            if (_justify != null)
                style += $"justify-content: {_justify.Invoke().Serialize()};";

            builder.AddAttribute(++seq, "style", style.Length != 0 ? style : null);

            foreach (IRenderable content in _content.GetAll())
            {
                builder.OpenElement(++seq, "span");
                builder.AddContent(++seq, content.Renderer());
                builder.CloseElement();
            }

            builder.CloseElement();
        };
    }
}