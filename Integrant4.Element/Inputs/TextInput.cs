using System;
using System.Diagnostics.CodeAnalysis;
using Integrant4.Element.Bits;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public partial class TextInput : StandardInput<string?>
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public ElementService? ElementService { get; init; }

            public Callbacks.Callback<string>? Placeholder { get; init; }

            public Callbacks.IsVisible?  IsVisible       { get; init; }
            public Callbacks.IsDisabled? IsDisabled      { get; init; }
            public Callbacks.IsRequired? IsRequired      { get; init; }
            public Callbacks.Classes?    Classes         { get; init; }
            public Callbacks.Size?       Margin          { get; init; }
            public Callbacks.Size?       Padding         { get; init; }
            public Callbacks.Color?      BackgroundColor { get; init; }
            public Callbacks.Color?      ForegroundColor { get; init; }
            public Callbacks.Pixels?     Height          { get; init; }
            public Callbacks.Pixels?     HeightMax       { get; init; }
            public Callbacks.Pixels?     Width           { get; init; }
            public Callbacks.Pixels?     WidthMax        { get; init; }
            public Callbacks.REM?        FontSize        { get; init; }
            public Callbacks.FontWeight? FontWeight      { get; init; }
            public Callbacks.Display?    Display         { get; init; }
            public Callbacks.Data?       Data            { get; init; }
            public Callbacks.Tooltip?    Tooltip         { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
                ElementService  = ElementService,
                IsVisible       = IsVisible,
                IsDisabled      = IsDisabled,
                IsRequired      = IsRequired,
                Classes         = Classes,
                Margin          = Margin,
                Padding         = Padding,
                BackgroundColor = BackgroundColor,
                ForegroundColor = ForegroundColor,
                Height          = Height,
                HeightMax       = HeightMax,
                Width           = Width,
                WidthMax        = WidthMax,
                FontSize        = FontSize,
                FontWeight      = FontWeight,
                Display         = Display,
                Data            = Data,
                Tooltip         = Tooltip,
            };
        }
    }

    public partial class TextInput
    {
        private readonly Callbacks.Callback<string>? _placeholder;

        public TextInput
        (
            IJSRuntime jsRuntime,
            string?    value,
            Spec?      spec = null
        )
            : base(jsRuntime, spec?.ToBaseSpec(), new ClassSet("I4E-Input", "I4E-Input-Text"))
        {
            _placeholder = spec?.Placeholder;

            Value = Nullify(value);
        }

        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                Console.WriteLine("RENDER");
                var seq = -1;

                builder.OpenElement(++seq, "div");
                InputBuilder.ApplyOuterAttributes(this, builder, ref seq, null);

                builder.OpenElement(++seq, "input");
                InputBuilder.ApplyInnerAttributes(this, builder, ref seq, null);

                builder.AddAttribute(++seq, "type", "text");
                builder.AddAttribute(++seq, "value", Serialize(Value));
                builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));
                builder.AddAttribute(++seq, "disabled", BaseSpec.IsDisabled?.Invoke());
                builder.AddAttribute(++seq, "required", BaseSpec.IsRequired?.Invoke());
                builder.AddAttribute(++seq, "placeholder", _placeholder?.Invoke());

                builder.AddElementReferenceCapture(++seq, r => Reference = r);
                builder.CloseElement();

                builder.CloseElement();
            }

            return Fragment;
        }

        protected override        string  Serialize(string?   v) => v ?? "";
        protected override        string? Deserialize(string? v) => string.IsNullOrEmpty(v) ? null : v;
        protected sealed override string? Nullify(string?     v) => string.IsNullOrEmpty(v) ? null : v;

        private void Change(ChangeEventArgs args) => InvokeOnChange(Deserialize(args.Value?.ToString()));
    }
}