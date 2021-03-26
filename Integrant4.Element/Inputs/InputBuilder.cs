using System;
using System.Collections.Generic;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Inputs
{
    internal static class InputBuilder
    {
        internal static void ApplyOuterAttributes<T>
        (
            InputBase<T>      inputBase,
            RenderTreeBuilder builder,
            ref int           seq,
            string[]?         additionalClasses
        )
        {
            builder.AddAttribute(++seq, "id", inputBase.ID);
            builder.AddAttribute(++seq, "class",
                ElementBuilder.ClassAttribute(inputBase.BaseClasses, inputBase.BaseSpec, additionalClasses));

            ++seq;
            if (inputBase.BaseSpec.IsVisible?.Invoke() == false)
                builder.AddAttribute(seq, "hidden", true);

            ++seq;
            if (inputBase.BaseSpec.Tooltip != null)
                builder.AddAttribute(seq, "data-i4e.tooltip", inputBase.BaseSpec.Tooltip.Invoke());

            if (inputBase.BaseSpec.Data != null)
                foreach ((string name, Callbacks.DataValue getter) in inputBase.BaseSpec.Data.Invoke())
                    builder.AddAttribute(++seq, "data-" + name, getter.Invoke());
        }

        internal static void ApplyInnerAttributes<T>
        (
            InputBase<T>      inputBase,
            RenderTreeBuilder builder,
            ref int           seq,
            string[]?         additionalStyles
        )
        {
            builder.AddAttribute(++seq, "id",          $"{inputBase.ID}.Inner");

            builder.AddAttribute(++seq, "disabled",    inputBase.BaseSpec.IsDisabled?.Invoke());
            builder.AddAttribute(++seq, "required",    inputBase.BaseSpec.IsRequired?.Invoke());

            builder.AddAttribute(++seq, "style",
                ElementBuilder.StyleAttribute(inputBase.BaseSpec, additionalStyles));
        }

        internal static void ScheduleElementJobs<T>(InputBase<T> inputBase, RenderTreeBuilder builder, ref int seq)
        {
            if (inputBase.BaseSpec.Tooltip == null) return;

            List<Action<ElementService>> jobs = new();

            if (inputBase.BaseSpec.Tooltip != null)
                jobs.Add(v => v.AddJob(x => Interop.CreateTooltips(x, v.CancellationToken, inputBase.ID)));

            if (jobs.Count > 0)
                ServiceInjector<ElementService>.Inject(builder, ref seq, jobs.ToArray());
        }
    }
}