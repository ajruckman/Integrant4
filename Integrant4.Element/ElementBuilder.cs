using System;
using System.Collections.Generic;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element
{
    public interface ISpec
    {
    }

    public interface IUnifiedSpec : ISpec
    {
        SpecSet ToSpec();
    }

    public interface IDualSpec : ISpec
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

            if (spec?.Tooltip == null)
            {
                Console.Write($"SEQ {seq} += 4 -> ");
            }
            else
            {
                Console.Write($"SEQ {seq} -> ");
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

            Console.WriteLine(seq);

            if (spec?.Data != null)
                foreach ((string name, Callbacks.DataValue getter) in spec.Data.Invoke())
                    builder.AddAttribute(++seq, "data-" + name, getter.Invoke());
        }

        internal static void ScheduleElementJobs(string id, SpecSet? spec, RenderTreeBuilder builder, ref int seq)
        {
            if (spec?.Tooltip == null) return;

            List<Action<ElementService>> jobs = new()
                { v => v.AddJob((j, t) => Interop.CreateTooltips(j, t, id)) };

            ServiceInjector<ElementService>.Inject(builder, ref seq, jobs.ToArray());
        }

        // internal static string ClassAttribute(ClassSet baseSet, SpecSet specSet, IEnumerable<string>? additional)
        // {
        //     ClassSet c = baseSet.Clone();
        //
        //     if (additional != null)
        //         c.AddRange(additional);
        //     if (specSet.Classes != null)
        //         c.AddRange(specSet.Classes.Invoke());
        //
        //     if (specSet.IsDisabled?.Invoke() == true)
        //         c.Add("I4E-Bit--Disabled");
        //     if (specSet.IsRequired?.Invoke() == true)
        //         c.Add("I4E-Bit--Required");
        //     if (!string.IsNullOrEmpty(specSet.HighlightColor?.Invoke()))
        //         c.Add("I4E-Bit--Highlighted");
        //
        //     return c.ToString();
        // }
        //
        // internal static string? ContentStyleAttribute(SpecSet specSet)
        // {
        //     List<string> result = new(3);
        //
        //     if (specSet.FontSize != null)
        //         result.Add($"font-size: {specSet.FontSize.Invoke()}rem;");
        //
        //     if (specSet.FlexAlign != null)
        //         result.Add($"align-items: {specSet.FlexAlign.Invoke().Serialize()}");
        //
        //     if (specSet.FlexJustify != null)
        //         result.Add($"justify-content: {specSet.FlexJustify.Invoke().Serialize()}");
        //
        //     return result.Any() ? string.Join(' ', result) : null;
        // }
    }
}