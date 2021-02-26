using System;
using System.Threading.Tasks;
using Integrant4.Element.Inputs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Web.Pages
{
    public partial class Elements
    {
        private IntegerInput _intInput;
        private IntegerInput _intInput0Null;
        private DecimalInput _decimalInput;
        private DecimalInput _decimalInput0Null;
        private DecimalInput _decimalInputStepped;

        private CheckboxInput _checkboxInput;

        [Inject] public IJSRuntime JSRuntime { get; set; }

        protected override void OnInitialized()
        {
            _intInput            = new IntegerInput(JSRuntime, 0, false, false);
            _intInput0Null       = new IntegerInput(JSRuntime, 0, false, false, true);
            _decimalInput        = new DecimalInput(JSRuntime, (decimal) 0.0, false, false);
            _decimalInput0Null   = new DecimalInput(JSRuntime, (decimal) 0.0, false, false, true);
            _decimalInputStepped = new DecimalInput(JSRuntime, (decimal) 0.0, false, false, step: "0.01");

            void PrintI(int?     v) => Console.WriteLine($"int -> {v}");
            void PrintD(decimal? v) => Console.WriteLine($"decimal -> {v}");

            _intInput.OnChange            += PrintI;
            _intInput0Null.OnChange       += PrintI;
            _decimalInput.OnChange        += PrintD;
            _decimalInput0Null.OnChange   += PrintD;
            _decimalInputStepped.OnChange += PrintD;

            //

            _checkboxInput = new CheckboxInput(JSRuntime, false);

            void PrintB(bool v) => Console.WriteLine($"bool -> {v}");

            _checkboxInput.OnChange += PrintB;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var v = await _intInput.GetValue();
            Console.WriteLine(v);
        }
    }
}