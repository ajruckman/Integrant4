using System;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Integrant4.Colorant
{
    public sealed class VariantLoader
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IJSRuntime           _jsRuntime;
        private readonly ITheme               _theme;
        private readonly string               _defaultVariant;
        private readonly Hook                 _update = new();

        private string? _variant;

        public VariantLoader
        (
            ILocalStorageService localStorage, IJSRuntime jsRuntime, ITheme theme, string defaultVariant
        )
        {
            _localStorage   = localStorage;
            _jsRuntime      = jsRuntime;
            _theme          = theme;
            _defaultVariant = defaultVariant;

            if (theme.Variants.Count() == 1)
            {
                _variant = theme.Variants.First();
                Complete = true;
            }
        }

        public string? Variant => _variant;

        public bool Complete { get; private set; }

        public event Action<string>? OnComplete;
        public event Action<string>? OnVariantChange;

        public RenderFragment RenderStylesheets()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenComponent<Latch>(++seq);
                builder.AddAttribute(++seq, "Hook", (ReadOnlyHook)_update);

                builder.AddAttribute(++seq, "ChildContent", new RenderFragment(builder2 =>
                {
                    builder2.OpenElement(++seq, "link");
                    builder2.AddAttribute(++seq, "rel", "stylesheet");
                    builder2.AddAttribute(++seq, "href",
                        $"_content/{_theme.Assembly}/css/{_theme.Name}/{_variant}.css");
                    builder2.CloseElement();
                }));

                builder.CloseComponent();
            }

            return Fragment;
        }

        public RenderFragment RenderDropdown()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "id", $"I4C-Component-{nameof(VariantLoader)}");

                builder.OpenElement(++seq, "span");

                builder.OpenElement(++seq, "label");
                builder.AddAttribute(++seq, "for", $"I4C-Component-{nameof(VariantLoader)}");
                builder.AddContent(++seq, "Theme");
                builder.CloseElement();

                builder.OpenComponent<Latch>(++seq);
                builder.AddAttribute(++seq, "Hook", (ReadOnlyHook)_update);
                builder.AddAttribute(++seq, "ChildContent", (RenderFragment)(builder2 =>
                {
                    builder2.OpenElement(++seq, "select");
                    builder2.AddAttribute(++seq, "id",
                        $"I4C-Component-{nameof(VariantLoader)}-Dropdown");
                    builder2.AddAttribute(
                        ++seq,
                        "onchange",
                        EventCallback.Factory.Create<ChangeEventArgs>(this, OnVariantSelection)
                    );

                    foreach (var variant in _theme.Variants)
                    {
                        builder2.OpenElement(++seq, "option");
                        builder2.AddAttribute(++seq, "selected", _variant == variant);
                        builder2.AddContent(++seq, variant);
                        builder2.CloseElement();
                    }

                    builder2.CloseElement();
                }));
                builder.CloseComponent();

                builder.CloseElement();

                builder.CloseElement();
            }

            return Fragment;
        }

        public async Task Load()
        {
            var variant = await _localStorage.GetItemAsync<string>($"I4C.Variant.{_theme.Name}");
            if (string.IsNullOrEmpty(variant))
            {
                _variant = _defaultVariant;
                await _localStorage.SetItemAsync($"I4C.Variant.{_theme.Name}", _variant);
            }
            else
            {
                _variant = variant;
            }

            await _jsRuntime.InvokeVoidAsync("I4.Colorant.SetThemeVariant", _theme.Name, _variant);

            _update.Invoke();
            Complete = true;
            OnComplete?.Invoke(variant);
        }

        private async Task OnVariantSelection(ChangeEventArgs args)
        {
            var variant = args.Value!.ToString()!;
            _variant = variant;
            await _localStorage.SetItemAsync($"I4C.Variant.{_theme.Name}", _variant);

            await _jsRuntime.InvokeVoidAsync("I4.Colorant.SetThemeVariant", _theme.Name, _variant);

            _update.Invoke();
            OnVariantChange?.Invoke(variant);
        }
    }
}