using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Integrant4.Element.Inputs;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using Superset.Utilities;

namespace Integrant4.Element.Constructs
{
    public partial class Combobox<TValue> where TValue : IEquatable<TValue>
    {
        private readonly Spec _spec;

        private readonly TextOptionGetter?    _textOptionGetter;
        private readonly ContentOptionGetter? _contentOptionGetter;

        public Combobox
        (
            IJSRuntime       jsRuntime,
            TextOptionGetter optionGetter,
            Spec?            spec = null
        )
        {
            _mode             = Mode.Text;
            _textOptionGetter = optionGetter;
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

        public Combobox
        (
            IJSRuntime          jsRuntime,
            ContentOptionGetter optionGetter,
            Spec?               spec = null
        )
        {
            _mode                = Mode.Content;
            _contentOptionGetter = optionGetter;
            _spec                = spec ?? new Spec();

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

    public partial class Combobox<TValue>
    {
        public delegate TextOption<TValue>[]    TextOptionGetter();
        public delegate ContentOption<TValue>[] ContentOptionGetter();

        private const string DefaultNoSelectionText       = "Click to select";
        private const string DefaultFilterPlaceholderText = "Filter options";
        private const string DefaultUncachedText          = "Loading options...";
        private const string DefaultNoOptionsText         = "No options available";
        private const string DefaultNoResultsText         = "No options matched filter";

        // This Spec is different from others, but it is nicer to initialize the Combobox with named parameters.
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public bool Filterable { get; init; }

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

    public partial class Combobox<TValue>
    {
        public Task<TValue?> GetValue() => Task.FromResult(Selection != null ? Selection.Value : default);

        public Task SetValue(TValue? value)
        {
            if (GetValue().Result?.Equals(value) == true) return Task.CompletedTask;

            lock (_optionsLock)
            {
                if (Options == null)
                    throw new InvalidOperationException(
                        "Attempted to set the value of a Combobox with no loaded options.");

                IOptionBase<TValue>? selection = _mode switch
                {
                    Mode.Text    => _textOptions!.FirstOrDefault(v => v.Value?.Equals(value)    == true),
                    Mode.Content => _contentOptions!.FirstOrDefault(v => v.Value?.Equals(value) == true),
                    _            => throw new ArgumentOutOfRangeException(),
                };

                if (Selection == null)
                    throw new InvalidOperationException(
                        "Could not find an option with a value equal to the one passed to SetValue.");

                OnChange?.Invoke(value);
            }

            _refresher?.Invoke();

            return Task.CompletedTask;
        }

        public event Action<TValue?>? OnChange;

        public Task Refresh()
        {
            _refresher?.Invoke();

            return Task.CompletedTask;
        }

        public Task ClearValue()
        {
            if (Selection == null) return Task.CompletedTask;

            _textSelection    = null;
            _contentSelection = null;

            OnChange?.Invoke(default);
            _refresher?.Invoke();

            return Task.CompletedTask;
        }
    }

    public partial class Combobox<TValue>
    {
        [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
        private IOptionBase<TValue>[]? Options => _mode switch
        {
            Mode.Text    => _textOptions,
            Mode.Content => _contentOptions,
            _            => throw new ArgumentOutOfRangeException(),
        };

        private TextOption<TValue>[]?    _textOptions;
        private ContentOption<TValue>[]? _contentOptions;

        private IOptionBase<TValue>? Selection => _mode switch
        {
            Mode.Text    => _textSelection,
            Mode.Content => _contentSelection,
            _            => throw new ArgumentOutOfRangeException(),
        };

        private TextOption<TValue>?    _textSelection;
        private ContentOption<TValue>? _contentSelection;
    }

    public partial class Combobox<TValue>
    {
        private readonly object _optionsLock = new();
        private readonly object _ctsLock     = new();

        private CancellationTokenSource? _cts = new();

        private string? _filterTerm;

        [SuppressMessage("ReSharper", "RedundantCast")]
        private async Task BeginCache()
        {
            CancellationToken token;

            lock (_ctsLock)
            {
                _cts?.Cancel();
                _cts  = new CancellationTokenSource();
                token = _cts.Token;
            }

            Task<IReadOnlyList<IOptionBase<TValue>>> task = Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();

                return _mode switch
                {
                    Mode.Text    => (IReadOnlyList<IOptionBase<TValue>>) _textOptionGetter!.Invoke(),
                    Mode.Content => (IReadOnlyList<IOptionBase<TValue>>) _contentOptionGetter!.Invoke(),
                    _            => throw new ArgumentOutOfRangeException(),
                };
            }, token);

            try
            {
                IReadOnlyList<IOptionBase<TValue>> result = await task;

                lock (_optionsLock)
                {
                    token.ThrowIfCancellationRequested();

                    switch (_mode)
                    {
                        case Mode.Text:
                            _textOptions = (TextOption<TValue>[]?) result;
                            break;
                        case Mode.Content:
                            _contentOptions = (ContentOption<TValue>[]?) result;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

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
                _textOptions    = null;
                _contentOptions = null;
            }

            _refresher?.Invoke();
        }
    }

    public partial class Combobox<TValue>
    {
        private void UpdateFilterValue(string? value)
        {
            _filterTerm = value;
            _refresher?.Invoke();
        }
    }

    public partial class Combobox<TValue>
    {
        [JSInvokable("I4E.Construct.Combobox.Select")]
        public void Construct_Combobox_Select(int i)
        {
            lock (_optionsLock)
            {
                if (Options == null) return;

                if (_spec.IsDisabled?.Invoke() ?? false)
                {
                    if (!_disabledAtLastRender)
                    {
                        Console.WriteLine("Disabled now");
                        _refresher?.Invoke();
                    }

                    return;
                }

                switch (_mode)
                {
                    case Mode.Text:
                        _textSelection = (TextOption<TValue>?) Options[i];
                        break;
                    case Mode.Content:
                        _contentSelection = (ContentOption<TValue>?) Options[i];
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                OnChange?.Invoke(Selection!.Value);
                Console.WriteLine($"{i} <- {Selection}");

                _refresher?.Invoke();
            }
        }
    }

    public partial class Combobox<TValue>
    {
        private const int UnfilteredDisplayLimit = 15;

        private readonly TextInput? _filterInput;

        private ElementReference? _elemRef;
        private Func<Task>?       _refresher;
        private ElementService?   _elementService;
        private bool              _disabledAtLastRender;

        public RenderFragment Renderer() => RefreshLifecycleWrapper.Create
        (
            builder =>
            {
                Console.WriteLine("Combobox > Renderer");
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
                        ? "I4E-Construct-Combobox"
                        : "I4E-Construct-Combobox I4E-Construct-Combobox--Filterable");

                builder.AddAttribute(++seq, "data-visible", _spec.IsVisible?.Invoke() ?? true);
                builder.AddAttribute(++seq, "data-disabled", disabled);

                ++seq;
                if (_spec.Scale != null) builder.AddAttribute(seq, "style", $"font-size: {_spec.Scale.Invoke()}rem;");

                builder.AddElementReferenceCapture(++seq, r => _elemRef = r);

                //

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-Combobox-Head");
                builder.AddAttribute(++seq, "data-disabled", disabled);
                builder.AddAttribute(++seq, "tabindex", 0);

                builder.OpenElement(++seq, "div");

                if (Selection == null)
                {
                    builder.AddAttribute(++seq, "class", "I4E-Construct-Combobox-NoSelection");
                    builder.AddContent(++seq, _spec.NoSelectionText?.Invoke() ?? DefaultNoSelectionText);
                }
                else
                {
                    builder.AddAttribute(++seq, "class", "I4E-Construct-Combobox-Selection");
                    switch (_mode)
                    {
                        case Mode.Text:
                            builder.AddContent(++seq, _textSelection!.SelectionText);
                            break;
                        case Mode.Content:
                            builder.AddContent(++seq, _contentSelection!.SelectionContent.Renderer());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                builder.CloseElement();

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-Combobox-ClearButtonWrapper");
                builder.AddAttribute(++seq, "tabindex", 0);
                builder.AddAttribute(++seq, "onclick", EventCallback.Factory.Create(this, ClearValue));
                builder.AddContent(++seq, clearValueButton.Renderer());
                builder.CloseElement();

                builder.CloseElement();

                //

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-Combobox-Dropdown");

                ++seq;
                if (_spec.Filterable) builder.AddContent(seq, _filterInput?.Renderer());

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-Combobox-Scroller");
                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Construct-Combobox-Options");

                lock (_optionsLock)
                {
                    if (Options == null)
                    {
                        builder.OpenElement(++seq, "p");
                        builder.AddAttribute(++seq, "class", "I4E-Construct-Combobox-Options-Null");
                        builder.AddContent(++seq, _spec.UncachedText?.Invoke() ?? DefaultUncachedText);
                        builder.CloseElement();
                    }
                    else if (Options.Length == 0)
                    {
                        builder.OpenElement(++seq, "p");
                        builder.AddAttribute(++seq, "class", "I4E-Construct-Combobox-Options-None");
                        builder.AddContent(++seq, _spec.NoOptionsText?.Invoke() ?? DefaultNoOptionsText);
                        builder.CloseElement();
                    }
                    else
                    {
                        var shownCount = 0;

                        for (var i = 0; i < Options.Length; i++)
                        {
                            IOptionBase<TValue> option = Options[i];

                            bool selected =
                                Selection                             != null &&
                                option.Value?.Equals(Selection.Value) == true;

                            bool shown =
                                _spec.Filterable                    &&
                                shownCount < UnfilteredDisplayLimit &&
                                (string.IsNullOrWhiteSpace(_filterTerm) || option.Matches(_filterTerm));

                            if (_spec.Filterable && shown) shownCount++;

                            builder.OpenElement(++seq, "div");
                            builder.SetKey(i);

                            builder.AddAttribute(++seq, "tabindex", 0);
                            builder.AddAttribute(++seq, "data-selected", selected);
                            builder.AddAttribute(++seq, "data-i", i);
                            if (_spec.Filterable)
                                builder.AddAttribute(++seq, "data-shown", shown);
                            option.Render(builder, ref seq);
                            builder.CloseElement();
                        }

                        if (_spec.Filterable)
                        {
                            builder.OpenElement(++seq, "p");
                            builder.AddAttribute(++seq, "class", "I4E-Construct-Options-LimitMessage");
                            builder.AddAttribute(++seq, "data-shown", shownCount == UnfilteredDisplayLimit);
                            builder.AddContent(++seq, $"Filter to see more than {UnfilteredDisplayLimit} options.");
                            builder.CloseElement();

                            builder.OpenElement(++seq, "p");
                            builder.AddAttribute(++seq, "class", "I4E-Construct-Options-NoResults");
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
                    return Interop.InitCombobox
                    (
                        _elementService.JSRuntime,
                        _elementService.CancellationToken,
                        _elemRef.Value,
                        DotNetObjectReference.Create(this),
                        _spec.Filterable
                    );
                else
                    return Interop.UpdateCombobox
                    (
                        _elementService.JSRuntime,
                        _elementService.CancellationToken,
                        _elemRef.Value
                    );
            });

        public void LoadInBackground()
        {
            lock (_optionsLock)
            {
                if (Options == null)
                {
                    Task.Run(BeginCache);
                }
            }
        }

        public async Task Show()
        {
            if (_elementService == null || _elemRef == null) return;

            await Interop.ShowCombobox(_elementService.JSRuntime, _elementService.CancellationToken, _elemRef.Value);
        }

        public async Task Hide()
        {
            if (_elementService == null || _elemRef == null) return;

            await Interop.HideCombobox(_elementService.JSRuntime, _elementService.CancellationToken, _elemRef.Value);
        }
    }

    public partial class Combobox<TValue>
    {
        private readonly Mode _mode;

        private enum Mode
        {
            Text, Content,
        };
    }

    public interface IOptionBase<out TValue> where TValue : IEquatable<TValue>
    {
        public TValue? Value       { get; }
        public bool    Disabled    { get; }
        public bool    Placeholder { get; }

        public bool Matches(string           term);
        void        Render(RenderTreeBuilder builder, ref int seq);
    }

    public class TextOption<TValue> : IOptionBase<TValue> where TValue : IEquatable<TValue>
    {
        public readonly string OptionText;
        public readonly string SelectionText;

        public TextOption
        (
            TValue? value,
            string  optionText,
            string? selectionText = null,
            bool    disabled      = false,
            bool    placeholder   = false
        )
        {
            Value         = value;
            OptionText    = optionText;
            SelectionText = selectionText ?? optionText;
            Disabled      = disabled;
            Placeholder   = placeholder;
        }

        public TValue? Value       { get; }
        public bool    Disabled    { get; }
        public bool    Placeholder { get; }

        public bool Matches(string term) => OptionText.Contains(term, StringComparison.OrdinalIgnoreCase);

        public void Render(RenderTreeBuilder builder, ref int seq) =>
            builder.AddContent(++seq, OptionText);
    }

    public class ContentOption<TValue> : IOptionBase<TValue> where TValue : IEquatable<TValue>
    {
        public readonly string  FilterableText;
        public readonly Content OptionContent;
        public readonly Content SelectionContent;

        public ContentOption
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

        public TValue? Value       { get; }
        public bool    Disabled    { get; }
        public bool    Placeholder { get; }

        public bool Matches(string term) => FilterableText.Contains(term, StringComparison.OrdinalIgnoreCase);

        public void Render(RenderTreeBuilder builder, ref int seq) =>
            builder.AddContent(++seq, OptionContent.Renderer());
    }
}