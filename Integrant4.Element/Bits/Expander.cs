using System;
using Integrant4.API;
using Integrant4.Colorant.Themes.Main;
using Integrant4.Element.Constructs;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
// ReSharper disable RedundantArgumentDefaultValue

namespace Integrant4.Element.Bits
{
    public partial class Expander : IRefreshableBit
    {
        private static readonly BootstrapIcon DownIcon = new("caret-down-fill", 16);
        private static readonly BootstrapIcon UpIcon   = new("caret-up-fill", 16);

        private readonly Hook                _hook;
        private readonly Button?             _button;
        private readonly Button.StyleGetter? _buttonStyle;

        private WriteOnlyHook? _refresher;

        public Expander
        (
            ContentRef          expandContent,
            ContentRef          contractContent,
            Button.StyleGetter? buttonStyle = null
        )
        {
            _buttonStyle = buttonStyle;
            _hook        = new Hook();

            IRenderable[] Contents() => Expanded
                ? new IRenderable[] {contractContent, UpIcon}
                : new IRenderable[] {expandContent, DownIcon};

            _button = new Button
            (
                ContentRef.Dynamic(Contents),
                new Button.Spec
                {
                    Style = () =>
                        _buttonStyle?.Invoke() ??
                        (!Expanded ? Button.Style.Transparent : Button.Style.AccentTransparent),
                    IsSmall = null,
                    OnClick = (_, _) =>
                    {
                        Expanded = !Expanded;
                        _refresher?.Invoke();
                        _hook.Invoke();
                        OnChange?.Invoke(Expanded);
                    },
                    Classes = () => new ClassSet("I4E-Bit-Expander-PanelButton"),
                }
            );
        }

        public bool Expanded { get; private set; }

        public ReadOnlyHook Hook => _hook;

        public RenderFragment Renderer() => Latch.Create(builder =>
            {
                int seq = -1;

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class",
                    $"I4E-Bit-Expander I4E-Bit-Expanded--{(Expanded ? "Expanded" : "Contracted")}");

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Bit-Expander-Line");
                builder.CloseElement();

                builder.AddContent(++seq, _button!.Renderer());

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Bit-Expander-Line");
                builder.CloseElement();

                builder.CloseElement();
            },
            v => _refresher = v);

        public void Refresh() => _refresher?.Invoke();

        public event Action<bool>? OnChange;

        public void Expand()
        {
            Expanded = true;
            _refresher?.Invoke();
            _hook.Invoke();
        }

        public void Contract()
        {
            Expanded = true;
            _refresher?.Invoke();
            _hook.Invoke();
        }
    }

    public partial class Expander
    {
        public static readonly IRenderable DefaultExpandableHeaderNotice;

        static Expander()
        {
            DefaultExpandableHeaderNotice = new RenderableArray
            (
                new Filler(),
                new TextBlock
                (
                    ContentRef.Static("Click to expand"), new TextBlock.Spec
                    {
                        Padding         = () => new Size(0, 3),
                        ForegroundColor = () => Constants.Text_2,
                    }
                )
            );
        }
    }
}