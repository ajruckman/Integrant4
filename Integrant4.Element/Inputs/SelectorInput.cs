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
            TValue?                       value,
            Selector<TValue>.OptionGetter optionGetter,
            Spec?                         spec = null
        )
        {
            _selector = new Selector<TValue>
            (
                jsRuntime,
                optionGetter,
                new Selector<TValue>.Spec
                {
                    Filterable            = spec?.Filterable ?? false,
                    Value                 = value != null ? () => value : null,
                    NoSelectionText       = spec?.NoSelectionText,
                    FilterPlaceholderText = spec?.FilterPlaceholderText,
                    UncachedText          = spec?.UncachedText,
                    NoOptionsText         = spec?.NoOptionsText,
                    NoResultsText         = spec?.NoResultsText,
                    IsVisible             = spec?.IsVisible,
                    IsDisabled            = spec?.IsDisabled,
                    Scale                 = spec?.Scale,
                }
            );

            _selector.OnChange += OnChange;
        }

        public Task           Refresh()               => _selector.Refresh();
        public RenderFragment Renderer()              => _selector.Renderer();
        public Task<TValue?>  GetValue()              => _selector.GetValue();
        public Task           SetValue(TValue? value) => _selector.SetValue(value);

        public event Action<TValue?>? OnChange;

        public void BeginLoadingOptions() => _selector.BeginLoadingOptions();

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
}