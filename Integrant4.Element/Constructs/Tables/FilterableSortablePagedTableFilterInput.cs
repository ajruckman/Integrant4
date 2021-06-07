using System;
using Integrant4.Colorant.Themes.Default;
using Integrant4.Element.Inputs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using Integrant4.Fundament;

namespace Integrant4.Element.Constructs.Tables
{
    public class FilterableSortablePagedTableFilterInput<TRow> : ComponentBase where TRow : class
    {
        private TextInput          _input     = null!;
        private Debouncer<string?> _debouncer = null!;
        private Exception?         _error;

        [Parameter] public IFilterableSortablePagedTable<TRow> Table          { get; set; } = null!;
        [Parameter] public string                              ID             { get; set; } = null!;
        [Parameter] public string                              HighlightColor { get; set; } = Constants.Accent_7;

        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;

        protected override void OnInitialized()
        {
            if (string.IsNullOrEmpty(ID))
                throw new Exception("ID was not passed to FilterableSortablePagedTableFilterInput component.");

            _input = new TextInput(JSRuntime, Table.GetFilter(ID), new TextInput.Spec
            {
                IsClearable    = Always.True,
                HighlightColor = () => Table.GetFilter(ID) == null ? "" : HighlightColor,
            });

            _debouncer = new Debouncer<string?>(null, v =>
            {
                if (string.IsNullOrEmpty(v))
                    Table.ClearFilter(ID, false);
                else
                    Table.SetFilter(ID, v, false);

                _input.Refresh();
            }, /*Table.GetFilter(ID),*/ 250, e =>
            {
                _error = e;
                InvokeAsync(StateHasChanged);
            });

            _input.OnChange += v => _debouncer.Reset(v);

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

        protected override void OnAfterRender(bool firstRender)
        {
            if (_error != null) throw _error;
        }
    }
}