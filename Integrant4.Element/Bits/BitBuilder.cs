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
            builder.AddAttribute(++seq, "id", bitBase.ID);
            builder.AddAttribute(++seq, "class",
                ElementBuilder.ClassAttribute(bitBase.BaseClasses, bitBase.BaseSpec, additionalClasses));
            builder.AddAttribute(++seq, "style",
                ElementBuilder.StyleAttribute(bitBase.BaseSpec, additionalStyles));

            ++seq;
            if (bitBase.BaseSpec.IsVisible?.Invoke() == false)
                builder.AddAttribute(seq, "hidden", true);

            ++seq;
            if (bitBase.BaseSpec.Tooltip != null)
                builder.AddAttribute(seq, "data-i4e.tooltip", bitBase.BaseSpec.Tooltip.Invoke());

            if (bitBase.BaseSpec.Data != null)
                foreach ((string name, Callbacks.DataValue getter) in bitBase.BaseSpec.Data.Invoke())
                    builder.AddAttribute(++seq, "data-" + name, getter.Invoke());
        }
    }
}