using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Constructs
{
    public class FlexRow : IConstruct
    {
        private readonly ContentRef             _content;
        private readonly Callbacks.FlexJustify? _justify;
        private readonly Callbacks.FlexAlign?   _align;

        public FlexRow
        (
            ContentRef             content,
            Callbacks.FlexJustify? justify = null,
            Callbacks.FlexAlign?   align   = null
        )
        {
            _content = content;
            _justify = justify;
            _align   = align;
        }

        public RenderFragment Renderer() => builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct-FlexRow");

            string style = "";
            if (_justify != null)
                style += $"justify-content: {_justify.Invoke().Serialize()};";
            if (_align != null)
                style += $"align-items: {_align.Invoke().Serialize()};";

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