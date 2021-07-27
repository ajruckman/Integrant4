using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public partial class DateInput : StandardInput<DateTime?>
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec : DualSpec
        {
            internal static readonly Spec Default = new();

            public Callbacks.Callback<DateTime>? Min { get; init; }
            public Callbacks.Callback<DateTime>? Max { get; init; }

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
                BaseClasses     = new ClassSet("I4E-Input", "I4E-Input-Date"),
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

    public partial class DateInput
    {
        private readonly Callbacks.Callback<DateTime>? _min;
        private readonly Callbacks.Callback<DateTime>? _max;

        public DateInput
        (
            IJSRuntime jsRuntime,
            DateTime?  value,
            Spec?      spec = null
        )
            : base(jsRuntime, spec ?? Spec.Default)
        {
            _min = spec?.Min;
            _max = spec?.Max;

            Value = Nullify(value);
        }

        public override RenderFragment Renderer() => Latch.Create(builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            InputBuilder.ApplyOuterAttributes(this, builder, ref seq);

            builder.OpenElement(++seq, "input");
            InputBuilder.ApplyInputAttributes(this, builder, ref seq);

            builder.AddAttribute(++seq, "type",    "date");
            builder.AddAttribute(++seq, "value",   Serialize(Value));
            builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));

            if (_min != null) builder.AddAttribute(++seq, "min", Serialize(_min.Invoke()));
            if (_max != null) builder.AddAttribute(++seq, "max", Serialize(_max.Invoke()));

            builder.AddElementReferenceCapture(++seq, r => Reference = r);
            builder.CloseElement();

            builder.CloseElement();

            InputBuilder.ScheduleElementJobs(this, builder, ref seq);
        }, v => Refresher = v);

        private void Change(ChangeEventArgs args) => InvokeOnChange(Deserialize(args.Value?.ToString()));

        protected override string Serialize(DateTime? v) => v?.ToString("yyyy-MM-dd") ?? "";

        protected override DateTime? Deserialize(string? v)
        {
            if (string.IsNullOrEmpty(v))
                return null;

            DateTime d = DateTime.ParseExact(v, "yyyy-MM-dd", new DateTimeFormatInfo());

            DateTime? min = _min?.Invoke();
            if (d < min)
                d = min.Value;

            DateTime? max = _max?.Invoke();
            if (d > max)
                d = max.Value;

            return d;
        }

        protected sealed override DateTime? Nullify(DateTime? v) =>
            v == null || v.Value == DateTime.MinValue
                ? null
                : v.Value;
    }
}