using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Constructs.Tables
{
    public class SortablePagedTableSortButton<TRow> : ComponentBase where TRow : class
    {
        [Parameter] public ISortablePagedTable<TRow> Table { get; set; } = null!;
        [Parameter] public string                    ID    { get; set; } = null!;

        protected override void OnParametersSet()
        {
            // Table.OnSort.Event    += () => InvokeAsync(StateHasChanged);
            Table.OnInvalidate.Event += () => InvokeAsync(StateHasChanged);
            Table.OnRefresh.Event    += () => InvokeAsync(StateHasChanged);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "button");
            builder.AddAttribute(1, "class", "I4E-Construct-PagedTable-SortIndicator " + (
                Table.ActiveSorter == ID
                    ? "I4E-Construct-PagedTable-SortIndicator--Active"
                    : "I4E-Construct-PagedTable-SortIndicator--Inactive"
            ));

            builder.AddAttribute(2, "onclick",
                EventCallback.Factory.Create(this, () => Table.NextSortDirection(ID)));

            builder.OpenComponent<BootstrapIcon>(3);
            builder.AddAttribute(4, "Size", (ushort)16);

            if (Table.ActiveSorter != ID)
            {
                builder.AddAttribute(5, "ID", "arrow-down-up");
            }
            else if (Table.ActiveSortDirection == TableSortDirection.Ascending)
            {
                builder.AddAttribute(5, "ID", "arrow-bar-up");
            }
            else if (Table.ActiveSortDirection == TableSortDirection.Descending)
            {
                builder.AddAttribute(5, "ID", "arrow-bar-down");
            }

            builder.CloseComponent();

            builder.CloseElement();
        }
    }
}