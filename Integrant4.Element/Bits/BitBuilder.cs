using System;
using System.Collections.Generic;
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

            if (bitBase.BaseSpec.Tooltip == null) seq += 3;
            else
            {
                Tooltip? t = bitBase.BaseSpec.Tooltip.Invoke();
                if (t != null)
                {
                    builder.AddAttribute(++seq, "data-i4e.tooltip-text", t.Value.Text);
                    builder.AddAttribute(++seq, "data-i4e.tooltip-delay", t.Value.Delay                ?? 0);
                    builder.AddAttribute(++seq, "data-i4e.tooltip-placement", t.Value.Placement?.Map() ?? "top");
                }
                else seq += 3;
            }

            if (bitBase.BaseSpec.Data != null)
                foreach ((string name, Callbacks.DataValue getter) in bitBase.BaseSpec.Data.Invoke())
                    builder.AddAttribute(++seq, "data-" + name, getter.Invoke());
        }

        internal static void ScheduleElementJobs(BitBase bitBase, RenderTreeBuilder builder, ref int seq)
        {
            if (bitBase.BaseSpec.Tooltip == null) return;

            List<Action<ElementService>> jobs = new();

            if (bitBase.BaseSpec.Tooltip != null)
                jobs.Add(v => v.AddJob(x => Interop.CreateTooltips(x, bitBase.ID)));

            if (jobs.Count > 0)
                ServiceInjector<ElementService>.Inject(builder, ref seq, jobs.ToArray());
        }
    }
}