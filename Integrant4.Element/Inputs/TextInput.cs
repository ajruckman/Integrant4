using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public partial class TextInput : StandardInput<string?>
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec : DualSpec
        {
            internal static readonly Spec Default = new();

            public Callbacks.Callback<string>? Placeholder { get; init; }
            public Callbacks.Callback<bool>?   IsClearable { get; init; }

            public Callbacks.IsVisible?  IsVisible       { get; init; }
            public Callbacks.IsDisabled? IsDisabled      { get; init; }
            public Callbacks.IsRequired? IsRequired      { get; init; }
            public Callbacks.Classes?    Classes         { get; init; }
            public Callbacks.Size?       Margin          { get; init; }
            public Callbacks.Size?       Padding         { get; init; }
            public Callbacks.Color?      BackgroundColor { get; init; }
            public Callbacks.Color?      ForegroundColor { get; init; }
            public Callbacks.Color?      HighlightColor  { get; init; }
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
                BaseClasses     = new ClassSet("I4E-Input", "I4E-Input-Text"),
                Scaled          = true,
                IsVisible       = IsVisible,
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

    public partial class TextInput
    {
        private readonly Callbacks.Callback<string>? _placeholder;
        private readonly Callbacks.Callback<bool>?   _isClearable;

        public TextInput
        (
            IJSRuntime jsRuntime,
            string?    value,
            Spec?      spec = null
        )
            : base(jsRuntime, spec ?? Spec.Default)
        {
            _placeholder = spec?.Placeholder;
            _isClearable = spec?.IsClearable;

            Value = Nullify(value);
        }

        public override RenderFragment Renderer() => Latch.Create(builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            InputBuilder.ApplyOuterAttributes(this, builder, ref seq,
                _isClearable?.Invoke() == true
                    ? new[] { "I4E-Input-Text--Clearable" }
                    : null
            );

            builder.OpenElement(++seq, "input");
            InputBuilder.ApplyInputAttributes(this, builder, ref seq);

            builder.AddAttribute(++seq, "type", "text");
            builder.AddAttribute(++seq, "value", Serialize(Value));
            builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));
            builder.AddAttribute(++seq, "placeholder", _placeholder?.Invoke());

            builder.AddElementReferenceCapture(++seq, r => Reference = r);
            builder.CloseElement();

            if (_isClearable?.Invoke() == true)
            {
                ushort  size  = 16;
                double? scale = OuterSpec!.Scale?.Invoke();
                if (scale != null)
                    size = (ushort)(size * scale);

                builder.OpenElement(++seq, "button");
                builder.AddAttribute(++seq, "class", "I4E-Input-Text-ClearButton");
                builder.AddAttribute(++seq, "onclick", EventCallback.Factory.Create(this, OnClearClick));
                ++seq;
                if (scale != null)
                    builder.AddAttribute(seq, "style", $"font-size: {scale}rem;");

                builder.OpenComponent<BootstrapIcon>(++seq);
                builder.AddAttribute(++seq, "ID", "x-circle-fill");

                builder.AddAttribute(++seq, "Size", size);

                builder.CloseComponent();
                builder.CloseElement();
            }

            builder.CloseElement();

            InputBuilder.ScheduleElementJobs(this, builder, ref seq);
        }, v => Refresher = v);

        private async Task OnClearClick()
        {
            if (_isClearable?.Invoke() == true)
                await SetValue(null);
        }

        protected override        string  Serialize(string?   v) => v ?? "";
        protected override        string? Deserialize(string? v) => string.IsNullOrEmpty(v) ? null : v;
        protected sealed override string? Nullify(string?     v) => string.IsNullOrEmpty(v) ? null : v;

        private void Change(ChangeEventArgs args) => InvokeOnChange(Deserialize(args.Value?.ToString()));
    }
}