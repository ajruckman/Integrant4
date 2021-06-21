using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Colorant.Themes.Default;
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
        private readonly List<Button> _buttonsColored = new();

        private Header                _header             = null!;
        private Callbacks.BitContents _panelExpanderElems = null!;

        private TextInput _textInput                = null!;
        private TextInput _textInputHighlighted     = null!;
        private TextInput _textInputClearable       = null!;
        private TextInput _textInputClearableScaled = null!;

        private IntegerInput _intInput            = null!;
        private IntegerInput _intInput0Null       = null!;
        private DecimalInput _decimalInput        = null!;
        private DecimalInput _decimalInput0Null   = null!;
        private DecimalInput _decimalInputStepped = null!;

        private Button _buttonNoIcon    = null!;
        private Button _buttonOnlyIcon  = null!;
        private Button _buttonIconFirst = null!;
        private Button _buttonIconLast  = null!;
        private Button _buttonIconAll   = null!;
        private Button _buttonStacked   = null!;

        private Chip _chip         = null!;
        private Chip _chipLink     = null!;
        private Chip _chipWithIcon = null!;
        private Chip _chipUncached = null!;
        private Chip _chipCached   = null!;

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

        private readonly List<Button> _buttonsScaled = new();
        private readonly List<Chip>   _chipsScaled   = new();
        private readonly List<Link>   _linksScaled   = new();

        [Inject] public IJSRuntime     JSRuntime      { get; set; } = null!;
        [Inject] public ElementService ElementService { get; set; } = null!;

        protected override void OnInitialized()
        {
            _header = new Header(() => new IRenderable[]
            {
                new PageLink(() => "Secondary header".AsContent(),
                    new PageLink.Spec(() => "/elements") { IsTitle = Always.True }),
                new Filler(),
                new PageLink(() => "Normal link".AsContent(), new PageLink.Spec(() => "/elements")),
            }, Header.Style.Secondary);

            _panelExpanderElems = () => new[]
            {
                new TextBlock("Number ID list".AsTextContent(size: 1.2)),
            };

            _intInput = new IntegerInput(JSRuntime, 0);
            _intInput0Null = new IntegerInput(JSRuntime, 0,
                new IntegerInput.Spec { Consider0Null = Always.True });
            _decimalInput = new DecimalInput(JSRuntime, 0.0m);
            _decimalInput0Null = new DecimalInput(JSRuntime, 0.0m,
                new DecimalInput.Spec { Consider0Null = Always.True, Min = () => -2.5m, Max = () => 2.5m });

            decimal? _decimalInputSteppedV = null;
            _decimalInputStepped =
                new DecimalInput(JSRuntime, 0.0m,
                    new DecimalInput.Spec
                        { Step = () => 0.01m, Tooltip = () => new Tooltip(_decimalInputSteppedV?.ToString() ?? "") });
            _decimalInputStepped.OnChange += v =>
            {
                _decimalInputSteppedV = v;
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

            _textInput = new TextInput(JSRuntime, null, new TextInput.Spec
                { });
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

            _checkboxInput = new CheckboxInput(JSRuntime, false, new CheckboxInput.Spec { IsRequired = Always.True });

            void PrintB(bool v) => Console.WriteLine($"bool -> {v}");

            _checkboxInput.OnChange += PrintB;

            //

            _buttonNoIcon = new Button(() =>
            {
                Console.WriteLine("_buttonNoIcon");
                return "asdf".AsContent();
            });

            _buttonOnlyIcon = new Button(() => new BootstrapIcon("caret-right-fill", 16));
            _buttonIconFirst = new Button(() => new IRenderable[]
            {
                new BootstrapIcon("caret-left-fill", 16),
                "asdf left".AsContent(),
            }, new Button.Spec
            {
                HREF = () => "/asdf/left",
            });
            _buttonIconLast = new Button(() => new IRenderable[]
            {
                "asdf right".AsContent(),
                new BootstrapIcon("caret-right-fill", 16),
            }, new Button.Spec()
            {
                HREF = () => "/asdf/left",
            });
            _buttonIconAll = new Button(() => new IRenderable[]
            {
                new BootstrapIcon("caret-left-fill", 16),
                new BootstrapIcon("caret-right-fill", 16),
            });
            _buttonStacked = new Button(() => new IRenderable[]
            {
                new StackedContent(() => new IRenderable[]
                {
                    "Top content/full name".AsContent(),
                    "Lower content".AsTextContent(size: 0.8, weight: FontWeight.Normal),
                }, () => FlexAlign.Start),
                new BootstrapIcon("chevron-right"),
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
                        Style   = () => style,
                        Tooltip = () => new Tooltip($"{style} {_checked}", 500, Placement.Right),
                    });

                b.OnClick += (_, _) => Console.WriteLine($"Click: {style}");

                var b2 = new Button(() => new IRenderable[]
                    {
                        ("Color: " + style).AsContent(),
                        new BootstrapIcon("slash-circle-fill", 16),
                    },
                    new Button.Spec
                    {
                        Style      = () => style,
                        Tooltip    = () => new Tooltip($"{style} {_checked}", 500, Placement.Right),
                        IsDisabled = () => true,
                    });

                _buttonsColored.Add(b);
                _buttonsColored.Add(b2);
            }

            //

            _chip = new Chip(() => "Chip 1".AsContent(), new Chip.Spec
            {
                Height  = () => 24,
                Tooltip = () => new Tooltip("Tooltip"),
            });

            _chipLink = new Chip(() => "Chip 2".AsContent(), new Chip.Spec
            {
                HREF   = () => "/",
                Height = () => 24,
            });

            _chipWithIcon = new Chip(() => new IRenderable[]
            {
                new BootstrapIcon("slash-circle-fill"),
                "Text content".AsContent(),
                new BootstrapIcon("slash-circle-fill"),
            });

            _chipUncached = new Chip(() => DateTime.Now.ToString(CultureInfo.CurrentCulture).AsContent());

            _checkbox = new Checkbox(new Checkbox.Spec
            {
                IsChecked  = () => _checked,
                IsDisabled = () => _checked,
            });
            _checkbox.OnToggle += (_, v) => PrintB(v);

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

            _expander = new Expander
            (
                () => "Show advanced options".AsContent(),
                () => "Hide advanced options".AsContent()
            );
            _expander.Expand();

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
                    new Button(() => new IRenderable[]
                    {
                        new BootstrapIcon("gear"),
                        "Settings".AsContent(),
                    }, new Button.Spec { HREF = () => "/", Style = () => Button.Style.Transparent }),
                    new Button(() => new IRenderable[]
                    {
                        new BootstrapIcon("gear-fill"),
                        "Settings 2".AsContent(),
                    }, new Button.Spec { HREF = () => "/", Style = () => Button.Style.Transparent }),
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
                            new Button(() => new IRenderable[]
                            {
                                new BootstrapIcon("chevron-right"),
                                "Chevron".AsContent(),
                            }, new Button.Spec { HREF = () => "/", Style = () => Button.Style.Transparent }),
                        }, new Dropdown.Spec { PlacementGetter = () => Placement.RightStart }),
                }
            );

            _spinnerNoTextNormal = new Spinner();
            _spinnerTextNormal = new Spinner(new Spinner.Spec
                { Text = () => "Spinner with text normal...", });

            _spinnerNoTextInline = new Spinner(new Spinner.Spec { Style = () => Spinner.Style.Inline });
            _spinnerTextInline = new Spinner(new Spinner.Spec
                { Style = () => Spinner.Style.Inline, Text = () => "Spinner with text inline..." });

            _spinnerTextLarge = new Spinner(new Spinner.Spec
                { Text = () => "Spinner with text large...", Scale = () => 3 });
            _spinnerTextLargeFont = new Spinner(new Spinner.Spec
                { Text = () => "Spinner with text large font...", FontSize = () => 3 });

            _localModal1 = new LocalModal(() => new IRenderable[]
            {
                _spinnerTextNormal,
            });

            //

            for (double i = 0; i < 3; i += 0.1)
            {
                double i1 = i;
                _buttonsScaled.Add(new Button(() => new IRenderable[]
                {
                    $"Button content {i1}".AsContent(),
                }, new Button.Spec
                {
                    Scale = () => i1,
                    HREF  = () => "/elements",
                }));
                _buttonsScaled.Add(new Button(() => new IRenderable[]
                {
                    $"Button content {i1}".AsContent(),
                }, new Button.Spec
                {
                    Scale   = () => i1,
                    IsSmall = () => true,
                }));
                _chipsScaled.Add(new Chip(() => new IRenderable[]
                {
                    $"Chip content {i1}".AsContent(),
                }, new Chip.Spec
                {
                    Scale = () => i1,
                }));
                _linksScaled.Add(new Link(() => new IRenderable[]
                {
                    $"Link content {i1}".AsContent(),
                }, new Link.Spec(() => "/elements")
                {
                    Scale = () => i1,
                }));
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Console.WriteLine($"Elements: OnAfterRenderAsync {firstRender}");

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