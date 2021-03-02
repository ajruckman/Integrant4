using System;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element.Bits;
using Integrant4.Element.Inputs;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Web.Pages
{
    public partial class Elements
    {
        private IntegerInput _intInput            = null!;
        private IntegerInput _intInput0Null       = null!;
        private DecimalInput _decimalInput        = null!;
        private DecimalInput _decimalInput0Null   = null!;
        private DecimalInput _decimalInputStepped = null!;

        private Button _buttonNoIcon   = null!;
        private Button _buttonOnlyIcon = null!;
        private Button _buttonIconFirst = null!;
        private Button _buttonIconLast = null!;
        private Button _buttonIconAll = null!;

        private CheckboxInput _checkboxInput = null!;

        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;

        protected override void OnInitialized()
        {
            _intInput          = new IntegerInput(JSRuntime, 0, () => false, () => false);
            _intInput0Null     = new IntegerInput(JSRuntime, 0, () => false, () => false, () => true);
            _decimalInput      = new DecimalInput(JSRuntime, (decimal) 0.0, () => false, () => false);
            _decimalInput0Null = new DecimalInput(JSRuntime, (decimal) 0.0, () => false, () => false, () => true);
            _decimalInputStepped =
                new DecimalInput(JSRuntime, (decimal) 0.0, () => false, () => false, step: () => "0.01");

            void PrintI(int?     v) => Console.WriteLine($"int -> {v}");
            void PrintD(decimal? v) => Console.WriteLine($"decimal -> {v}");

            _intInput.OnChange            += PrintI;
            _intInput0Null.OnChange       += PrintI;
            _decimalInput.OnChange        += PrintD;
            _decimalInput0Null.OnChange   += PrintD;
            _decimalInputStepped.OnChange += PrintD;

            //

            _checkboxInput = new CheckboxInput(JSRuntime, false, () => false, () => true);

            void PrintB(bool v) => Console.WriteLine($"bool -> {v}");

            _checkboxInput.OnChange += PrintB;

            //

            _buttonNoIcon    = new Button(() => "asdf".AsContent());
            _buttonOnlyIcon  = new Button(() => new BootstrapIcon("chevron-right", 24));
            _buttonIconFirst = new Button(() => new IRenderable[]
            {
                new BootstrapIcon("chevron-left", 24),
                "asdf left".AsContent()
            });
            _buttonIconLast = new Button(() => new IRenderable[]
            {
                "asdf right".AsContent(),
                new BootstrapIcon("chevron-right", 24)
            });
            _buttonIconAll = new Button(() => new IRenderable[]
            {
                new BootstrapIcon("chevron-left", 24),
                new BootstrapIcon("chevron-right", 24)
            });

            // Chip c = new(new Chip.Spec
            // (
            //     () => "asdf"
            // )
            // {
            //     PixelsWidth = () => 24,
            // });

            //
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var v = await _intInput.GetValue();
            Console.WriteLine(v);
            
            Console.WriteLine(await _checkboxInput.GetValue());
        }
    }
}