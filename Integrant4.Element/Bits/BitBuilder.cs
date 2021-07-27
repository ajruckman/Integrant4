using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Bits
{
    internal static class BitBuilder
    {
        internal static void ApplyOuterAttributes
        (
            BitBase              bitBase,
            RenderTreeBuilder    builder,
            ref int              seq,
            IEnumerable<string>? additionalClasses = null,
            IEnumerable<string>? additionalStyles  = null
        ) => ElementBuilder.ApplyAttributes(bitBase.ID, bitBase.OuterSpec, builder, ref seq,
            additionalClasses, additionalStyles);

        internal static void ApplyInnerAttributes
        (
            BitBase              bitBase,
            RenderTreeBuilder    builder,
            ref int              seq,
            IEnumerable<string>? additionalClasses = null,
            IEnumerable<string>? additionalStyles  = null
        ) => ElementBuilder.ApplyAttributes(bitBase.ID, bitBase.InnerSpec, builder, ref seq,
            additionalClasses, additionalStyles);

        internal static void ScheduleElementJobs(BitBase bitBase, RenderTreeBuilder builder, ref int seq)
        {
            ElementBuilder.ScheduleElementJobs(bitBase.ID, bitBase.OuterSpec, builder, ref seq);
            ElementBuilder.ScheduleElementJobs(bitBase.ID, bitBase.InnerSpec, builder, ref seq);
        }
    }
}