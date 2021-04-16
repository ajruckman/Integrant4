using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public partial class CheckboxInput : InputBase<bool>
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
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

    public partial class CheckboxInput
    {
        private readonly IJSRuntime _jsRuntime;

        private bool             _value;
        private ElementReference _reference;

        public CheckboxInput
        (
            IJSRuntime jsRuntime,
            bool       value,
            Spec?      spec = null
        )
            : base(jsRuntime, spec?.ToBaseSpec(), new ClassSet("I4E-Input", "I4E-Input-Checkbox"))
        {
            _jsRuntime = jsRuntime;
            _value     = value;
        }

        public override RenderFragment Renderer() => Latch.Create(builder =>
        {
            var seq = -1;

            builder.OpenElement(++seq, "div");
            InputBuilder.ApplyOuterAttributes(this, builder, ref seq, null);

            builder.OpenElement(++seq, "input");
            InputBuilder.ApplyInnerAttributes(this, builder, ref seq, null);

            builder.AddAttribute(++seq, "type",    "checkbox");
            builder.AddAttribute(++seq, "checked", _value);
            builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));

            builder.AddElementReferenceCapture(++seq, r => _reference = r);
            builder.CloseElement();

            builder.CloseElement();

            InputBuilder.ScheduleElementJobs(this, builder, ref seq);
        }, v => Refresher = v);

        public override async Task<bool> GetValue()
        {
            return await _jsRuntime.InvokeAsync<bool>("window.I4.Element.Inputs.GetChecked", _reference);
        }

        public override async Task SetValue(bool value)
        {
            _value = value;
            await _jsRuntime.InvokeVoidAsync("window.I4.Element.Inputs.SetChecked", _reference, _value);
            OnChange?.Invoke(_value);
        }

        public override event Action<bool>? OnChange;

        private void Change(ChangeEventArgs args) => OnChange?.Invoke(Deserialize(args.Value?.ToString()));

        private bool Deserialize(string? v) =>
            v switch
            {
                "False" => false,
                "True"  => true,
                _       => throw new ArgumentOutOfRangeException(),
            };
    }
}