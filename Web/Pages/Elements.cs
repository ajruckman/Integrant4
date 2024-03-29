using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Colorant.Themes.Main;
using Integrant4.Element;
using Integrant4.Element.Bits;
using Integrant4.Element.Constructs;
using Integrant4.Element.Constructs.Headers;
using Integrant4.Element.Inputs;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Web.Pages
{
    public partial class Elements
    {
        private readonly List<Button> _buttonsColored = new();

        private readonly List<Button> _buttonsScaled = new();
        private readonly List<Chip>   _chipsScaled   = new();
        private readonly List<Link>   _linksScaled   = new();

        private SecondaryHeader _header             = null!;
        private ContentRef      _panelExpanderElems = null!;

        private TextInput _textInput                = null!;
        private TextInput _textInputHighlighted     = null!;
        private TextInput _textInputClearable       = null!;
        private TextInput _textInputClearableScaled = null!;

        private IntInput     _intInput            = null!;
        private IntInput     _intInput0Null       = null!;
        private DecimalInput _decimalInput        = null!;
        private DecimalInput _decimalInput0Null   = null!;
        private DecimalInput _decimalInputStepped = null!;

        private Button _buttonNoIcon      = null!;
        private Button _buttonOnlyIcon    = null!;
        private Button _buttonIconFirst   = null!;
        private Button _buttonIconLast    = null!;
        private Button _buttonIconAll     = null!;
        private Button _buttonStacked     = null!;
        private Button _buttonWithFlexRow = null!;

        private Chip _chip         = null!;
        private Chip _chipLink     = null!;
        private Chip _chipWithIcon = null!;

        private Checkbox _checkbox = null!;

        private CheckboxInput _checkboxInput = null!;

        private Link _link1 = null!;
        private Link _link2 = null!;
        private Link _link3 = null!;
        private Link _link4 = null!;

        private Expander _expander = null!;

        private Dropdown   _dropdown1   = null!;
        private LocalModal _localModal1 = null!;

        private bool _checked = true;

        private Spinner _spinnerNoTextNormal  = null!;
        private Spinner _spinnerTextNormal    = null!;
        private Spinner _spinnerNoTextInline  = null!;
        private Spinner _spinnerTextInline    = null!;
        private Spinner _spinnerTextLarge     = null!;
        private Spinner _spinnerTextLargeFont = null!;

        [Inject] public IJSRuntime     JSRuntime      { get; set; } = null!;
        [Inject] public ElementService ElementService { get; set; } = null!;

        protected override void OnInitialized()
        {
            _header = new SecondaryHeader
            (
                new HeaderLink("Secondary header".AsStatic(), () => "/elements").AsStatic(),
                new IRenderable[]
                {
                    new Chip("asdf".AsStatic()),
                    new BootstrapIcon("chevron-right"),
                    new Chip("asdf".AsStatic()),
                    new BootstrapIcon("chevron-right"),
                    new TextBlock(ContentRef.Static("Test block 1")),
                    new Space(),
                    new VerticalLine(),
                    new Space(),
                    new TextBlock("Test block 2".AsStatic()),
                    new Space(),
                    new HeaderLink("Normal link".AsStatic(), () => "/elements"),
                }.AsStatic());

            _panelExpanderElems = ContentRef.Static(new[]
            {
                new TextBlock(ContentRef.Static("Number ID list".AsTextContent(size: 1.2))),
            });

            _intInput = new IntInput(JSRuntime, 0);
            _intInput0Null = new IntInput(JSRuntime, 0,
                new IntInput.Spec {Consider0Null = Always.True});
            _decimalInput = new DecimalInput(JSRuntime, 0.0m);
            _decimalInput0Null = new DecimalInput(JSRuntime, 0.0m,
                new DecimalInput.Spec {Consider0Null = Always.True, Min = () => -2.5m, Max = () => 2.5m});

            decimal? decimalInputSteppedV = null;
            _decimalInputStepped =
                new DecimalInput(JSRuntime, 0.0m,
                    new DecimalInput.Spec
                        {Step = () => 0.01m, Tooltip = () => new Tooltip(decimalInputSteppedV?.ToString() ?? "")});
            _decimalInputStepped.OnChange += v =>
            {
                decimalInputSteppedV = v;
                InvokeAsync(StateHasChanged);
            };

            void PrintL(int?     v) => Console.WriteLine($"int -> {v}");
            void PrintD(decimal? v) => Console.WriteLine($"decimal -> {v}");

            _intInput.OnChange            += PrintL;
            _intInput0Null.OnChange       += PrintL;
            _decimalInput.OnChange        += PrintD;
            _decimalInput0Null.OnChange   += PrintD;
            _decimalInputStepped.OnChange += PrintD;

            //

            _textInput = new TextInput(JSRuntime, null);
            _textInputHighlighted = new TextInput(JSRuntime, null, new TextInput.Spec
            {
                HighlightColor = () => Constants.Accent_7,
            });
            _textInputClearable = new TextInput(JSRuntime, null, new TextInput.Spec
            {
                IsClearable = Always.True,
            });
            _textInputClearableScaled = new TextInput(JSRuntime, null, new TextInput.Spec
            {
                IsClearable = Always.True,
                Scale       = () => 2,
            });

            //

            _checkboxInput = new CheckboxInput(JSRuntime, false, new CheckboxInput.Spec {IsRequired = Always.True});

            void PrintB(bool v) => Console.WriteLine($"bool -> {v}");

            _checkboxInput.OnChange += PrintB;

            //

            _buttonNoIcon = new Button("asdf".AsStatic(), new Button.Spec
            {
                Tooltip = () => new Tooltip("<strong>asdf</strong>"),
            });

            _buttonOnlyIcon = new Button(ContentRef.Static(new BootstrapIcon("caret-right-fill", 16).Renderer()));
            _buttonIconFirst = new Button(ContentRef.Static(new IRenderable[]
            {
                new BootstrapIcon("caret-left-fill", 16),
                "asdf left".AsContent(),
            }), new Button.Spec
            {
                HREF    = () => "/asdf/left",
                Tooltip = () => new Tooltip("asdf left", placement: TooltipPlacement.Left),
            });
            _buttonIconLast = new Button(ContentRef.Static(new IRenderable[]
            {
                "asdf right".AsContent(),
                new BootstrapIcon("caret-right-fill", 16),
            }), new Button.Spec()
            {
                HREF    = () => "/asdf/left",
                Tooltip = () => new Tooltip("asdf right", placement: TooltipPlacement.Right),
            });
            _buttonIconAll = new Button(ContentRef.Static(new IRenderable[]
            {
                new BootstrapIcon("caret-left-fill",  16),
                new BootstrapIcon("caret-right-fill", 16),
            }));
            _buttonStacked = new Button(ContentRef.Static(new IRenderable[]
            {
                new FlexColumn(ContentRef.Static(new IRenderable[]
                {
                    "Top content/full name".AsContent(),
                    "Lower content".AsTextContent(size: 0.8, weight: FontWeight.Normal),
                })),
                new BootstrapIcon("chevron-right"),
            }));
            _buttonWithFlexRow = new Button(ContentRef.Static(new IRenderable[]
            {
                new FlexRow(ContentRef.Static(new IRenderable[]
                {
                    "asdf".AsStatic(),
                    new BootstrapIcon("chevron-right"),
                })),
            }));

            foreach (Button.Style style in Enum.GetValues<Button.Style>())
            {
                var b = new Button(ContentRef.Static(new IRenderable[]
                    {
                        ("Color: " + style).AsContent(),
                        ContentRef.Static(new BootstrapIcon("caret-down-fill", 16)),
                    }),
                    new Button.Spec
                    {
                        Style = () => style,
                        Tooltip = () =>
                            new Tooltip($"{style} {_checked}", 10, TooltipFollow.Initial, TooltipPlacement.Right),
                    });

                b.OnClick += (_, _) => Console.WriteLine($"Click: {style}");

                // TODO: Still works?
                var b2 = new Button(ContentRef.Static(new IRenderable[]
                    {
                        ("Color: " + style).AsContent(),
                        new BootstrapIcon("slash-circle-fill", 16),
                    }),
                    new Button.Spec
                    {
                        Style      = () => style,
                        Tooltip    = () => new Tooltip($"{style} {_checked}", 10, placement: TooltipPlacement.Right),
                        IsDisabled = () => true,
                    });

                _buttonsColored.Add(b);
                _buttonsColored.Add(b2);
            }

            //

            _chip = new Chip("Chip 1".AsStatic(), new Chip.Spec
            {
                Height  = () => 24,
                Tooltip = () => new Tooltip("Tooltip"),
            });

            _chipLink = new Chip("Chip 2".AsStatic(), new Chip.Spec
            {
                HREF   = () => "/",
                Height = () => 24,
            });

            _chipWithIcon = new Chip(ContentRef.Static(new IRenderable[]
            {
                new BootstrapIcon("slash-circle-fill"),
                "Text content".AsContent(),
                new BootstrapIcon("slash-circle-fill"),
            }));

            _checkbox = new Checkbox(new Checkbox.Spec
            {
                IsChecked  = () => _checked,
                IsDisabled = () => _checked,
            });
            _checkbox.OnToggle += (_, v) => PrintB(v);

            _link1 = new Link("Link 1".AsStatic(), new Link.Spec(() => "/"));
            _link2 = new Link("Link 2".AsStatic(), new Link.Spec(() => "/")
            {
                FontWeight = () => FontWeight.SemiBold,
            });
            _link3 = new Link("Link 3".AsStatic(), new Link.Spec(() => "/")
            {
                IsAccented = Always.True,
            });
            _link4 = new Link("Link 4 (highlighted)".AsStatic(), new Link.Spec(() => "/")
            {
                IsHighlighted = Always.True,
            });

            _expander = new Expander
            (
                "Show advanced options".AsStatic(),
                "Hide advanced options".AsStatic()
            );
            _expander.Expand();

            _dropdown1 = new Dropdown
            (
                ContentRef.Static(new IRenderable[]
                {
                    new Link(ContentRef.Static(new IRenderable[]
                    {
                        "Dropdown 1".AsContent(),
                        new BootstrapIcon("chevron-down"),
                    }), new Link.Spec(() => "/elements")),
                }),
                ContentRef.Static(new IRenderable[]
                {
                    "asdf".AsContent(),
                    new HorizontalLine(() => new Size(9, 0), () => Unit.Percentage(50)),
                    "asdf".AsContent(),
                    new Button(ContentRef.Static(new IRenderable[]
                    {
                        "Settings".AsContent(),
                        new BootstrapIcon("gear"),
                    }), new Button.Spec {HREF = () => "/", Style = () => Button.Style.Transparent}),
                    new Button(ContentRef.Static(new IRenderable[]
                    {
                        "Settings 222".AsContent(),
                        new BootstrapIcon("gear-fill"),
                    }), new Button.Spec {HREF = () => "/", Style = () => Button.Style.Transparent}),
                    new Button(new FlexRow(new IRenderable[]
                        {
                            "Settings".AsContent(),
                            new Space(() => 10),
                            new BootstrapIcon("gear"),
                        }.AsStatic(), () => FlexJustify.SpaceBetween).AsStatic(),
                        new Button.Spec {HREF = () => "/", Style = () => Button.Style.Transparent}),
                    new Button(new FlexRow(new IRenderable[]
                        {
                            "Settings 222".AsContent(),
                            new Space(() => 10),
                            new BootstrapIcon("gear-fill"),
                        }.AsStatic(), () => FlexJustify.SpaceBetween).AsStatic(),
                        new Button.Spec {HREF = () => "/", Style = () => Button.Style.Transparent}),
                    new HorizontalLine(),
                    new Dropdown
                    (
                        ContentRef.Static(new IRenderable[]
                        {
                            new Link(ContentRef.Static(new IRenderable[]
                            {
                                "Dropdown 1".AsContent(),
                                new BootstrapIcon("chevron-down"),
                            }), new Link.Spec(() => "/elements")),
                        }),
                        ContentRef.Static(new IRenderable[]
                        {
                            "asdf".AsContent(),
                            new HorizontalLine(),
                            "asdf".AsContent(),
                            new HorizontalLine(),
                            new Button(ContentRef.Static(new IRenderable[]
                            {
                                new BootstrapIcon("chevron-right"),
                                "Chevron".AsContent(),
                            }), new Button.Spec {HREF = () => "/", Style = () => Button.Style.Transparent}),
                        }), new Dropdown.Spec {PlacementGetter = () => TooltipPlacement.RightStart}),
                })
            );

            _spinnerNoTextNormal = new Spinner();
            _spinnerTextNormal = new Spinner(new Spinner.Spec
                {Text = () => "Spinner with text normal...",});

            _spinnerNoTextInline = new Spinner(new Spinner.Spec {Style = () => Spinner.Style.Inline});
            _spinnerTextInline = new Spinner(new Spinner.Spec
                {Style = () => Spinner.Style.Inline, Text = () => "Spinner with text inline..."});

            _spinnerTextLarge = new Spinner(new Spinner.Spec
                {Text = () => "Spinner with text large...", Scale = () => 3});
            _spinnerTextLargeFont = new Spinner(new Spinner.Spec
                {Text = () => "Spinner with text large font...", FontSize = () => 3});

            _localModal1 = new LocalModal(new IRenderable[]
            {
                _spinnerTextNormal,
            }.AsStatic());

            //

            for (double i = 0; i < 3; i += 0.1)
            {
                double i1 = i;
                _buttonsScaled.Add(new Button(new IRenderable[]
                {
                    $"Button content {i1}".AsContent(),
                }.AsStatic(), new Button.Spec
                {
                    Scale = () => i1,
                    HREF  = () => "/elements",
                }));
                _buttonsScaled.Add(new Button(new IRenderable[]
                {
                    $"Button content {i1}".AsContent(),
                }.AsStatic(), new Button.Spec
                {
                    Scale   = () => i1,
                    IsSmall = () => true,
                }));
                _chipsScaled.Add(new Chip(new IRenderable[]
                {
                    $"Chip content {i1}".AsContent(),
                }.AsStatic(), new Chip.Spec
                {
                    Scale = () => i1,
                }));
                _linksScaled.Add(new Link(new IRenderable[]
                {
                    $"Link content {i1}".AsContent(),
                }.AsStatic(), new Link.Spec(() => "/elements")
                {
                    Scale = () => i1,
                }));
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            int? v = _intInput.GetValue();

            if (firstRender)
                Task.Run(() =>
                {
                    Thread.Sleep(2500);
                    _checked = false;
                    _checkbox.Reset();
                    InvokeAsync(StateHasChanged);
                });

            // await JSRuntime.InvokeVoidAsync("window.LoadAllScrollbars");

            await ElementService.ProcessJobs();

            // await Interop.CreateBitTooltips(JSRuntime, _id);
        }
    }
}