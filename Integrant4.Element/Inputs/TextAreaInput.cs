using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public partial class TextAreaInput : StandardInput<string?>
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec : IDualSpec
        {
            internal static readonly Spec Default = new();

            public Callbacks.Callback<string>? Placeholder { get; init; }
            public Callbacks.Callback<int>?    Rows        { get; init; }
            public Callbacks.Callback<int>?    Columns     { get; init; }

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

            public SpecSet ToOuterSpec() => new()
            {
                BaseClasses     = new ClassSet("I4E-Input", "I4E-Input-TextArea"),
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

            public SpecSet ToInnerSpec() => new()
            {
                IsDisabled = IsDisabled,
                IsRequired = IsRequired,
                FontWeight = FontWeight,
            };
        }
    }

    public partial class TextAreaInput
    {
        private readonly Callbacks.Callback<string>? _placeholder;
        private readonly Callbacks.Callback<int>?    _rows;
        private readonly Callbacks.Callback<int>?    _columns;

        public TextAreaInput
        (
            IJSRuntime jsRuntime,
            string?    value,
            Spec?      spec = null
        )
            : base(jsRuntime, spec ?? Spec.Default)
        {
            _placeholder = spec?.Placeholder;
            _rows        = spec?.Rows;
            _columns     = spec?.Columns;

            Value = Nullify(value);
        }

        public override RenderFragment Renderer() => Latch.Create(builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            InputBuilder.ApplyOuterAttributes(this, builder, ref seq);

            builder.OpenElement(++seq, "textarea");
            InputBuilder.ApplyInnerAttributes(this, builder, ref seq);

            builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));
            builder.AddAttribute(++seq, "placeholder", _placeholder?.Invoke());

            if (_rows    != null) builder.AddAttribute(++seq, "rows", _rows.Invoke());
            if (_columns != null) builder.AddAttribute(++seq, "cols", _columns.Invoke());

            builder.AddContent(++seq, Serialize(Value));

            builder.AddElementReferenceCapture(++seq, r => Reference = r);
            builder.CloseElement();

            builder.CloseElement();

            InputBuilder.ScheduleElementJobs(this, builder, ref seq);
        }, v => Refresher = v);

        protected override        string  Serialize(string?   v) => v ?? "";
        protected override        string? Deserialize(string? v) => string.IsNullOrEmpty(v) ? null : v;
        protected sealed override string? Nullify(string?     v) => string.IsNullOrEmpty(v) ? null : v;

        private void Change(ChangeEventArgs args) => InvokeOnChange(Deserialize(args.Value?.ToString()));
    }
}