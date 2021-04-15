using System.Diagnostics.CodeAnalysis;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public partial class TextInput : StandardInput<string?>
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public Callbacks.Callback<string>? Placeholder { get; init; }

            public Callbacks.IsVisible?  IsVisible       { get; init; }
            public Callbacks.IsDisabled? IsDisabled      { get; init; }
            public Callbacks.IsRequired? IsRequired      { get; init; }
            public Callbacks.Classes?    Classes         { get; init; }
            public Callbacks.Size?       Margin          { get; init; }
            public Callbacks.Size?       Padding         { get; init; }
            public Callbacks.Color?      BackgroundColor { get; init; }
            public Callbacks.Color?      ForegroundColor { get; init; }
            public Callbacks.Color?      HighlightColor  { get; init; }
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
                HighlightColor  = HighlightColor,
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

        public override RenderFragment Renderer() => RefreshWrapper.Create(builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            InputBuilder.ApplyOuterAttributes(this, builder, ref seq, null);

            builder.OpenElement(++seq, "input");
            InputBuilder.ApplyInnerAttributes(this, builder, ref seq, null);

            builder.AddAttribute(++seq, "type",        "text");
            builder.AddAttribute(++seq, "value",       Serialize(Value));
            builder.AddAttribute(++seq, "oninput",     EventCallback.Factory.Create(this, Change));
            builder.AddAttribute(++seq, "placeholder", _placeholder?.Invoke());

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