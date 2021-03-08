using System;
using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public partial class DecimalInput : NumberInput<decimal?>
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public ElementService? ElementService { get; init; }

            public Callbacks.Callback<bool>?   Consider0Null { get; init; }
            public Callbacks.Callback<string>? Step          { get; init; }

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

    public partial class DecimalInput
    {
        private readonly Callbacks.Callback<bool> _consider0Null;
        private readonly Callbacks.Callback<string> _step;

        public DecimalInput
        (
            IJSRuntime jsRuntime,
            decimal?   value,
            Spec?      spec = null
        )
            : base(jsRuntime, new ClassSet("I4E-Input", "I4E-Input-Decimal"), spec?.ToBaseSpec())
        {
            _consider0Null = spec?.Consider0Null ?? (() => false);
            _step          = spec?.Step          ?? (() => "0.1");

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

                builder.AddAttribute(++seq, "value", Serialize(Value));
                builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));
                builder.AddAttribute(++seq, "step", _step.Invoke());

                builder.AddElementReferenceCapture(++seq, r => Reference = r);
                builder.CloseElement();

                builder.CloseElement();
            }

            return Fragment;
        }

        protected override decimal? Deserialize(string? v) =>
            string.IsNullOrEmpty(v)
                ? null
                : decimal.Parse(v);

        protected sealed override decimal? Nullify(decimal? v) =>
            v == null
                ? null
                : _consider0Null.Invoke() && v == 0
                    ? null
                    : v;
    }
}