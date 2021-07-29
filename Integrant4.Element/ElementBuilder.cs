using System;
using System.Collections.Generic;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element
{
    internal interface IUnifiedSpec
    {
        SpecSet ToSpec();
    }

    internal interface IDualSpec
    {
        SpecSet ToOuterSpec();
        SpecSet ToInnerSpec();
    }

    internal static class ElementBuilder
    {
        internal static void ApplyAttributes
        (
            string               id,
            SpecSet?             spec,
            RenderTreeBuilder    builder,
            ref int              seq,
            IEnumerable<string>? additionalClasses,
            IEnumerable<string>? additionalStyles
        )
        {
            builder.AddAttribute(++seq, "id",    id);
            builder.AddAttribute(++seq, "class", spec?.ClassAttribute(additionalClasses));
            builder.AddAttribute(++seq, "style", spec?.StyleAttribute(additionalStyles));

            ++seq;
            if (spec?.IsVisible?.Invoke() == false)
                builder.AddAttribute(seq, "hidden", true);

            builder.AddAttribute(++seq, "disabled", spec?.IsDisabled?.Invoke());
            builder.AddAttribute(++seq, "required", spec?.IsRequired?.Invoke());

            ++seq;
            if (spec?.HighlightColor != null)
                builder.AddAttribute(seq, "style", $"--I4E-Highlight: {spec.HighlightColor.Invoke()};");

            if (spec?.Tooltip != null)
            {
                Tooltip? t = spec.Tooltip.Invoke();
                if (t != null)
                {
                    builder.AddAttribute(++seq, "data-i4e.tooltip-text",      t.Value.Text);
                    builder.AddAttribute(++seq, "data-i4e.tooltip-delay",     t.Value.Delay ?? 0);
                    builder.AddAttribute(++seq, "data-i4e.tooltip-follow",    t.Value.Follow.Map());
                    builder.AddAttribute(++seq, "data-i4e.tooltip-placement", t.Value.Placement.Map());
                }
                else seq += 4;
            }

            if (spec?.Data != null)
                foreach ((string name, Callbacks.DataValue getter) in spec.Data.Invoke())
                    builder.AddAttribute(++seq, "data-" + name, getter.Invoke());
        }

        internal static void ScheduleElementJobs(string id, SpecSet? spec, RenderTreeBuilder builder, ref int seq)
        {
            if (spec?.Tooltip == null) return;

            List<Action<ElementService>> jobs = new()
                {v => v.AddJob((j, t) => Interop.CreateTooltips(j, t, id))};

            ServiceInjector<ElementService>.Inject(builder, ref seq, jobs.ToArray());
        }
    }
}