using System;
using System.Linq;
using Integrant4.API;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Integrant4.Element.Bits
{
    public class Button : BitBase
    {
        public delegate Style ColorGetter();

        public enum Style
        {
            Default,
            Filled,
            Blue,
            Green,
            Orange,
            Purple,
            Red,
            Yellow,
        }

        private readonly ColorGetter _color;

        public Button
        (
            Callbacks.BitContent     content,
            ColorGetter?             color           = null,
            bool                     isStatic        = false,
            Callbacks.BitIsVisible?  isVisible       = null,
            Callbacks.BitIsDisabled? isDisabled      = null,
            Callbacks.BitClasses?    classes         = null,
            Callbacks.BitSize?       margin          = null,
            Callbacks.BitSize?       padding         = null,
            Callbacks.BitColor?      foregroundColor = null,
            Callbacks.BitColor?      backgroundColor = null,
            Callbacks.BitPixels?     pixelsHeight    = null,
            Callbacks.BitPixels?     pixelsWidth     = null,
            Callbacks.BitREM?        fontSize        = null,
            Callbacks.BitWeight?     fontWeight      = null,
            Callbacks.BitDisplay?    display         = null,
            Callbacks.BitData?       data            = null,
            Callbacks.BitTooltip?    tooltip         = null
        ) : this(content.AsContents(), color, isStatic, isVisible, isDisabled, classes, margin, padding,
            foregroundColor, backgroundColor, pixelsHeight, pixelsWidth, fontSize, fontWeight, display, data, tooltip)
        {
        }

        public Button
        (
            Callbacks.BitContents    contents,
            ColorGetter?             color           = null,
            bool                     isStatic        = false,
            Callbacks.BitIsVisible?  isVisible       = null,
            Callbacks.BitIsDisabled? isDisabled      = null,
            Callbacks.BitClasses?    classes         = null,
            Callbacks.BitSize?       margin          = null,
            Callbacks.BitSize?       padding         = null,
            Callbacks.BitColor?      foregroundColor = null,
            Callbacks.BitColor?      backgroundColor = null,
            Callbacks.BitPixels?     pixelsHeight    = null,
            Callbacks.BitPixels?     pixelsWidth     = null,
            Callbacks.BitREM?        fontSize        = null,
            Callbacks.BitWeight?     fontWeight      = null,
            Callbacks.BitDisplay?    display         = null,
            Callbacks.BitData?       data            = null,
            Callbacks.BitTooltip?    tooltip         = null
        ) : base
        (
            new BitSpec(contents)
            {
                IsStatic        = isStatic,
                IsVisible       = isVisible,
                IsDisabled      = isDisabled,
                Classes         = classes,
                Margin          = margin,
                Padding         = padding,
                ForegroundColor = foregroundColor,
                BackgroundColor = backgroundColor,
                Height          = pixelsHeight,
                Width           = pixelsWidth,
                FontSize        = fontSize,
                FontWeight      = fontWeight,
                Display         = display,
                Data            = data,
                Tooltip         = tooltip,
            }, new ClassSet("I4E." + nameof(Button))
        )
        {
            _color = color ?? DefaultStyleGetter;
        }

        private static Style DefaultStyleGetter() => Style.Default;

        private string[] LocalClasses(IRenderable[] contents)
        {
            string[] result = {"I4E." + nameof(Button) + ":" + _color.Invoke()};

            if (contents.First() is IIcon) result = result.Append("I4E.Button--IconFirst").ToArray();
            if (contents.Last() is IIcon) result  = result.Append("I4E.Button--IconLast").ToArray();

            return result;
        }

        public override RenderFragment Renderer()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;

                IRenderable[] contents = Spec.Contents.Invoke().ToArray();

                BitBuilder.OpenElement(builder, ref seq, "button", this, null, LocalClasses(contents));

                builder.AddAttribute(++seq, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, Click));

                foreach (IRenderable renderable in contents)
                {
                    builder.AddContent(++seq, renderable.Renderer());
                }

                BitBuilder.CloseElement(builder);
            }

            return Fragment;
        }

        public event Action<ClickArgs>? OnClick;

        private void Click(MouseEventArgs args)
        {
            if (Spec.IsDisabled?.Invoke() == true) return;

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
}