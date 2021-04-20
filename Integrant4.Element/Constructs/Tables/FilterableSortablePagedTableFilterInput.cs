using System;
using Integrant4.Element.Inputs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Integrant4.Element.Constructs.Tables
{
    public class FilterableSortablePagedTableFilterInput<TRow> : ComponentBase where TRow : class
    {
        private TextInput _input = null!;

        [Parameter] public IFilterableSortablePagedTable<TRow> Table { get; set; } = null!;
        [Parameter] public string                              ID    { get; set; } = null!;

        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;

        protected override void OnInitialized()
        {
            if (string.IsNullOrEmpty(ID))
                throw new Exception("ID was not passed to FilterableSortablePagedTableFilterInput component.");

            _input = new TextInput(JSRuntime, Table.GetFilter(ID));

            _input.OnChange += v =>
            {
                if (string.IsNullOrEmpty(v))
                    Table.ClearFilter(ID, false);
                else
                    Table.SetFilter(ID, v, false);
            };

            Table.OnFilterChange += async (key, value) =>
            {
                if (key == ID) await _input.SetValue(value);
            };
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "I4E-Construct-PagedTable-FilterInput");
            builder.AddContent(2, _input.Renderer());
            builder.CloseElement();
        }
    }
}