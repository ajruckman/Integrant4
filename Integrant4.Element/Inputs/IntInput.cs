using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public partial class IntInput : NumberInput<int?>
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec : DualSpec
        {
            public Callbacks.Callback<bool>? Consider0Null { get; init; }
            public Callbacks.Callback<int>?  Min           { get; init; }
            public Callbacks.Callback<int>?  Max           { get; init; }

            public Callbacks.IsVisible?  IsVisible       { get; init; }
            public Callbacks.IsDisabled? IsDisabled      { get; init; }
            public Callbacks.IsRequired? IsRequired      { get; init; }
            public Callbacks.Classes?    Classes         { get; init; }
            public Callbacks.Size?       Margin          { get; init; }
            public Callbacks.Size?       Padding         { get; init; }
            public Callbacks.Color?      BackgroundColor { get; init; }
            public Callbacks.Color?      ForegroundColor { get; init; }
            public Callbacks.Unit?       Height          { get; init; }
            public Callbacks.Unit?       HeightMax       { get; init; }
            public Callbacks.Unit?       Width           { get; init; }
            public Callbacks.Unit?       WidthMax        { get; init; }
            public Callbacks.Scale?      Scale           { get; init; }
            public Callbacks.FontWeight? FontWeight      { get; init; }
            public Callbacks.Display?    Display         { get; init; }
            public Callbacks.Data?       Data            { get; init; }
            public Callbacks.Tooltip?    Tooltip         { get; init; }

            internal override SpecSet ToOuterSpec() => new()
            {
                BaseClasses     = new ClassSet("I4E-Input", "I4E-Input-Integer"),
                Scaled          = true,
                IsVisible       = IsVisible,
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
                Display         = Display,
                Data            = Data,
                Tooltip         = Tooltip,
            };

            internal override SpecSet ToInnerSpec() => new()
            {
                IsDisabled = IsDisabled,
                IsRequired = IsRequired,
                FontWeight = FontWeight,
            };
        }
    }

    public partial class IntInput
    {
        private readonly Callbacks.Callback<bool> _consider0Null;
        private readonly Callbacks.Callback<int>? _min;
        private readonly Callbacks.Callback<int>? _max;

        public IntInput
        (
            IJSRuntime jsRuntime,
            int?       value,
            Spec?      spec = null
        ) : base(jsRuntime, spec)
        {
            _consider0Null = spec?.Consider0Null ?? (() => false);
            _min           = spec?.Min;
            _max           = spec?.Max;

            Value = Nullify(value);
        }

        public override RenderFragment Renderer() => Latch.Create(builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            InputBuilder.ApplyOuterAttributes(this, builder, ref seq, null);

            builder.OpenElement(++seq, "input");
            InputBuilder.ApplyInputAttributes(this, builder, ref seq, null);

            builder.AddAttribute(++seq, "type",    "number");
            builder.AddAttribute(++seq, "value",   Serialize(Value));
            builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));

            if (_min != null) builder.AddAttribute(++seq, "min", _min.Invoke());
            if (_max != null) builder.AddAttribute(++seq, "max", _max.Invoke());

            builder.AddElementReferenceCapture(++seq, r => Reference = r);
            builder.CloseElement();

            builder.CloseElement();

            InputBuilder.ScheduleElementJobs(this, builder, ref seq);
        }, v => Refresher = v);

        protected override int? Deserialize(string? v)
        {
            if (string.IsNullOrEmpty(v))
                return null;

            int i = int.Parse(v);

            int? min = _min?.Invoke();
            if (i < min)
                i = min.Value;

            int? max = _max?.Invoke();
            if (i > max)
                i = max.Value;

            return i;
        }

        protected sealed override int? Nullify(int? v) =>
            v == null
                ? null
                : _consider0Null.Invoke() && v == 0
                    ? null
                    : v;
    }
}