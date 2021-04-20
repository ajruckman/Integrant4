using Integrant4.Colorant.Themes.Default;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Constructs.Tables
{
    public class FilterableSortablePagedTableFilterSortGroup<TRow> : ComponentBase where TRow : class
    {
        [Parameter] public IFilterableSortablePagedTable<TRow> Table          { get; set; } = null!;
        [Parameter] public string                              ID             { get; set; } = null!;
        [Parameter] public string                              HighlightColor { get; set; } = Constants.Accent_7;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "I4E-Layout-ListTable-TableControlsContainer");

            builder.OpenComponent<FilterableSortablePagedTableFilterInput<TRow>>(2);
            builder.AddAttribute(3, "Table",          Table);
            builder.AddAttribute(4, "ID",             ID);
            builder.AddAttribute(5, "HighlightColor", HighlightColor);
            builder.CloseComponent();

            builder.OpenComponent<SortablePagedTableSortButton<TRow>>(6);
            builder.AddAttribute(7, "Table", Table);
            builder.AddAttribute(8, "ID",    ID);
            builder.CloseComponent();

            builder.CloseElement();
        }
    }
}