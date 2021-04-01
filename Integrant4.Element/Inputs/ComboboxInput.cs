using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element.Constructs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public class ComboboxInput<TValue> : IRefreshableInput<TValue>
    {
        private readonly Combobox<TValue> _combobox;

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        public ComboboxInput
        (
            IJSRuntime                    jsRuntime,
            Combobox<TValue>.OptionGetter optionGetter,
            Spec?                         spec = null
        )
        {
            _combobox = new Combobox<TValue>
            (
                jsRuntime,
                optionGetter,
                spec
            );

            _combobox.OnChange += OnChange;
        }

        public Task           Refresh()               => _combobox.Refresh();
        public RenderFragment Renderer()              => _combobox.Renderer();
        public Task<TValue?>  GetValue()              => _combobox.GetValue();
        public Task           SetValue(TValue? value) => _combobox.SetValue(value);

        public event Action<TValue?>? OnChange;

        public void LoadOptions() => _combobox.LoadOptions();

        public class Spec : Combobox<TValue>.Spec {}
    }
}