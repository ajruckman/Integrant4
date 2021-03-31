using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public partial class DecimalInput : NumberInput<decimal?>
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public Callbacks.Callback<bool>?    Consider0Null { get; init; }
            public Callbacks.Callback<decimal>? Min           { get; init; }
            public Callbacks.Callback<decimal>? Max           { get; init; }
            public Callbacks.Callback<decimal>? Step          { get; init; }

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
            public Callbacks.Scale?      Scale           { get; init; }
            public Callbacks.FontWeight? FontWeight      { get; init; }
            public Callbacks.Display?    Display         { get; init; }
            public Callbacks.Data?       Data            { get; init; }
            public Callbacks.Tooltip?    Tooltip         { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
                Scaled = true,

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
                Scale           = Scale,
                FontWeight      = FontWeight,
                Display         = Display,
                Data            = Data,
                Tooltip         = Tooltip,
            };
        }
    }

    public partial class DecimalInput
    {
        private readonly Callbacks.Callback<bool>     _consider0Null;
        private readonly Callbacks.Callback<decimal>? _min;
        private readonly Callbacks.Callback<decimal>? _max;
        private readonly Callbacks.Callback<decimal>  _step;

        public DecimalInput
        (
            IJSRuntime jsRuntime,
            decimal?   value,
            Spec?      spec = null
        )
            : base(jsRuntime, new ClassSet("I4E-Input", "I4E-Input-Decimal"), spec?.ToBaseSpec())
        {
            _consider0Null = spec?.Consider0Null ?? (() => false);
            _min           = spec?.Min;
            _max           = spec?.Max;
            _step          = spec?.Step ?? (() => 0.1m);

            Value = Nullify(value);
        }

        public override RenderFragment Renderer() => RefreshWrapper.Create(builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            InputBuilder.ApplyOuterAttributes(this, builder, ref seq, null);

            builder.OpenElement(++seq, "input");
            InputBuilder.ApplyInnerAttributes(this, builder, ref seq, null);

            builder.AddAttribute(++seq, "type", "number");
            builder.AddAttribute(++seq, "value", Serialize(Value));
            builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));

            if (_min != null) builder.AddAttribute(++seq, "min", _min.Invoke());
            if (_max != null) builder.AddAttribute(++seq, "max", _max.Invoke());
            builder.AddAttribute(++seq, "step", _step.Invoke());

            builder.AddElementReferenceCapture(++seq, r => Reference = r);
            builder.CloseElement();

            builder.CloseElement();

            InputBuilder.ScheduleElementJobs(this, builder, ref seq);
        }, v => Refresher = v);

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