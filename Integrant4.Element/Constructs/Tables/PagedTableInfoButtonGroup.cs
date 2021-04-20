using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Constructs.Tables
{
    public class PagedTableInfoButtonGroup<TRow> : ComponentBase where TRow : class
    {
        [Parameter] public IPagedTable<TRow> Table { get; set; } = null!;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "I4E-Layout-ListTable-TableControlsContainer");

            builder.OpenComponent<PagedTablePageInfo<TRow>>(2);
            builder.AddAttribute(3, "Table", Table);
            builder.CloseComponent();

            builder.OpenComponent<PagedTableButtons<TRow>>(5);
            builder.AddAttribute(6, "Table", Table);
            builder.CloseComponent();

            builder.CloseElement();
        }
    }
}