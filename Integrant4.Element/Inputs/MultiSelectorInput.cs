using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element.Constructs.Selectors;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Inputs
{
    public class MultiSelectorInput<TValue> : IRefreshableInput<TValue?[]?>
    {
        private readonly MultiSelector<TValue> _multiSelector;

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        public MultiSelectorInput(MultiSelector<TValue> multiSelector)
        {
            _multiSelector = multiSelector;

            _multiSelector.OnChange += v =>
            {
                if (v == null)
                {
                    OnChange?.Invoke(null);
                    return;
                }

                OnChange?.Invoke(v.Select(x => x.Value).ToArray());
            };
        }

        public void             Refresh()                  => _multiSelector.Refresh();
        public Task<TValue?[]?> GetValue()                 => _multiSelector.GetValue();
        public Task             SetValue(TValue?[]? value) => _multiSelector.SetValue(value);
        public RenderFragment   Renderer()                 => _multiSelector.Renderer();

        public event Action<TValue?[]?>? OnChange;

        public void BeginLoadingOptions(Action? then = null) =>
            _multiSelector.BeginLoadingOptions(then);
    }
}