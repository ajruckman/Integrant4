using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Resources.Icons
{
    public class BootstrapIcon : ComponentBase, IIcon
    {
        public BootstrapIcon()
        {
        }

        public BootstrapIcon(string id, ushort size = 16, string? color = null)
        {
            ID  = id;
            Size  = size;
            Color = color;
        }

        [Parameter] public string  ID  { get; set; } = null!;
        [Parameter] public ushort  Size  { get; set; } = 32;
        [Parameter] public string? Color { get; set; }

        public RenderFragment Renderer() => BuildRenderTree;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(1, "class", "I4R-BootstrapIcon");
            if (Color != null) builder.AddAttribute(2, "style", $"color: {Color}");
            builder.OpenElement(3, "svg");
            builder.AddAttribute(4, "class", "bi");
            builder.AddAttribute(5, "width", Size);
            builder.AddAttribute(6, "height", Size);
            builder.AddAttribute(7, "fill", "currentColor");
            builder.OpenElement(8, "use");
            builder.AddAttribute(9, "href",
                $"_content/Integrant4.Resources/Icons/Bootstrap/bootstrap-icons.svg#{ID}");
            builder.CloseElement();
            builder.CloseElement();
            builder.CloseElement();
        }
    }
}