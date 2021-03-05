using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Integrant4.Element.Bits
{
    public partial class Button : BitBase
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public StyleGetter? Style { get; init; }

            public Callbacks.BitIsVisible? IsVisible { get; init; }

            public Callbacks.BitIsDisabled? IsDisabled { get; init; }

            // public Callbacks.BitID?         ID         { get; init; }
            public Callbacks.BitClasses? Classes    { get; init; }
            public Callbacks.BitSize?    Margin     { get; init; }
            public Callbacks.BitSize?    Padding    { get; init; }
            public Callbacks.BitREM?     FontSize   { get; init; }
            public Callbacks.BitWeight?  FontWeight { get; init; }
            public Callbacks.BitDisplay? Display    { get; init; }
            public Callbacks.BitData?    Data       { get; init; }
            public Callbacks.BitTooltip? Tooltip    { get; init; }

            public ElementService? ElementService { get; init; }

            internal BitSpec ToBitSpec() => new()
            {
                ElementService = ElementService,
                IsVisible      = IsVisible,
                IsDisabled     = IsDisabled,
                // ID         = ID,
                Classes    = Classes,
                Margin     = Margin,
                Padding    = Padding,
                FontSize   = FontSize,
                FontWeight = FontWeight,
                Display    = Display,
                Data       = Data,
                Tooltip    = Tooltip,
            };
        }
    }

    public partial class Button
    {
        private readonly Callbacks.BitContents _contents;

        public Button(Callbacks.BitContent content, Spec? spec = null)
            : this(content.AsContents(), spec)
        {
        }

        public Button(Callbacks.BitContents contents, Spec? spec = null)
            : base(spec?.ToBitSpec(), new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(Button)))
        {
            _contents    = contents;
            _styleGetter = spec?.Style ?? DefaultStyleGetter;
        }
    }

    public partial class Button
    {
        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                IRenderable[] contents = _contents.Invoke().ToArray();

                List<string> ac = new() {"I4E-Bit-Button--" + _styleGetter.Invoke()};

                if (contents.First() is IIcon) ac.Add("I4E-Bit-Button--IconLeft");
                if (contents.Last() is IIcon) ac.Add("I4E-Bit-Button--IconRight");

                //

                int seq = -1;
                builder.OpenElement(++seq, "button");

                BitBuilder.ApplyAttributes(this, builder, ref seq, ac.ToArray(), null);

                builder.AddAttribute(++seq, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, Click));

                foreach (IRenderable renderable in contents)
                {
                    builder.OpenElement(++seq, "span");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-Button-Content");
                    builder.AddContent(++seq, renderable.Renderer());
                    builder.CloseElement();
                }

                builder.CloseElement();

                QueueTooltip();
            }

            return Fragment;
        }

        public event Action<ClickArgs>? OnClick;

        private async Task Click(MouseEventArgs args)
        {
            if (BaseSpec.IsDisabled?.Invoke() == true) return;

            OnClick?.Invoke(new ClickArgs
            (
                (ushort) args.Button,
                (ushort) args.ClientX,
                (ushort) args.ClientY,
                args.ShiftKey,
                args.CtrlKey
            ));
        }
    }

    public partial class Button
    {
        public delegate Style StyleGetter();

        public enum Style
        {
            Default,
            Accent,
            Transparent,
            Blue,
            Green,
            Orange,
            Purple,
            Red,
            Yellow,
        }

        private readonly StyleGetter _styleGetter;

        private static StyleGetter DefaultStyleGetter => () => Style.Default;
    }
}