using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Constructs.Tables
{
    public class PagedTableNoValuesMessage : ComponentBase
    {
        [Parameter] public string Message { get; set; } = "No results";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "tr");
            builder.AddAttribute(1, "class", "I4E-Construct-PagedTable-NoValuesMessage");

            builder.OpenElement(2, "td");
            builder.AddAttribute(3, "colspan", "100%");
            builder.AddContent(4, Message);
            builder.CloseElement();

            builder.CloseElement();
        }
    }
}