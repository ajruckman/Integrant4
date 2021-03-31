using System;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element.Constructs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public class ComboboxInput<TValue> : IRefreshableInput<TValue> where TValue : IEquatable<TValue>
    {
        private readonly Combobox<TValue> _combobox;

        public ComboboxInput
        (
            IJSRuntime                    jsRuntime,
            Combobox<TValue>.OptionGetter optionGetter,
            bool                          filterable            = false,
            string?                       uncachedText          = null,
            string?                       noOptionText          = null,
            string?                       filterPlaceholderText = null
        )
        {
            _combobox = new Combobox<TValue>
            (
                jsRuntime,
                optionGetter,
                filterable,
                uncachedText,
                noOptionText,
                filterPlaceholderText
            );

            _combobox.OnChange += OnChange;
        }

        public Task           Refresh()               => _combobox.Refresh();
        public RenderFragment Renderer()              => _combobox.Renderer();
        public Task<TValue?>  GetValue()              => _combobox.GetValue();
        public Task           SetValue(TValue? value) => _combobox.SetValue(value);

        public event Action<TValue?>? OnChange;
    }
}