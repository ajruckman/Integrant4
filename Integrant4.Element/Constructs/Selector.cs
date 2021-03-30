using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element.Inputs;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Superset.Utilities;

namespace Integrant4.Element.Constructs
{
    public partial class Selector<TValue> : IInput<TValue>
        where TValue : IEquatable<TValue>
    {
        public delegate Option<TValue>[] OptionGetter();
    }

    public partial class Selector<TValue>
    {
        public Task<TValue?> GetValue() => Task.FromResult(_selection != null ? _selection.Value.Value : default);

        public Task SetValue(TValue? value)
        {
            if (_selection?.Value?.Equals(value) == true) return Task.CompletedTask;

            lock (_optionsLock)
            {
                if (_options == null)
                    throw new InvalidOperationException(
                        "Attempted to set the value of a Selector with no loaded options.");

                _selection = _options.FirstOrDefault(v => v.Value?.Equals(value) == true);

                if (_selection == null)
                    throw new InvalidOperationException(
                        "Could not find an option with a value equal to the one passed to SetValue.");

                OnChange?.Invoke(value);
            }

            _refresher?.Invoke();

            return Task.CompletedTask;
        }

        public event Action<TValue?>? OnChange;

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
        private readonly OptionGetter _optionGetter;
        private readonly bool         _filterable;
        private readonly string       _uncachedText = "Loading options...";
        private readonly string       _noOptionText = "No results";

        public Selector
        (
            IJSRuntime   jsRuntime,
            OptionGetter optionGetter,
            bool         filterable            = false,
            string?      uncachedText          = null,
            string?      noOptionText          = null,
            string?      filterPlaceholderText = null
        )
        {
            _optionGetter = optionGetter;
            _filterable   = filterable;
            _uncachedText = uncachedText ?? _uncachedText;
            _noOptionText = noOptionText ?? _noOptionText;


            if (_filterable)
            {
                Debouncer<string?> filterDebouncer = new(
                    UpdateFilterValue,
                    null,
                    200
                );

                _filterInput = new TextInput(jsRuntime, null, new TextInput.Spec
                {
                    Placeholder = () => filterPlaceholderText ?? "Filter options",
                });
                _filterInput.OnChange += v => filterDebouncer.Reset(v);
            }
        }
    }

    public partial class Selector<TValue>
    {
        private readonly object _optionsLock = new();
        private readonly object _ctsLock     = new();

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
                    _refresher?.Invoke();
                    Console.WriteLine("OPTIONS LOADED");
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

    public readonly struct Option<TValue>
    {
        public readonly TValue? Value;
        public readonly string  FilterableText;
        public readonly Content OptionContent;
        public readonly Content SelectionContent;
        public readonly bool    Disabled;
        public readonly bool    Placeholder;

        public Option
        (
            TValue?  value,
            string   filterableText,
            Content  optionContent,
            Content? selectionContent = null,
            bool     disabled         = false,
            bool     placeholder      = false
        )
        {
            Value            = value;
            FilterableText   = filterableText;
            OptionContent    = optionContent;
            SelectionContent = selectionContent ?? optionContent;
            Disabled         = disabled;
            Placeholder      = placeholder;
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

                _selection = _options[i];
                OnChange?.Invoke(_selection.Value.Value);
                Console.WriteLine($"{i} <- {_selection}");

                _refresher?.Invoke();
            }
        }
    }

    public partial class Selector<TValue>
    {
        private const int UnfilteredDisplayLimit = 15;

        private readonly TextInput?        _filterInput;
        private readonly BootstrapIcon     _clearValueButton = new("x-circle-fill", 12);
        private          ElementReference? _elemRef;
        private          Func<Task>?       _refresher;
        private          ElementService?   _elementService;

        public RenderFragment Renderer() => RefreshLifecycleWrapper.Create
        (
            builder =>
            {
                Console.WriteLine("Selector > Renderer");
                Stopwatch sw = new();
                sw.Start();

                int seq = -1;

                ServiceInjector<ElementService>.Inject(builder, ref seq, v => _elementService = v);

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class",
                    !_filterable
                        ? "I4E-Construct-Selector"
                        : "I4E-Construct-Selector I4E-Construct-Selector--Filterable");
                builder.AddElementReferenceCapture(++seq, r => _elemRef = r);

                //

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Head");
                builder.AddAttribute(++seq, "tabindex", 0);

                builder.OpenElement(++seq, "div");

                if (_selection == null)
                {
                    builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-NoSelection");
                    builder.AddContent(++seq, "No selection");
                }
                else
                {
                    builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Selection");
                    builder.AddContent(++seq, _selection.Value.SelectionContent.Renderer());
                }

                builder.CloseElement();

                builder.OpenElement(++seq, "button");
                builder.AddAttribute(++seq, "tabindex", 0);
                builder.AddAttribute(++seq, "onclick", EventCallback.Factory.Create(this, ClearValue));
                builder.AddContent(++seq, _clearValueButton.Renderer());
                builder.CloseElement();

                builder.CloseElement();

                //

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Dropdown");

                ++seq;
                if (_filterable)
                {
                    builder.AddContent(seq, _filterInput?.Renderer());
                }

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Options");

                lock (_optionsLock)
                {
                    if (_options == null)
                    {
                        builder.OpenElement(++seq, "p");
                        builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Options-Null");
                        builder.AddContent(++seq, _uncachedText);
                        builder.CloseElement();
                    }
                    else if (_options.Length == 0)
                    {
                        builder.OpenElement(++seq, "p");
                        builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Options-None");
                        builder.AddContent(++seq, _noOptionText);
                        builder.CloseElement();
                    }
                    else
                    {
                        var shownCount = 0;

                        for (var i = 0; i < _options.Length; i++)
                        {
                            // if (shownCount >= DisplayLimit)
                            // {
                            //     builder.OpenElement(++seq, "span");
                            //     builder.AddAttribute(++seq, "class", "I4E-Construct-Options-LimitMessage");
                            //     builder.AddContent(++seq, $"Filter to see more than {DisplayLimit} options.");
                            //     builder.CloseElement();
                            //
                            //     break;
                            // }

                            Option<TValue> option = _options[i];

                            bool selected =
                                _selection                                   != null &&
                                option.Value?.Equals(_selection.Value.Value) == true;

                            bool shown =
                                _filterable                         &&
                                shownCount < UnfilteredDisplayLimit &&
                                (string.IsNullOrWhiteSpace(_filterTerm) ||
                                 option.FilterableText.Contains(_filterTerm, StringComparison.OrdinalIgnoreCase));

                            if (_filterable && shown) shownCount++;

                            builder.OpenElement(++seq, "div");
                            builder.SetKey(i);
                            
                            builder.AddAttribute(++seq, "tabindex", 0);
                            builder.AddAttribute(++seq, "data-selected", selected);
                            builder.AddAttribute(++seq, "data-i", i);
                            if (_filterable)
                                builder.AddAttribute(++seq, "data-shown", shown);
                            
                            builder.AddContent(++seq, option.OptionContent.Renderer());
                            builder.CloseElement();
                        }

                        builder.OpenElement(++seq, "p");
                        builder.AddAttribute(++seq, "class", "I4E-Construct-Options-LimitMessage");
                        builder.AddAttribute(++seq, "data-shown", shownCount == UnfilteredDisplayLimit);
                        builder.AddContent(++seq, $"Filter to see more than {UnfilteredDisplayLimit} options.");
                        builder.CloseElement();
                    }
                }

                builder.CloseElement();
                builder.CloseElement();

                //

                builder.CloseElement();

                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);
            },
            v => _refresher = v,
            firstRender =>
                _elementService == null || _elemRef == null
                    ? Task.CompletedTask
                    : firstRender
                        ? Interop.InitSelector
                        (
                            _elementService.JSRuntime,
                            _elementService.CancellationToken,
                            _elemRef.Value,
                            DotNetObjectReference.Create(this),
                            _filterable
                        )
                        : Task.CompletedTask
        );

        public void LoadInBackground()
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

            await Interop.ShowSelector(_elementService.JSRuntime, _elementService.CancellationToken, _elemRef.Value);
        }

        public async Task Hide()
        {
            if (_elementService == null || _elemRef == null) return;

            await Interop.HideSelector(_elementService.JSRuntime, _elementService.CancellationToken, _elemRef.Value);
        }
    }
}