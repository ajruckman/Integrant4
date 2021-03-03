using System;
using System.Collections.Generic;
using System.Linq;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    internal static class BitBuilder
    {
        internal static void ApplyAttributes
        (
            BitBase           bitBase,
            RenderTreeBuilder builder,
            ref int           seq,
            string[]?         additionalClasses,
            string[]?         additionalStyles
        )
        {
            builder.AddAttribute(++seq, "class", ClassAttribute(bitBase.BaseClasses, bitBase.Spec, additionalClasses));
            builder.AddAttribute(++seq, "style", StyleAttribute(bitBase.Spec, additionalStyles));

            ++seq;
            if (bitBase.Spec.IsVisible?.Invoke() == false)
                builder.AddAttribute(seq, "hidden", true);

            ++seq;
            if (bitBase.Spec.Tooltip != null)
                builder.AddAttribute(seq, "data-i4e.tooltip", bitBase.Spec.Tooltip.Invoke());

            ++seq;
            if (bitBase.Spec.ID != null)
                builder.AddAttribute(seq, "id", bitBase.Spec.ID.Invoke());
            
            if (bitBase.Spec.Data != null)
                foreach ((string name, Callbacks.DataValue getter) in bitBase.Spec.Data.Invoke())
                    builder.AddAttribute(++seq, "data-" + name, getter.Invoke());
        }

        internal static string ClassAttribute(ClassSet baseSet, BitSpec spec, string[]? additional)
        {
            ClassSet c = baseSet.Clone();

            if (additional != null)
                c.AddRange(additional);

            if (spec.IsDisabled?.Invoke() == true)
                c.Add("I4E.Bit.IsDisabled");
            if (spec.IsRequired?.Invoke() == true)
                c.Add("I4E.Bit.IsRequired");

            return c.ToString();
        }

        internal static string? StyleAttribute(BitSpec spec, string[]? additional)
        {
            List<string> result = new();

            if (spec.Margin != null)
            {
                Size v = spec.Margin.Invoke();
                result.Add($"margin: {v.Top}px {v.Right}px {v.Bottom}px {v.Left}px;");
            }

            if (spec.Padding != null)
            {
                Size v = spec.Padding.Invoke();
                result.Add($"padding: {v.Top}px {v.Right}px {v.Bottom}px {v.Left}px;");
            }

            if (spec.BackgroundColor != null)
            {
                result.Add($"background-color: {spec.BackgroundColor.Invoke()};");
            }

            if (spec.ForegroundColor != null)
            {
                result.Add($"color: {spec.ForegroundColor.Invoke()};");
            }

            if (spec.Height != null)
            {
                result.Add($"height: {spec.Height.Invoke()}px;");
            }

            if (spec.HeightMax != null)
            {
                result.Add($"max-height: {spec.HeightMax.Invoke()}px;");
            }

            if (spec.Width != null)
            {
                result.Add($"width: {spec.Width.Invoke()}px;");
            }

            if (spec.WidthMax != null)
            {
                result.Add($"max-width: {spec.WidthMax.Invoke()}px;");
            }

            if (spec.FontSize != null)
            {
                result.Add($"font-size: {spec.FontSize.Invoke()}rem;");
            }

            if (spec.FontWeight != null)
            {
                result.Add($"font-weight: {(int) spec.FontWeight.Invoke()};");
            }

            if (spec.Display != null)
            {
                string display = spec.Display.Invoke() switch
                {
                    Display.Undefined   => "unset",
                    Display.Inline      => "inline",
                    Display.InlineBlock => "inline-block",
                    Display.Block       => "block",
                    _                   => throw new ArgumentOutOfRangeException(),
                };
                result.Add($"display: {display};");
            }

            if (spec.Classes != null)
            {
                result.AddRange(spec.Classes.Invoke());
            }

            //

            if (additional != null)
                result.AddRange(additional);

            return result.Any() ? string.Join(' ', result) : null;
        }
    }
}