using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public partial class TimeInput : StandardInput<DateTime?>
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec : IDualSpec
        {
            internal static readonly Spec Default = new();

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
            public Callbacks.REM?        FontSize        { get; init; }
            public Callbacks.Scale?      Scale           { get; init; }
            public Callbacks.FontWeight? FontWeight      { get; init; }
            public Callbacks.Display?    Display         { get; init; }
            public Callbacks.Data?       Data            { get; init; }
            public Callbacks.Tooltip?    Tooltip         { get; init; }

            public SpecSet ToOuterSpec() => new()
            {
                BaseClasses     = new ClassSet("I4E-Input", "I4E-Input-Time"),
                Scaled          = Scale != null,
                IsVisible       = IsVisible,
                Classes         = Classes,
                BackgroundColor = BackgroundColor,
                ForegroundColor = ForegroundColor,
                FontSize        = FontSize,
                Scale           = Scale,
                Display         = Display,
                Data            = Data,
                Tooltip         = Tooltip,
            };

            public SpecSet ToInnerSpec() => new()
            {
                IsDisabled = IsDisabled,
                IsRequired = IsRequired,
                Margin     = Margin,
                Padding    = Padding,
                Height     = Height,
                HeightMax  = HeightMax,
                Width      = Width,
                WidthMax   = WidthMax,
                FontWeight = FontWeight,
            };
        }
    }

    public partial class TimeInput
    {
        public TimeInput
        (
            IJSRuntime jsRuntime,
            DateTime?  value,
            Spec?      spec = null
        )
            : base(jsRuntime, spec ?? Spec.Default)
        {
            Value = Nullify(value);
        }

        public override RenderFragment Renderer() => Latch.Create(builder =>
        {
            var seq = -1;

            builder.OpenElement(++seq, "div");
            InputBuilder.ApplyOuterAttributes(this, builder, ref seq);

            builder.OpenElement(++seq, "input");
            InputBuilder.ApplyInnerAttributes(this, builder, ref seq);

            builder.AddAttribute(++seq, "type",    "time");
            builder.AddAttribute(++seq, "value",   Serialize(Value));
            builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));

            builder.AddElementReferenceCapture(++seq, r => Reference = r);
            builder.CloseElement();

            builder.CloseElement();

            InputBuilder.ScheduleElementJobs(this, builder, ref seq);
        }, v => Refresher = v);

        private void Change(ChangeEventArgs args) => InvokeOnChange(Deserialize(args.Value?.ToString()));

        protected override string Serialize(DateTime? v) => v?.ToString("HH:mm") ?? "";

        protected override DateTime? Deserialize(string? v) =>
            string.IsNullOrEmpty(v)
                ? null
                : DateTime.ParseExact(v, v.Split(':').Length == 2
                        ? "HH:mm"
                        : "HH:mm:ss",
                    new DateTimeFormatInfo());

        protected sealed override DateTime? Nullify(DateTime? v) =>
            v == null || v.Value == DateTime.MinValue
                ? null
                : v.Value;
    }
}