using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element.Constructs;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Inputs
{
    public class SelectorInput<TValue> : IRefreshableInput<TValue>
    {
        private readonly Selector<TValue> _selector;

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        public SelectorInput
        (
            Selector<TValue> selector
        )
        {
            _selector = selector;

            _selector.OnChange += v => OnChange?.Invoke(v);
        }

        public void           Refresh()               => _selector.Refresh();
        public RenderFragment Renderer()              => _selector.Renderer();
        public Task<TValue?>  GetValue()              => _selector.GetValue();
        public Task           SetValue(TValue? value) => _selector.SetValue(value);

        public event Action<TValue?>? OnChange;

        public void BeginLoadingOptions(Action? then = null) => _selector.BeginLoadingOptions(then);
    }
}