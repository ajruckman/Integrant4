using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Integrant4.API;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Integrant4.Element.Bits
{
    public partial class Button : BitBase, IRefreshableBit
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Spec
        {
            public StyleGetter? Style { get; init; }

            public Callbacks.Callback<bool>?  IsSmall { get; init; }
            public Action<Button, ClickArgs>? OnClick { get; init; }

            public Callbacks.IsVisible?  IsVisible  { get; init; }
            public Callbacks.IsDisabled? IsDisabled { get; init; }

            public Callbacks.Classes?    Classes    { get; init; }
            public Callbacks.HREF?       HREF       { get; init; }
            public Callbacks.Pixels?     Height     { get; init; }
            public Callbacks.Pixels?     Width      { get; init; }
            public Callbacks.Size?       Margin     { get; init; }
            public Callbacks.Size?       Padding    { get; init; }
            public Callbacks.Scale?      Scale      { get; init; }
            public Callbacks.FontWeight? FontWeight { get; init; }
            public Callbacks.Display?    Display    { get; init; }
            public Callbacks.Data?       Data       { get; init; }
            public Callbacks.Tooltip?    Tooltip    { get; init; }

            internal BaseSpec ToBaseSpec() => new()
            {
                Scaled     = true,
                IsVisible  = IsVisible,
                IsDisabled = IsDisabled,
                HREF       = HREF,
                Height     = Height,
                Width      = Width,
                Classes    = Classes,
                Margin     = Margin,
                Padding    = Padding,
                Scale      = Scale,
                FontWeight = FontWeight,
                Display    = Display,
                Data       = Data,
                Tooltip    = Tooltip,
            };
        }
    }

    public partial class Button
    {
        private readonly Callbacks.BitContents     _contents;
        private readonly Callbacks.Callback<bool>? _isSmall;

        public Button(Callbacks.BitContent content, Spec? spec = null)
            : this(content.AsContents(), spec) { }

        public Button(Callbacks.BitContents contents, Spec? spec = null)
            : base(spec?.ToBaseSpec(), new ClassSet("I4E-Bit", "I4E-Bit-" + nameof(Button)))
        {
            _contents    = contents;
            _styleGetter = spec?.Style ?? DefaultStyleGetter;
            _isSmall     = spec?.IsSmall;

            if (spec?.OnClick != null)
            {
                OnClick += spec.OnClick;
            }
        }
    }

    public partial class Button
    {
        private WriteOnlyHook? _refresher;

        public override RenderFragment Renderer() => Latch.Create(builder =>
        {
            IRenderable[] contents = _contents.Invoke().ToArray();

            List<string> ac = new() {"I4E-Bit-Button--" + _styleGetter.Invoke()};

            if (contents.First() is IIcon) ac.Add("I4E-Bit-Button--IconLeft");
            if (contents.Last() is IIcon) ac.Add("I4E-Bit-Button--IconRight");
            if (_isSmall?.Invoke() == true) ac.Add("I4E-Bit-Button--Small");

            //

            int seq = -1;

            if (BaseSpec.HREF == null)
            {
                builder.OpenElement(++seq, "button");
            }
            else
            {
                builder.OpenElement(++seq, "a");
                builder.AddAttribute(++seq, "href", BaseSpec.HREF.Invoke());
            }

            BitBuilder.ApplyAttributes(this, builder, ref seq, ac, null);

            builder.AddAttribute(++seq, "onclick",
                EventCallback.Factory.Create<MouseEventArgs>(this, Click));

            builder.OpenElement(++seq, "span");
            builder.AddAttribute(++seq, "class", "I4E-Bit-Button-Contents");

            BitBuilder.ApplyContentAttributes(this, builder, ref seq);

            foreach (IRenderable renderable in contents)
            {
                builder.OpenElement(++seq, "span");
                builder.AddAttribute(++seq, "class", "I4E-Bit-Button-Content");
                builder.AddContent(++seq, renderable.Renderer());
                builder.CloseElement();
            }

            builder.CloseElement();

            builder.CloseElement();

            BitBuilder.ScheduleElementJobs(this, builder, ref seq);
        }, v => _refresher = v);

        public void Refresh() => _refresher?.Invoke();

        public event Action<Button, ClickArgs>? OnClick;

        private void Click(MouseEventArgs args)
        {
            if (BaseSpec.IsDisabled?.Invoke() == true) return;

            var c = new ClickArgs
            (
                (ushort) args.Button,
                (ushort) args.ClientX,
                (ushort) args.ClientY,
                args.ShiftKey,
                args.CtrlKey
            );

            OnClick?.Invoke(this, c);
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
            AccentTransparent,
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