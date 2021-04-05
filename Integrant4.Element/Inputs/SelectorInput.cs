using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element.Constructs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public class SelectorInput<TValue> : IRefreshableInput<TValue>
    {
        private readonly Selector<TValue> _selector;

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        public SelectorInput
        (
            IJSRuntime                    jsRuntime,
            Selector<TValue>.OptionGetter optionGetter,
            Spec?                         spec = null
        )
        {
            _selector = new Selector<TValue>
            (
                jsRuntime,
                optionGetter,
                spec
            );

            _selector.OnChange += OnChange;
        }

        public Task           Refresh()               => _selector.Refresh();
        public RenderFragment Renderer()              => _selector.Renderer();
        public Task<TValue?>  GetValue()              => _selector.GetValue();
        public Task           SetValue(TValue? value) => _selector.SetValue(value);

        public event Action<TValue?>? OnChange;

        public void LoadOptions() => _selector.LoadOptions();

        public class Spec : Selector<TValue>.Spec {}
    }
}