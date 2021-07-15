using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits.BitComponents
{
    public class TextBlockComponent : ComponentBase
    {
        private TextBlock? _textBlock;

        [Parameter] public double?     FontSize   { get; set; }
        [Parameter] public FontWeight? FontWeight { get; set; }
        [Parameter] public TextAlign?  TextAlign  { get; set; }
        [Parameter] public Display?    Display    { get; set; }
        [Parameter] public Tooltip?    Tooltip    { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; } = null!;

        protected override void OnParametersSet()
        {
            TextBlock.Spec spec = new()
            {
                FontSize   = FontSize   != null ? () => FontSize.Value : null,
                FontWeight = FontWeight != null ? () => FontWeight.Value : null,
                TextAlign  = TextAlign  != null ? () => TextAlign.Value : null,
                Display    = Display    != null ? () => Display.Value : null,
                Tooltip    = Tooltip    != null ? () => Tooltip.Value : null,
            };

            _textBlock = new TextBlock(ChildContent, spec);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(0, _textBlock?.Renderer());
        }
    }
}