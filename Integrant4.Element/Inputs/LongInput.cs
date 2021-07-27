using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public partial class LongInput : NumberInput<long?>
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec : DualSpec
        {
            public Callbacks.Callback<bool>? Consider0Null { get; init; }
            public Callbacks.Callback<long>? Min           { get; init; }
            public Callbacks.Callback<long>? Max           { get; init; }

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
                BaseClasses = new ClassSet("I4E-Input", "I4E-Input-Long"),
                Scaled      = true,
                IsVisible   = IsVisible,
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

            internal override SpecSet ToInnerSpec() => new()
            {
                IsDisabled = IsDisabled,
                IsRequired = IsRequired,
            };
        }
    }

    public partial class LongInput
    {
        private readonly Callbacks.Callback<bool>  _consider0Null;
        private readonly Callbacks.Callback<long>? _min;
        private readonly Callbacks.Callback<long>? _max;

        public LongInput
        (
            IJSRuntime jsRuntime,
            long?      value,
            Spec?      spec = null
        )
            : base(jsRuntime, spec)
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
            InputBuilder.ApplyOuterAttributes(this, builder, ref seq);

            builder.OpenElement(++seq, "input");
            InputBuilder.ApplyInputAttributes(this, builder, ref seq);

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

        protected override long? Deserialize(string? v)
        {
            if (string.IsNullOrEmpty(v))
                return null;

            long i = long.Parse(v);

            long? min = _min?.Invoke();
            if (i < min)
                i = min.Value;

            long? max = _max?.Invoke();
            if (i > max)
                i = max.Value;

            return i;
        }

        protected sealed override long? Nullify(long? v) =>
            v == null
                ? null
                : _consider0Null.Invoke() && v == 0
                    ? null
                    : v;
    }
}