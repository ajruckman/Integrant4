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
            public Callbacks.FontWeight? FontWeight      { get; init; }
            public Callbacks.Display?    Display         { get; init; }
            public Callbacks.Data?       Data            { get; init; }
            public Callbacks.Tooltip?    Tooltip         { get; init; }

            public SpecSet ToOuterSpec() => new()
            {
                BaseClasses     = new ClassSet("I4E-Input", "I4E-Input-Checkbox"),
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
                Display         = Display,
                Data            = Data,
                Tooltip         = Tooltip,
            };

            public SpecSet ToInnerSpec() => new()
            {
                IsDisabled = IsDisabled,
                IsRequired = IsRequired, FontSize = FontSize,
                FontWeight = FontWeight,
            };
        }
    }

    public partial class CheckboxInput
    {
        private readonly IJSRuntime _jsRuntime;

        private ElementReference _reference;

        public CheckboxInput
        (
            IJSRuntime jsRuntime,
            bool       value,
            Spec?      spec = null
        ) : base(jsRuntime, spec ?? Spec.Default)
        {
            _jsRuntime = jsRuntime;
            Value      = value;
        }

        public override RenderFragment Renderer() => Latch.Create(builder =>
        {
            int seq = -1;

            builder.OpenElement(++seq, "div");
            InputBuilder.ApplyOuterAttributes(this, builder, ref seq);

            builder.OpenElement(++seq, "input");
            InputBuilder.ApplyInnerAttributes(this, builder, ref seq);

            builder.AddAttribute(++seq, "type",    "checkbox");
            builder.AddAttribute(++seq, "checked", Value);
            builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));

            builder.AddElementReferenceCapture(++seq, r => _reference = r);
            builder.CloseElement();

            builder.CloseElement();

            InputBuilder.ScheduleElementJobs(this, builder, ref seq);
        }, v => Refresher = v);

        public override bool GetValue() => Value;

        public override async Task<bool> ReadValue() =>
            Value = await _jsRuntime.InvokeAsync<bool>("window.I4.Element.Inputs.GetChecked", _reference);

        public override async Task SetValue(bool value, bool invokeOnChange = true)
        {
            Value = value;
            await _jsRuntime.InvokeVoidAsync("window.I4.Element.Inputs.SetChecked", _reference, Value);

            if (invokeOnChange) OnChange?.Invoke(Value);
        }

        public override event Action<bool>? OnChange;

        private void Change(ChangeEventArgs args)
        {
            bool value = Deserialize(args.Value?.ToString());
            Value = value;
            OnChange?.Invoke(value);
        }

        private static bool Deserialize(string? v) =>
            v switch
            {
                "False" => false,
                "True"  => true,
                _       => throw new ArgumentOutOfRangeException(),
            };
    }
}