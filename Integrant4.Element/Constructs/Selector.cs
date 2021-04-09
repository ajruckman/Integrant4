using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Integrant4.Element.Inputs;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Superset.Utilities;

namespace Integrant4.Element.Constructs
{
    public partial class Selector<TValue>
    {
        private readonly OptionGetter _optionGetter;
        private readonly Spec         _spec;

        public Selector
        (
            IJSRuntime              jsRuntime,
            OptionGetter            optionGetter,
            Spec?                   spec             = null,
            OptionEqualityComparer? equalityComparer = null
        )
        {
            _optionGetter     = optionGetter;
            _equalityComparer = equalityComparer;
            _spec             = spec ?? new Spec();

            if (_spec.Filterable)
            {
                Debouncer<string?> filterDebouncer = new(
                    UpdateFilterValue,
                    null,
                    200
                );

                _filterInput = new TextInput(jsRuntime, null, new TextInput.Spec
                {
                    Placeholder = () => _spec.FilterPlaceholderText?.Invoke() ?? DefaultFilterPlaceholderText,
                    Scale       = _spec.Scale,
                });
                _filterInput.OnChange += v => filterDebouncer.Reset(v);
            }
        }
    }

    public partial class Selector<TValue>
    {
        public delegate Option<TValue>[] OptionGetter();

        private const string DefaultNoSelectionText       = "Click to select";
        private const string DefaultFilterPlaceholderText = "Filter options";
        private const string DefaultUncachedText          = "Loading options...";
        private const string DefaultNoOptionsText         = "No options available";
        private const string DefaultNoResultsText         = "No options matched filter";

        // This Spec is different from others, but it is nicer to initialize the Selector with named parameters.
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public bool Filterable { get; init; }

            public Callbacks.Callback<TValue>? Value                 { get; init; }
            public Callbacks.Callback<string>? NoSelectionText       { get; init; }
            public Callbacks.Callback<string>? FilterPlaceholderText { get; init; }
            public Callbacks.Callback<string>? UncachedText          { get; init; }
            public Callbacks.Callback<string>? NoOptionsText         { get; init; }
            public Callbacks.Callback<string>? NoResultsText         { get; init; }

            public Callbacks.IsVisible?  IsVisible  { get; init; }
            public Callbacks.IsDisabled? IsDisabled { get; init; }
            public Callbacks.Scale?      Scale      { get; init; }
        }
    }

    public partial class Selector<TValue>
    {
        public Task<TValue?> GetValue() => Task.FromResult(_selection != null ? _selection.Value.Value : default);

        public Task SetValue(TValue? value)
        {
            if (_selection == null && value == null) return Task.CompletedTask;
            if (_selection?.Value?.Equals(value) == true) return Task.CompletedTask;

            lock (_optionsLock)
            {
                if (_options == null)
                    throw new InvalidOperationException(
                        "Attempted to set the value of a Selector with no loaded options.");

                if (value == null)
                {
                    _selection = null;
                }
                else
                {
                    _selection = _options.FirstOrDefault(v => ValueEquals(v.Value, value));

                    if (_selection == null)
                        throw new InvalidOperationException(
                            "Could not find an option with a value equal to the one passed to SetValue.");
                }

                OnChange?.Invoke(value);
            }

            _refresher?.Invoke();

            return Task.CompletedTask;
        }

        public event Action<TValue?>? OnChange;
        public event Action?          OnLoaded;

        public Task Refresh()
        {
            _refresher?.Invoke();

            return Task.CompletedTask;
        }

        public Task ClearValue()
        {
            if (_selection == null) return Task.CompletedTask;

            _selection = null;
            OnChange?.Invoke(default);
            _refresher?.Invoke();

            return Task.CompletedTask;
        }
    }

    public partial class Selector<TValue>
    {
        public delegate bool OptionEqualityComparer(TValue? left, TValue? right);

        private readonly object                  _optionsLock = new();
        private readonly object                  _ctsLock     = new();
        private readonly OptionEqualityComparer? _equalityComparer;

        private Option<TValue>[]?        _options;
        private CancellationTokenSource? _cts = new();

        private Option<TValue>? _selection;
        private string?         _filterTerm;

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

                lock (_optionsLock)
                {
                    token.ThrowIfCancellationRequested();
                    _options = result;

                    if (_spec.Value != null)
                    {
                        _selection = _options.FirstOrDefault(v => ValueEquals(v.Value, _spec.Value.Invoke()));
                    }

                    _refresher?.Invoke();
                    OnLoaded?.Invoke();
                }
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

        private void Invalidate()
        {
            lock (_optionsLock)
            {
                _options = null;
            }

            _refresher?.Invoke();
        }

        private bool OptionEquals(Option<TValue> left, Option<TValue> right) =>
            _equalityComparer?.Invoke(left.Value, right.Value) ?? left.Value?.Equals(right.Value) == true;

        private bool ValueEquals(TValue? left, TValue? right) =>
            _equalityComparer?.Invoke(left, right) ?? left?.Equals(right) == true;

        // private Option<TValue>[]? EffectiveOptions()
        // {
        //     lock (_optionsLock)
        //     {
        //         if (_options == null) return null;
        //
        //         if (string.IsNullOrWhiteSpace(_filterTerm)) return _options;
        //
        //         return _options.Where(v => v.FilterableText.Contains(_filterTerm, StringComparison.OrdinalIgnoreCase))
        //            .ToArray();
        //     }
        // }
    }

    public partial class Selector<TValue>
    {
        private void UpdateFilterValue(string? value)
        {
            _filterTerm = value;
            _refresher?.Invoke();
        }
    }

    public partial class Selector<TValue>
    {
        [JSInvokable("I4E.Construct.Selector.Select")]
        public void Construct_Selector_Select(int i)
        {
            lock (_optionsLock)
            {
                if (_options == null) return;

                if (_spec.IsDisabled?.Invoke() ?? false)
                {
                    if (!_disabledAtLastRender)
                    {
                        Console.WriteLine("Disabled now");
                        _refresher?.Invoke();
                    }

                    return;
                }

                _selection = _options[i];
                OnChange?.Invoke(_selection.Value.Value);
                Console.WriteLine($"{i} <- {_selection}");

                _refresher?.Invoke();
            }
        }

        [JSInvokable("I4E.Construct.Selector.Open")]
        public void Construct_Selector_Open()
        {
            BeginLoadingOptions();
        }
    }

    public partial class Selector<TValue>
    {
        private const int UnfilteredDisplayLimit = 15;

        private readonly TextInput? _filterInput;

        private ElementReference? _elemRef;
        private Func<Task>?       _refresher;
        private ElementService?   _elementService;
        private bool              _disabledAtLastRender;

        public RenderFragment Renderer() => TickingRefreshWrapper.Create
        (
            builder =>
            {
                Console.WriteLine("Selector > Renderer");
                Stopwatch sw = new();
                sw.Start();

                bool disabled = _spec.IsDisabled?.Invoke() ?? false;
                _disabledAtLastRender = disabled;

                BootstrapIcon clearValueButton = new("x-circle-fill", (ushort) (12 * _spec.Scale?.Invoke() ?? 12));

                int seq = -1;

                ServiceInjector<ElementService>.Inject(builder, ref seq, v => _elementService = v);

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class",
                    !_spec.Filterable
                        ? "I4E-Construct-Selector"
                        : "I4E-Construct-Selector I4E-Construct-Selector--Filterable");

                builder.AddAttribute(++seq, "data-visible",  _spec.IsVisible?.Invoke() ?? true);
                builder.AddAttribute(++seq, "data-disabled", disabled);

                ++seq;
                if (_spec.Scale != null) builder.AddAttribute(seq, "style", $"font-size: {_spec.Scale.Invoke()}rem;");

                builder.AddElementReferenceCapture(++seq, r => _elemRef = r);

                //

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class",         "I4E-Construct-Selector-Head");
                builder.AddAttribute(++seq, "data-disabled", disabled);
                builder.AddAttribute(++seq, "tabindex",      0);

                builder.OpenElement(++seq, "div");

                if (_selection == null)
                {
                    builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-NoSelection");
                    builder.AddContent(++seq, _spec.NoSelectionText?.Invoke() ?? DefaultNoSelectionText);
                }
                else
                {
                    builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Selection");
                    builder.AddContent(++seq, _selection.Value.SelectionContent?.Renderer() ??
                                              _selection.Value.OptionContent.Renderer());
                }

                builder.CloseElement();

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class",    "I4E-Construct-Selector-ClearButtonWrapper");
                builder.AddAttribute(++seq, "tabindex", 0);
                builder.AddAttribute(++seq, "onclick",  EventCallback.Factory.Create(this, ClearValue));
                builder.AddContent(++seq, clearValueButton.Renderer());
                builder.CloseElement();

                builder.CloseElement();

                //

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Dropdown");

                ++seq;
                if (_spec.Filterable) builder.AddContent(seq, _filterInput?.Renderer());

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Scroller");
                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Options");

                lock (_optionsLock)
                {
                    if (_options == null)
                    {
                        builder.OpenElement(++seq, "p");
                        builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Options-Null");
                        builder.AddContent(++seq, _spec.UncachedText?.Invoke() ?? DefaultUncachedText);
                        builder.CloseElement();
                    }
                    else if (_options.Length == 0)
                    {
                        builder.OpenElement(++seq, "p");
                        builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Options-None");
                        builder.AddContent(++seq, _spec.NoOptionsText?.Invoke() ?? DefaultNoOptionsText);
                        builder.CloseElement();
                    }
                    else
                    {
                        var shownCount = 0;

                        for (var i = 0; i < _options.Length; i++)
                        {
                            Option<TValue> option = _options[i];

                            if (_spec.Filterable && option.FilterableText == null)
                                throw new InvalidOperationException(
                                    "Option passed to Selector does not have filterable text, but the Selector is filterable.");

                            bool selected = _selection != null && OptionEquals(_selection.Value, option);

                            bool shown =
                                _spec.Filterable                    &&
                                shownCount < UnfilteredDisplayLimit &&
                                (string.IsNullOrWhiteSpace(_filterTerm) ||
                                 option.FilterableText!.Contains(_filterTerm, StringComparison.OrdinalIgnoreCase));

                            if (_spec.Filterable && shown) shownCount++;

                            builder.OpenElement(++seq, "div");
                            builder.SetKey(i);

                            builder.AddAttribute(++seq, "tabindex",      0);
                            builder.AddAttribute(++seq, "data-selected", selected);
                            builder.AddAttribute(++seq, "data-i",        i);
                            if (_spec.Filterable)
                                builder.AddAttribute(++seq, "data-shown", shown);

                            builder.AddContent(++seq, option.OptionContent.Renderer());
                            builder.CloseElement();
                        }

                        if (_spec.Filterable)
                        {
                            builder.OpenElement(++seq, "p");
                            builder.AddAttribute(++seq, "class",      "I4E-Construct-Options-LimitMessage");
                            builder.AddAttribute(++seq, "data-shown", shownCount == UnfilteredDisplayLimit);
                            builder.AddContent(++seq, $"Filter to see more than {UnfilteredDisplayLimit} options.");
                            builder.CloseElement();

                            builder.OpenElement(++seq, "p");
                            builder.AddAttribute(++seq, "class",      "I4E-Construct-Options-NoResults");
                            builder.AddAttribute(++seq, "data-shown", shownCount == 0);
                            builder.AddContent(++seq, _spec.NoResultsText?.Invoke() ?? DefaultNoResultsText);
                            builder.CloseElement();
                        }
                    }
                }

                builder.CloseElement();
                builder.CloseElement();
                builder.CloseElement();

                //

                builder.CloseElement();

                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);
            },
            v => _refresher = v,
            firstRender =>
            {
                if (_elementService == null || _elemRef == null)
                    return Task.CompletedTask;
                else if (firstRender)
                    return Interop.CallVoid
                    (
                        _elementService.JSRuntime,
                        _elementService.CancellationToken,
                        "I4.Element.InitSelector",
                        _elemRef.Value,
                        DotNetObjectReference.Create(this),
                        _spec.Filterable
                    );
                else
                    return Interop.CallVoid
                    (
                        _elementService.JSRuntime,
                        _elementService.CancellationToken,
                        "I4.Element.UpdateSelector",
                        _elemRef.Value
                    );
            });

        public void BeginLoadingOptions()
        {
            lock (_optionsLock)
            {
                if (_options == null)
                {
                    Task.Run(BeginCache);
                }
            }
        }

        public async Task Show()
        {
            if (_elementService == null || _elemRef == null) return;

            await Interop.CallVoid(_elementService.JSRuntime, _elementService.CancellationToken,
                "I4.Element.ShowSelector", _elemRef.Value);
        }

        public async Task Hide()
        {
            if (_elementService == null || _elemRef == null) return;

            await Interop.CallVoid(_elementService.JSRuntime, _elementService.CancellationToken,
                "I4.Element.HideSelector", _elemRef.Value);
        }
    }

    public readonly struct Option<TValue>
    {
        public readonly TValue?  Value;
        public readonly Content  OptionContent;
        public readonly Content? SelectionContent;
        public readonly string?  FilterableText;
        public readonly bool     Disabled;
        public readonly bool     Placeholder;

        public Option
        (
            TValue? value,
            Content optionContent, Content? selectionContent, string? filterableText,
            bool    disabled,      bool     placeholder
        )
        {
            Value            = value;
            OptionContent    = optionContent;
            SelectionContent = selectionContent;
            FilterableText   = filterableText;
            Disabled         = disabled;
            Placeholder      = placeholder;
        }
    }
}