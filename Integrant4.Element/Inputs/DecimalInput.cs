using System;
using System.Threading.Tasks;
using Integrant4.Element.Inputs.Managers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Integrant4.Element.Inputs
{
    public abstract class NumberInput<T> : InputBase<T>, IInputRequirable, IInputDisableable
    {
        protected readonly InputDisabledManager InputDisabledManager;
        protected readonly InputRequiredManager InputRequiredManager;

        protected NumberInput(IJSRuntime jsRuntime, T value, bool disabled, bool required) : base(jsRuntime, value)
        {
            InputDisabledManager = new InputDisabledManager(jsRuntime, disabled);
            InputRequiredManager = new InputRequiredManager(jsRuntime, required);
        }

        // public abstract RenderFragment Render();

        public       Task<bool> IsDisabled() => Task.FromResult(InputDisabledManager.IsDisabled());
        public async Task       Disable()    => await InputDisabledManager.Disable(Reference);
        public async Task       Enable()     => await InputDisabledManager.Enable(Reference);

        public       Task<bool> IsRequired() => Task.FromResult(InputRequiredManager.IsRequired());
        public async Task       Require()    => await InputRequiredManager.Require(Reference);
        public async Task       Unrequire()  => await InputRequiredManager.Unrequire(Reference);

        protected override string Serialize(T? v) => v?.ToString() ?? "";

        protected void Change(ChangeEventArgs args) => InvokeOnChange(Deserialize(args.Value?.ToString()));
    }

    public class IntegerInput : NumberInput<int?>
    {
        private readonly bool _consider0Null;

        public IntegerInput
        (
            IJSRuntime jsRuntime, int? value,
            bool       disabled,  bool required,
            bool       consider0Null = false
        )
            : base(jsRuntime, value, disabled, required)
        {
            _consider0Null = consider0Null;
        }

        public override RenderFragment Render()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                Console.WriteLine("RENDER");
                var seq = -1;

                builder.OpenElement(++seq, "input");
                builder.AddAttribute(++seq, "type", "number");

                builder.AddAttribute(++seq, "value", Serialize(Value));
                builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));
                builder.AddAttribute(++seq, "disabled", InputDisabledManager.IsDisabled());
                builder.AddAttribute(++seq, "required", InputRequiredManager.IsRequired());

                builder.AddElementReferenceCapture(++seq, r => Reference = r);
                builder.CloseElement();
            }

            return Fragment;
        }

        protected override int? Deserialize(string? v) =>
            string.IsNullOrEmpty(v)
                ? null
                : int.Parse(v);

        protected override int? Nullify(int? v) =>
            v == null
                ? null
                : _consider0Null && v == 0
                    ? null
                    : v;
    }

    public class DecimalInput : NumberInput<decimal?>
    {
        private readonly string? _step;
        private readonly bool    _consider0Null;

        public DecimalInput
        (
            IJSRuntime jsRuntime,             decimal? value,
            bool       disabled,              bool     required,
            bool       consider0Null = false, string   step = "0.1"
        )
            : base(jsRuntime, value, disabled, required)
        {
            _consider0Null = consider0Null;
            _step          = step;
        }

        public override RenderFragment Render()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                Console.WriteLine("RENDER");
                var seq = -1;

                builder.OpenElement(++seq, "input");
                builder.AddAttribute(++seq, "type", "number");

                ++seq;
                if (_step != null) builder.AddAttribute(seq, "step", _step);

                ++seq;
                if (Value != null)
                {
                    builder.AddAttribute(seq, "value", Serialize(Value));
                }
                else
                {
                    JSRuntime.InvokeVoidAsync("window.I4.Element.Inputs.SetIndeterminate", true);
                }

                builder.AddAttribute(++seq, "oninput", EventCallback.Factory.Create(this, Change));
                builder.AddAttribute(++seq, "disabled", InputDisabledManager.IsDisabled());
                builder.AddAttribute(++seq, "required", InputRequiredManager.IsRequired());

                builder.AddElementReferenceCapture(++seq, r => Reference = r);
                builder.CloseElement();
            }

            return Fragment;
        }

        protected override decimal? Deserialize(string? v) =>
            string.IsNullOrEmpty(v)
                ? null
                : decimal.Parse(v);

        protected override decimal? Nullify(decimal? v) =>
            v == null
                ? null
                : _consider0Null && v == 0
                    ? null
                    : v;
    }
}