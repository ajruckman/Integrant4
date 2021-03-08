using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element;
using Integrant4.Element.Bits;
using Integrant4.Element.Constructs;
using Integrant4.Element.Inputs;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Web.Pages
{
    public partial class Elements
    {
        private readonly List<Button> _buttonsColored      = new();
        private          IntegerInput _intInput            = null!;
        private          IntegerInput _intInput0Null       = null!;
        private          DecimalInput _decimalInput        = null!;
        private          DecimalInput _decimalInput0Null   = null!;
        private          DecimalInput _decimalInputStepped = null!;

        private Button _buttonNoIcon    = null!;
        private Button _buttonOnlyIcon  = null!;
        private Button _buttonIconFirst = null!;
        private Button _buttonIconLast  = null!;
        private Button _buttonIconAll   = null!;

        private Chip _chip     = null!;
        private Chip _chipLink = null!;

        private Checkbox _checkbox = null!;

        private CheckboxInput _checkboxInput = null!;

        private Link _link1 = null!;
        private Link _link2 = null!;
        private Link _link3 = null!;
        private Link _link4 = null!;

        private Dropdown _dropdown1 = null!;

        private Header _header1 = null!;

        private bool _checked = true;

        [Inject] public IJSRuntime     JSRuntime      { get; set; } = null!;
        [Inject] public ElementService ElementService { get; set; } = null!;

        protected override void OnInitialized()
        {
            _intInput = new IntegerInput(JSRuntime, 0);
            _intInput0Null = new IntegerInput(JSRuntime, 0,
                new IntegerInput.Spec {Consider0Null = Always.True});
            _decimalInput = new DecimalInput(JSRuntime, (decimal) 0.0);
            _decimalInput0Null = new DecimalInput(JSRuntime, (decimal) 0.0,
                new DecimalInput.Spec {Consider0Null = Always.True});
            _decimalInputStepped =
                new DecimalInput(JSRuntime, (decimal) 0.0, new DecimalInput.Spec {Step = () => "0.01"});

            void PrintI(int?     v) => Console.WriteLine($"int -> {v}");
            void PrintD(decimal? v) => Console.WriteLine($"decimal -> {v}");

            _intInput.OnChange            += PrintI;
            _intInput0Null.OnChange       += PrintI;
            _decimalInput.OnChange        += PrintD;
            _decimalInput0Null.OnChange   += PrintD;
            _decimalInputStepped.OnChange += PrintD;

            //

            _checkboxInput = new CheckboxInput(JSRuntime, false, new CheckboxInput.Spec {IsRequired = Always.True});

            void PrintB(bool v) => Console.WriteLine($"bool -> {v}");

            _checkboxInput.OnChange += PrintB;

            //

            _buttonNoIcon   = new Button(() => "asdf".AsContent());
            _buttonOnlyIcon = new Button(() => new BootstrapIcon("caret-right-fill", 16));
            _buttonIconFirst = new Button(() => new IRenderable[]
            {
                new BootstrapIcon("caret-left-fill", 16),
                "asdf left".AsContent(),
            });
            _buttonIconLast = new Button(() => new IRenderable[]
            {
                "asdf right".AsContent(),
                new BootstrapIcon("caret-right-fill", 16),
            });
            _buttonIconAll = new Button(() => new IRenderable[]
            {
                new BootstrapIcon("caret-left-fill", 16),
                new BootstrapIcon("caret-right-fill", 16),
            });

            foreach (Button.Style style in Enum.GetValues<Button.Style>())
            {
                var b = new Button(() => new IRenderable[]
                    {
                        ("Color: " + style).AsContent(),
                        new BootstrapIcon("caret-down-fill", 16),
                    },
                    new Button.Spec
                    {
                        ElementService = ElementService,
                        Style          = () => style,
                        Tooltip        = () => $"{style} {_checked}",
                    });

                // b.OnActivate += () => Console.WriteLine($"Activate: {style}");
                b.OnClick += _ => Console.WriteLine($"Click: {style}");
                // b.OnKeyUp += _ => Console.WriteLine($"KeyPress: {style}");

                _buttonsColored.Add(b);
            }

            // Chip c = new(new Chip.Spec
            // (
            //     () => "asdf"
            // )
            // {
            //     PixelsWidth = () => 24,
            // });

            //

            _chip = new Chip(() => "Chip 1".AsContent(), new Chip.Spec
            {
                Height = () => 24,
            });

            _chipLink = new Chip(() => "Chip 1".AsContent(), new Chip.Spec
            {
                HREF   = () => "/",
                Height = () => 24,
            });

            _checkbox = new Checkbox(new Checkbox.Spec
            {
                IsChecked  = () => _checked,
                IsDisabled = () => _checked,
            });
            _checkbox.OnToggle += PrintB;

            _link1 = new Link(() => "Link 1".AsContent(), new Link.Spec(() => "/"));
            _link2 = new Link(() => "Link 2".AsContent(), new Link.Spec(() => "/")
            {
                FontWeight = () => FontWeight.SemiBold,
            });
            _link3 = new Link(() => "Link 3".AsContent(), new Link.Spec(() => "/")
            {
                IsAccented = Always.True,
            });
            _link4 = new Link(() => "Link 4 (highlighted)".AsContent(), new Link.Spec(() => "/")
            {
                IsHighlighted = Always.True,
            });

            _dropdown1 = new Dropdown
            (
                () => new IRenderable[]
                {
                    new Link(() => new IRenderable[]
                    {
                        "Dropdown 1".AsContent(),
                        new BootstrapIcon("chevron-down"),
                    }, new Link.Spec(() => "/elements")),
                },
                () => new IRenderable[]
                {
                    "asdf".AsContent(),
                    new HorizontalLine(),
                    "asdf".AsContent(),
                    new Link(() => new IRenderable[]
                    {
                        new BootstrapIcon("gear"),
                        "Settings".AsContent(),
                    }, new Link.Spec(() => "/") {IsButton = Always.True}),
                    new Link(() => new IRenderable[]
                    {
                        new BootstrapIcon("gear-fill"),
                        "Settings 2".AsContent(),
                    }, new Link.Spec(() => "/") {IsButton = Always.True}),
                    new HorizontalLine(),
                    new Dropdown
                    (
                        () => new IRenderable[]
                        {
                            new Link(() => new IRenderable[]
                            {
                                "Dropdown 1".AsContent(),
                                new BootstrapIcon("chevron-down"),
                            }, new Link.Spec(() => "/elements")),
                        },
                        () => new IRenderable[]
                        {
                            "asdf".AsContent(),
                            new HorizontalLine(),
                            "asdf".AsContent(),
                            new HorizontalLine(),
                            new Link(() => new IRenderable[]
                            {
                                new BootstrapIcon("chevron-right"),
                                "Chevron".AsContent(),
                            }, new Link.Spec(() => "/") {IsButton = Always.True}),
                        }, new Dropdown.Spec(ElementService) {PlacementGetter = () => Dropdown.Placement.RightStart}),
                }, new Dropdown.Spec(ElementService));

            _header1 = new Header(() => new IRenderable[]
            {
                new Title(() => new IRenderable[]
                {
                    "Integrant 4".AsContent(),
                }, new Title.Spec(() => "/")),
                new Filler(),
                new PageLink(() => new IRenderable[]
                {
                    "Elements".AsContent(),
                }, new PageLink.Spec(() => "/elements")),
                new VerticalLine(),
                new PageLink(() => new IRenderable[]
                {
                    "Google".AsContent(),
                }, new PageLink.Spec(() => "https://google.com")),
                new VerticalLine(),
                new PageLink(() => new IRenderable[]
                {
                    "Google".AsContent(),
                }, new PageLink.Spec(() => "https://google.com")),
                new VerticalLine(),
                new PageLink(() => new IRenderable[]
                {
                    "Google".AsContent(),
                }, new PageLink.Spec(() => "https://google.com") {IsHighlighted = Always.True}),
                new VerticalLine(),
                new PageLink(() => new IRenderable[]
                {
                    "Google".AsContent(),
                }, new PageLink.Spec(() => "https://google.com")),
                new VerticalLine(),
                new PageLink(() => new IRenderable[]
                {
                    "Google".AsContent(),
                }, new PageLink.Spec(() => "https://google.com")),
                new VerticalLine(),
                new Dropdown
                (
                    () => new IRenderable[]
                    {
                        new PageLink(() => new IRenderable[]
                        {
                            "Dropdown 1".AsContent(),
                            new BootstrapIcon("chevron-down"),
                        }, new PageLink.Spec(() => "/elements")),
                    },
                    () => new IRenderable[]
                    {
                        new TextBlock(() => new IRenderable[] {"Content".AsContent()}),
                        new HorizontalLine(),
                        new TextBlock(() => new IRenderable[] {"Content 2".AsContent()}),
                    }, new Dropdown.Spec(ElementService)
                ),
            });
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var v = await _intInput.GetValue();
            Console.WriteLine(v);

            if (firstRender)
                Task.Run(() =>
                {
                    Thread.Sleep(2500);
                    _checked = false;
                    _checkbox.Reset();
                    InvokeAsync(StateHasChanged);
                });

            Console.WriteLine(await _checkboxInput.GetValue());

            // await JSRuntime.InvokeVoidAsync("window.LoadAllScrollbars");

            await ElementService.ProcessJobs();

            // await Interop.CreateBitTooltips(JSRuntime, _id);
        }
    }
}