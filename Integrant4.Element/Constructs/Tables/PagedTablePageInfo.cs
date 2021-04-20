using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Constructs.Tables
{
    public class PagedTablePageInfo<TRow> : ComponentBase where TRow : class
    {
        [Parameter] public IPagedTable<TRow> Table { get; set; } = null!;

        protected override void OnParametersSet()
        {
            Table.OnPaginate.Event   += () => InvokeAsync(StateHasChanged);
            Table.OnInvalidate.Event += () => InvokeAsync(StateHasChanged);
            Table.OnRefresh.Event    += () => InvokeAsync(StateHasChanged);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            int current = Table.CurrentPage * Table.NumPages + 1;
            int rows    = Table.BaseTable.Rows().Length;

            if (rows == 0) current = 0;

            int max = Math.Min(Table.CurrentPage * Table.NumPages + Table.PageSize, rows);

            string info =
                $"Showing {current} to {max} of {rows:#,##0} | " +
                $"{Table.NumPages} page{(Table.NumPages != 1 ? "s" : "")}";

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "I4E-Construct-PagedTable-PageInfo");
            builder.AddContent(2, info);
            builder.CloseElement();
        }
    }
}