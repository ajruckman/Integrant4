using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Inputs
{
    internal static class InputBuilder
    {
        internal static void ApplyOuterAttributes<T>
        (
            InputBase<T>         inputBase,
            RenderTreeBuilder    builder,
            ref int              seq,
            IEnumerable<string>? additionalClasses = null,
            IEnumerable<string>? additionalStyles  = null
        ) => ElementBuilder.ApplyAttributes(inputBase.ID, inputBase.OuterSpec, builder, ref seq,
            additionalClasses, additionalStyles);

        internal static void ApplyInnerAttributes<T>
        (
            InputBase<T>         inputBase,
            RenderTreeBuilder    builder,
            ref int              seq,
            IEnumerable<string>? additionalClasses = null,
            IEnumerable<string>? additionalStyles  = null
        ) => ElementBuilder.ApplyAttributes(inputBase.ID, inputBase.InnerSpec, builder, ref seq,
            additionalClasses, additionalStyles);

        internal static void ScheduleElementJobs<T>(InputBase<T> inputBase, RenderTreeBuilder builder, ref int seq)
        {
            ElementBuilder.ScheduleElementJobs(inputBase.ID, inputBase.OuterSpec, builder, ref seq);
            ElementBuilder.ScheduleElementJobs(inputBase.ID, inputBase.InnerSpec, builder, ref seq);
        }
    }
}