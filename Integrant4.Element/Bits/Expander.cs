using System.Collections.Generic;
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

        private readonly Hook _hook;

        private readonly Button? _button;

        private WriteOnlyHook? _refresher;

        public Expander
        (
            DynamicContent expandContent,
            DynamicContent contractContent,
            Spec?          spec = null
        )
        {
            _hook = new Hook();

            IEnumerable<IRenderable> Contents() => Expanded
                ? new[] { contractContent.Invoke(), UpIcon }
                : new[] { expandContent.Invoke(), DownIcon };

            _button = new Button
            (
                Contents,
                new Button.Spec
                {
                    Style = () =>
                        spec?.ButtonStyle?.Invoke() ??
                        (!Expanded ? Button.Style.Transparent : Button.Style.AccentTransparent),
                    IsSmall = null,
                    OnClick = (_, _) =>
                    {
                        Expanded = !Expanded;
                        _refresher?.Invoke();
                        _hook.Invoke();
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
            DefaultExpandableHeaderNotice = new RenderableArray(new IRenderable[]
            {
                new Filler(),
                new TextBlock("Click to expand".AsContent(), new TextBlock.Spec
                {
                    Padding         = () => new Size(0, 3),
                    ForegroundColor = () => Constants.Text_2,
                }),
            });
        }

        public class Spec
        {
            public Button.StyleGetter? ButtonStyle { get; init; }
        }
    }
}