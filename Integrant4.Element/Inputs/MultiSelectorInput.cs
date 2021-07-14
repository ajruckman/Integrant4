using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element.Constructs.Selectors;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Inputs
{
    public class MultiSelectorInput<TValue> : IWritableRefreshableInput<TValue?[]?>
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

        public void             Refresh()   => _multiSelector.Refresh();
        public TValue?[]?       GetValue()  => _multiSelector.GetValue();
        public Task<TValue?[]?> ReadValue() => Task.FromResult(GetValue());

        public Task SetValue(TValue?[]? value, bool invokeOnChange = true)
        {
            _multiSelector.SetValue(value, invokeOnChange);
            return Task.CompletedTask;
        }

        public RenderFragment Renderer() => _multiSelector.Renderer();

        public event Action<TValue?[]?>? OnChange;

        public void BeginLoadingOptions(Action? then = null) =>
            _multiSelector.BeginLoadingOptions(then);
    }
}