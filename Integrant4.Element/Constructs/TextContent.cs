using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Constructs
{
    public class TextContent : IConstruct
    {
        private readonly Callbacks.Callback<string> _content;

        private readonly string _style;

        public TextContent
        (
            Callbacks.Callback<string> content,
            string?                    color   = null,
            double?                    size    = null,
            FontWeight?                weight  = null,
            TextAlign?                 align   = null,
            Display?                   display = null
        )
        {
            _content = content;

            List<string> style = new(5);

            if (color != null)
                style.Add($"color: {color};");
            if (size != null)
                style.Add($"font-size: {size}rem;");
            if (weight != null)
                style.Add($"font-weight: {(int) weight};");

            if (align != null)
            {
                string v = align switch
                {
                    TextAlign.Left   => "left",
                    TextAlign.Center => "center",
                    TextAlign.Right  => "right",
                    _                => throw new ArgumentOutOfRangeException(),
                };
                style.Add($"text-align: {v};");
            }

            if (display != null)
            {
                string v = display switch
                {
                    Display.Undefined   => "unset",
                    Display.Inline      => "inline",
                    Display.InlineBlock => "inline-block",
                    Display.Block       => "block",
                    _                   => throw new ArgumentOutOfRangeException(),
                };
                style.Add($"display: {v};");
            }

            _style = string.Join(' ', style);
        }

        public RenderFragment Renderer() => builder =>
        {
            int seq = -1;
            builder.OpenElement(++seq, "div");
            builder.AddAttribute(++seq, "class", "I4E-Construct-TextContent");
            builder.AddAttribute(++seq, "style", _style);
            builder.AddContent(++seq, _content.Invoke());
            builder.CloseElement();
        };
    }

    public static class TextContentExtensions
    {
        public static TextContent AsTextContent
        (
            this string v,
            string?     color   = null,
            double?     size    = null,
            FontWeight? weight  = null,
            TextAlign?  align   = null,
            Display?    display = null
        )
        {
            return new TextContent(() => v, color, size, weight, align, display);
        }
    }
}