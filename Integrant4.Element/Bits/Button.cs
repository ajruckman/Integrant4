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

            public Callbacks.IsVisible? IsVisible { get; init; }

            public Callbacks.IsDisabled? IsDisabled { get; init; }

            // public Callbacks.BitID?         ID         { get; init; }
            public Callbacks.Classes? Classes    { get; init; }
            public Callbacks.Size?    Margin     { get; init; }
            public Callbacks.Size?    Padding    { get; init; }
            public Callbacks.REM?     FontSize   { get; init; }
            public Callbacks.FontWeight?  FontWeight { get; init; }
            public Callbacks.Display? Display    { get; init; }
            public Callbacks.Data?    Data       { get; init; }
            public Callbacks.Tooltip? Tooltip    { get; init; }

            public ElementService? ElementService { get; init; }

            internal BaseSpec ToBaseSpec() => new()
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
            : base(spec?.ToBaseSpec(), new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(Button)))
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

                BaseSpec.QueueTooltip(BaseSpec, ID);
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