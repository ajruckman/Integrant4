using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Constructs.Selectors
{
    public partial class MultiSelector<TValue> : IConstruct
    {
        private readonly Selector<TValue>.Spec                    _spec;
        private readonly Selector<TValue>.OptionEqualityComparer? _equalityComparer;
        private readonly Selector<TValue>.OptionGetter                 _optionGetter;

        private readonly Selector<TValue>     _selector;
        private readonly List<Option<TValue>> _selected;

        public MultiSelector
        (
            IJSRuntime                          jsRuntime,
            Selector<TValue>.OptionGetter       optionGetter,
            Selector<TValue>.Spec?              spec             = null,
            Selector<TValue>.OptionEqualityComparer? equalityComparer = null
        )
        {
            _spec             = spec ?? new Selector<TValue>.Spec();
            _equalityComparer = equalityComparer;
            _optionGetter     = optionGetter;

            _selector = new Selector<TValue>(jsRuntime, Array.Empty<Option<TValue>>, _spec, _equalityComparer);
            _selected = new List<Option<TValue>>();

            _selector.OnChange += Select;

            _deselectValueButton = new BootstrapIcon("x-circle-fill", (ushort) (12 * _spec.Scale?.Invoke() ?? 12));
        }
    }

    public partial class MultiSelector<TValue>
    {
        private readonly object _optionsLock = new();
        private readonly object _ctsLock     = new();

        private Option<TValue>[]?        _options;
        private CancellationTokenSource? _cts = new();

        private Option<TValue>[] Options()
        {
            lock (_optionsLock)
            {
                return _options ??= _optionGetter.Invoke();
            }
        }

        private Option<TValue>[] InnerOptions()
        {
            Option<TValue>[] all    = Options();
            Option<TValue>[] result = new Option<TValue> [all.Length];

            for (var i = 0; i < all.Length; i++)
            {
                Option<TValue> v = all[i];

                if (_selected.Contains(all[i]))
                    result[i] = new Option<TValue>
                    (
                        v.Value,
                        v.OptionContent,
                        v.SelectionContent,
                        v.FilterableText,
                        true,
                        v.Placeholder
                    );
                else
                    result[i] = v;
            }

            return result;
        }

        private async Task BeginCache()
        {
            CancellationToken token;

            lock (_ctsLock)
            {
                _cts?.Cancel();
                _cts  = new CancellationTokenSource();
                token = _cts.Token;
            }

            Task<Option<TValue>[]> task = Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();
                return _optionGetter.Invoke();
            }, token);

            try
            {
                Option<TValue>[] result = await task;
                token.ThrowIfCancellationRequested();

                lock (_optionsLock)
                {
                    token.ThrowIfCancellationRequested();
                    _options = result;
                }

                _selector.SetOptions(InnerOptions());
                _refresher?.Invoke();
                OnLoaded?.Invoke();
            }
            catch (OperationCanceledException)
            {
                // ignored
            }
            finally
            {
                lock (_ctsLock)
                {
                    _cts?.Dispose();
                    _cts = null;
                }
            }
        }

        public void BeginLoadingOptions(Action? then = null)
        {
            lock (_optionsLock)
            {
                if (_options == null)
                {
                    Task.Run(BeginCache).ContinueWith(_ => then?.Invoke());
                }
            }
        }

        public void Invalidate()
        {
            lock (_optionsLock)
            {
                _options = null;
            }

            _refresher?.Invoke();
        }
    }

    public partial class MultiSelector<TValue>
    {
        public event Action<IReadOnlyList<Option<TValue>>?>? OnChange;
        public event Action?                                 OnLoaded;

        public void Refresh()
        {
            _refresher?.Invoke();
        }

        public async Task<TValue?[]?> GetValue()
        {
            await Task.CompletedTask;

            return _selected.Count == 0
                ? null
                : _selected.Select(v => v.Value).ToArray();
        }

        public async Task SetValue(TValue?[]? value)
        {
            _selected.Clear();

            if (value == null)
            {
                Update();
                return;
            }

            foreach (Option<TValue> option in Options())
            {
                foreach (TValue? v in value)
                {
                    if (!ValueEquals(option.Value, v)) continue;

                    _selected.Add(option);
                    goto Next;
                }

                throw new InvalidOperationException(
                    "Could not find an option with a value equal to one passed to SetValue.");

                Next: ;
            }

            Update();

            await Task.CompletedTask;
        }

        public void ClearValue()
        {
            if (_spec.IsDisabled?.Invoke() ?? false) return;

            _selected.Clear();
            Update();
        }

        private bool ValueEquals(TValue? left, TValue? right) =>
            _equalityComparer?.Invoke(left, right) ?? left?.Equals(right) == true;
    }

    public partial class MultiSelector<TValue>
    {
        private void Select(Option<TValue>? v)
        {
            if (v == null || v.Value.Value == null) return;

            if (!_selected.Contains(v.Value))
            {
                _selected.Add(v.Value);
            }

            _selector.ClearValue();
            Update();
        }

        private void Deselect(Option<TValue> v)
        {
            if (v.Value == null) return;

            if (!_selected.Remove(v)) return;

            Update();
        }

        private void Update()
        {
            _selector.SetOptions(InnerOptions());
            OnChange?.Invoke(_selected.Count == 0 ? null : _selected);
            _refresher?.Invoke();
        }
    }

    public partial class MultiSelector<TValue>
    {
        private readonly BootstrapIcon _deselectValueButton;

        private WriteOnlyHook? _refresher;

        public RenderFragment Renderer() => Latch.Create(builder =>
        {
            double width = _spec.Width?.Invoke() ?? 300;

            int seq = -1;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct-MultiSelector");
            builder.AddAttribute(++seq, "style", $"max-width: {width}px");

            builder.AddContent(++seq, _selector.Renderer());

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct-MultiSelector-Selections");

            foreach (Option<TValue> selection in _selected)
            {
                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-MultiSelector-Selection");

                builder.AddContent(++seq, selection.SelectionContent?.Renderer() ??
                                          selection.OptionContent.Renderer());

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-MultiSelector-DeselectButtonWrapper");
                builder.AddAttribute(++seq, "tabindex", 0);
                builder.AddAttribute(++seq, "onclick",
                    EventCallback.Factory.Create(this, () => Deselect(selection)));
                builder.AddContent(++seq, _deselectValueButton.Renderer());
                builder.CloseElement();

                builder.CloseElement();
            }

            builder.CloseElement();

            builder.CloseElement();
        }, v => _refresher = v);
    }
}