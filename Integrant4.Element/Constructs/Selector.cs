using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element.Inputs;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Constructs
{
    public partial class Selector<TValue> : IRenderable, IInput<TValue>
        where TValue : IEquatable<TValue>
    {
        public delegate Option<TValue>[] OptionGetter(string? searchTerm);
    }

    public partial class Selector<TValue>
    {
        public Task<TValue?> GetValue() => Task.FromResult(_selection != null ? _selection.Value.Value : default);

        public async Task SetValue(TValue? value)
        {
            if (_selection?.Value?.Equals(value) == true) return;

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

            await (_refresher?.Invoke() ?? Task.CompletedTask);
        }

        public event Action<TValue?>? OnChange;
    }

    public partial class Selector<TValue>
    {
        private readonly OptionGetter _optionGetter;
        private readonly string       _uncachedText = "Loading options...";
        private readonly string       _noOptionText = "No results";

        public Selector(IJSRuntime jsRuntime, OptionGetter optionGetter, string? uncachedText = null,
            string?                noOptionText = null)
        {
            _optionGetter = optionGetter;
            _uncachedText = uncachedText ?? _uncachedText;
            _noOptionText = noOptionText ?? _noOptionText;

            _filterInput          =  new TextInput(jsRuntime, null);
            _filterInput.OnChange += UpdateFilterValue;
        }
    }

    public partial class Selector<TValue>
    {
        private readonly object _optionsLock = new();
        private readonly object _ctsLock     = new();

        private Option<TValue>[]?        _options;
        private CancellationTokenSource? _cts = new();

        private string?         _filterTerm;
        private Option<TValue>? _selection;

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
                return _optionGetter.Invoke(_filterTerm);
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

        private void UpdateFilterValue(string? value)
        {
            _filterTerm = value;
        }

        private Option<TValue>[]? EffectiveOptions()
        {
            lock (_optionsLock)
            {
                if (_options == null) return null;

                if (string.IsNullOrWhiteSpace(_filterTerm)) return _options;

                return _options.Where(v => v.FilterableText.Contains(_filterTerm, StringComparison.OrdinalIgnoreCase))
                   .ToArray();
            }
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
        private ElementReference? _elemRef;
        private Func<Task>?       _refresher;
        private ElementService?   _elementService;
        private TextInput?        _filterInput;

        public RenderFragment Renderer() => RefreshLifecycleWrapper.Create
        (
            builder =>
            {
                Console.WriteLine("Selector > Renderer");
                Stopwatch sw  = new();
                sw.Start();
                int       seq = -1;

                ServiceInjector<ElementService>.Inject(builder, ref seq, v => _elementService = v);

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-Selector");
                builder.AddElementReferenceCapture(++seq, r => _elemRef = r);

                //

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Head");

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

                builder.CloseElement();

                //

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Dropdown");
                builder.AddContent(++seq, _filterInput?.Renderer());

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Options");

                lock (_optionsLock)
                {
                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Options-Null");
                    builder.AddContent(++seq, _uncachedText);
                    builder.CloseElement();
                    
                    if (_options == null)
                    {
                        builder.OpenElement(++seq, "div");
                        builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Notice");
                        builder.AddContent(++seq, _uncachedText);
                        builder.CloseElement();
                    }
                    else if (_options.Length == 0)
                    {
                        builder.OpenElement(++seq, "div");
                        builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Notice");
                        builder.AddContent(++seq, _noOptionText);
                        builder.CloseElement();
                    }
                    else
                    {
                        builder.OpenElement(++seq, "div");
                        builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-Notice");
                        builder.AddAttribute(++seq, "style", "display: none");
                        builder.CloseElement();
                        
                        for (var i = 0; i < _options.Length; i++)
                        {
                            // if (i == DisplayLimit && _options.Length > DisplayLimit)
                            // {
                            //     builder.OpenElement(++seq, "div");
                            //     builder.AddAttribute(++seq, "class", "I4E-Construct-Selector-LimitMessage");
                            //     builder.AddContent(++seq, $"Filter to see more than {DisplayLimit} options.");
                            //     builder.CloseElement();
                            //
                            //     break;
                            // }

                            Option<TValue> option = _options[i];

                            // bool shown =
                            //     string.IsNullOrWhiteSpace(_filterTerm) ||
                            //     option.FilterableText.Contains(_filterTerm, StringComparison.OrdinalIgnoreCase);

                            builder.OpenElement(++seq, "div");
                            builder.AddAttribute(++seq, "selected",
                                _selection != null && (option.Value?.Equals(_selection.Value.Value) == true));
                            builder.AddAttribute(++seq, "i", i);
                            builder.AddContent(++seq, option.OptionContent.Renderer());
                            builder.CloseElement();
                        }
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
                            DotNetObjectReference.Create(this)
                        )
                        : Task.CompletedTask
        );

        private const int DisplayLimit = 5;

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