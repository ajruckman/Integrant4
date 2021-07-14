using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        private readonly Spec                                     _spec;
        private readonly Selector<TValue>.OptionEqualityComparer? _equalityComparer;
        private readonly Selector<TValue>.OptionGetter            _optionGetter;

        private readonly Selector<TValue>              _selector;
        private readonly List<Selector<TValue>.Option> _selected;

        public MultiSelector
        (
            IJSRuntime                               jsRuntime,
            OptionGetter                             optionGetter,
            Spec?                                    spec             = null,
            Selector<TValue>.OptionEqualityComparer? equalityComparer = null
        )
        {
            _spec             = spec ?? new Spec();
            _equalityComparer = equalityComparer;
            _optionGetter     = optionGetter.Invoke;

            _selector = new Selector<TValue>(jsRuntime, Array.Empty<Selector<TValue>.Option>,
                spec?.SubSpec() ?? new Selector<TValue>.Spec(), _equalityComparer);
            _selected = new List<Selector<TValue>.Option>();

            _selector.OnChange += Select;

            _deselectValueButton = new BootstrapIcon("x-circle-fill", (ushort) (12 * _spec.Scale?.Invoke() ?? 12));
        }
    }

    public partial class MultiSelector<TValue>
    {
        public delegate Selector<TValue>.Option[] OptionGetter();

        // This Spec is different from others, but it is nicer to initialize the Selector with named parameters.
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public bool   Filterable   { get; init; }
            public ushort DisplayLimit { get; init; } = 100;

            public Callbacks.Callback<TValue[]>? Value                 { get; init; }
            public Callbacks.Callback<string>?   NoSelectionText       { get; init; }
            public Callbacks.Callback<string>?   FilterPlaceholderText { get; init; }
            public Callbacks.Callback<string>?   UncachedText          { get; init; }
            public Callbacks.Callback<string>?   NoOptionsText         { get; init; }
            public Callbacks.Callback<string>?   NoResultsText         { get; init; }

            public Callbacks.IsVisible?  IsVisible  { get; init; }
            public Callbacks.IsDisabled? IsDisabled { get; init; }
            public Callbacks.Unit?       Width      { get; init; }
            public Callbacks.Scale?      Scale      { get; init; }

            internal Selector<TValue>.Spec SubSpec() => new()
            {
                Filterable            = Filterable,
                DisplayLimit          = DisplayLimit,
                Value                 = null,
                NoSelectionText       = NoSelectionText,
                FilterPlaceholderText = FilterPlaceholderText,
                UncachedText          = UncachedText,
                NoOptionsText         = NoOptionsText,
                NoResultsText         = NoResultsText,
                IsVisible             = IsVisible,
                IsDisabled            = IsDisabled,
                Width                 = Width,
                Scale                 = Scale,
            };
        }
    }

    public partial class MultiSelector<TValue>
    {
        private readonly object _optionsLock = new();
        private readonly object _ctsLock     = new();

        private Selector<TValue>.Option[]? _options;
        private CancellationTokenSource?   _cts = new();

        private Selector<TValue>.Option[] Options()
        {
            lock (_optionsLock)
            {
                return _options ??= _optionGetter.Invoke();
            }
        }

        private Selector<TValue>.Option[] InnerOptions()
        {
            Selector<TValue>.Option[] all    = Options();
            Selector<TValue>.Option[] result = new Selector<TValue>.Option [all.Length];

            for (var i = 0; i < all.Length; i++)
            {
                Selector<TValue>.Option v = all[i];

                if (_selected.Contains(all[i]))
                    result[i] = new Selector<TValue>.Option
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

            Task<Selector<TValue>.Option[]> task = Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();
                return _optionGetter.Invoke();
            }, token);

            try
            {
                Selector<TValue>.Option[] result = await task;
                token.ThrowIfCancellationRequested();

                lock (_optionsLock)
                {
                    if (_spec.Value != null)
                    {
                        foreach (TValue v in _spec.Value.Invoke())
                        {
                            foreach (Selector<TValue>.Option option in result)
                            {
                                if (!ValueEquals(option.Value, v)) continue;

                                _selected.Add(option);
                                goto Next;
                            }

                            throw new InvalidOperationException(
                                "Could not find an option with a value equal to a pre-selected value.");

                            Next: ;
                        }
                    }

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
        public event Action<IReadOnlyList<Selector<TValue>.Option>?>? OnChange;
        public event Action?                                          OnLoaded;

        public void Refresh()
        {
            _refresher?.Invoke();
        }

        public TValue?[]? GetValue()
        {
            return _selected.Count == 0
                ? null
                : _selected.Select(v => v.Value).ToArray();
        }

        public void SetValue(TValue?[]? value, bool invokeOnChange = true)
        {
            _selected.Clear();

            if (value == null)
            {
                Update();
                return;
            }

            foreach (TValue? v in value)
            {
                foreach (Selector<TValue>.Option option in Options())
                {
                    if (!ValueEquals(option.Value, v)) continue;

                    _selected.Add(option);
                    goto Next;
                }

                throw new InvalidOperationException(
                    "Could not find an option with a value equal to one passed to SetValue.");

                Next: ;
            }

            Update(invokeOnChange);
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
        private void Select(Selector<TValue>.Option? v)
        {
            if (v == null || v.Value.Value == null) return;

            if (!_selected.Contains(v.Value))
            {
                _selected.Add(v.Value);
            }

            _selector.ClearValue();
            Update();
        }

        private void Deselect(Selector<TValue>.Option v)
        {
            if (_spec.IsDisabled?.Invoke() ?? false) return;
            if (v.Value == null) return;

            if (!_selected.Remove(v)) return;

            Update();
        }

        private void Update(bool invokeOnChange = true)
        {
            _selector.SetOptions(InnerOptions());
            if (invokeOnChange) OnChange?.Invoke(_selected.Count == 0 ? null : _selected);
            _refresher?.Invoke();
        }
    }

    public partial class MultiSelector<TValue>
    {
        private readonly BootstrapIcon _deselectValueButton;

        private WriteOnlyHook? _refresher;

        public RenderFragment Renderer() => Latch.Create(builder =>
        {
            Unit width = _spec.Width?.Invoke() ?? 300;

            int seq = -1;

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class",         "I4E-Construct-MultiSelector");
            builder.AddAttribute(++seq, "style",         $"max-width: {width.Serialize()}");
            builder.AddAttribute(++seq, "data-disabled", _spec.IsDisabled?.Invoke() ?? false);

            builder.AddContent(++seq, _selector.Renderer());

            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct-MultiSelector-Selections");

            foreach (Selector<TValue>.Option selection in _selected)
            {
                builder.OpenRegion(++seq);
                var seqI = 0;

                builder.OpenElement(++seqI, "div");
                builder.AddAttribute(++seqI, "class", "I4E-Construct-MultiSelector-Selection");

                builder.AddContent(++seqI, selection.SelectionContent?.Renderer() ??
                                           selection.OptionContent.Renderer());

                builder.OpenElement(++seqI, "div");
                builder.AddAttribute(++seqI, "class",    "I4E-Construct-MultiSelector-DeselectButtonWrapper");
                builder.AddAttribute(++seqI, "tabindex", 0);
                builder.AddAttribute(++seqI, "onclick",
                    EventCallback.Factory.Create(this, () => Deselect(selection)));
                builder.AddContent(++seqI, _deselectValueButton.Renderer());
                builder.CloseElement();

                builder.CloseElement();

                builder.CloseRegion();
            }

            builder.CloseElement();

            builder.CloseElement();
        }, v => _refresher = v);
    }
}