using System.Collections.Generic;
using Integrant4.Element.Bits;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Constructs.Tables
{
    public class PagedTableButtons<TRow> : ComponentBase where TRow : class
    {
        private Button _previous = null!;
        private Button _next     = null!;

        [Parameter] public IPagedTable<TRow> Table { get; set; } = null!;

        protected override void OnParametersSet()
        {
            Table.OnPaginate.Event   += () => InvokeAsync(StateHasChanged);
            Table.OnInvalidate.Event += () => InvokeAsync(StateHasChanged);
            Table.OnRefresh.Event    += () => InvokeAsync(StateHasChanged);

            // if (Table is IFilterableSortablePagedTable<TRow> filterable)
            // {
            //     filterable.OnFilter.Event += () => InvokeAsync(StateHasChanged);
            // }

            _previous = new Button("PREVIOUS".AsContent(), new Button.Spec
            {
                IsDisabled = () => !Table.CanPrevious(),
                OnClick    = (_, _) => Table.Previous(),
            });

            _next = new Button("NEXT".AsContent(), new Button.Spec
            {
                IsDisabled = () => !Table.CanNext(),
                OnClick    = (_, _) => Table.Next(),
            });
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            IEnumerable<int> pages = Paginator.Pages(Table.NumPages, Table.CurrentPage);

            //

            int seq = -1;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct-PagedTable-PagedTableButtons");

            builder.AddContent(++seq, _previous.Renderer());

            foreach (int page in pages)
            {
                if (page != -1)
                {
                    builder.OpenElement(++seq, "button");
                    builder.AddAttribute(++seq, "disabled", Table.CurrentPage == page);
                    builder.AddAttribute(++seq, "onclick", EventCallback.Factory.Create(this, () => Table.Jump(page)));
                    builder.AddContent(++seq, page + 1);
                    builder.CloseElement();
                }
                else
                {
                    builder.OpenElement(++seq, "button");
                    builder.AddAttribute(++seq, "disabled", true);
                    builder.AddAttribute(++seq, "class", "I4E-Construct-PagedTable-Ellipses");
                    builder.AddContent(++seq, "...");
                    builder.CloseElement();
                }
            }

            builder.AddContent(++seq, _next.Renderer());

            builder.CloseElement();
        }
    }
}