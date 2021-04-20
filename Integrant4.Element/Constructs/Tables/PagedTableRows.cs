using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Constructs.Tables
{
    public class PagedTableRows<TRow> : ComponentBase where TRow : class
    {
        [Parameter] public IPagedTable<TRow> Table        { get; set; } = null!;
        [Parameter] public RenderFragment    ChildContent { get; set; } = null!;

        protected override void OnParametersSet()
        {
            Table.OnPaginate.Event   += () => InvokeAsync(StateHasChanged);
            Table.OnInvalidate.Event += () => InvokeAsync(StateHasChanged);
            Table.OnRefresh.Event    += () => InvokeAsync(StateHasChanged);

            // if (Table is IFilterableSortablePagedTable<TRow> filterable)
            // {
            //     filterable.OnFilter.Event += () => InvokeAsync(StateHasChanged);
            // }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddContent(0, ChildContent);
    }
}