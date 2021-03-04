using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Bits
{
    public abstract class BitBase : IBit
    {
        internal readonly BitSpec  BaseSpec;
        internal readonly ClassSet BaseClasses;

        // protected string? _cachedClassAttribute;
        // protected string? _cachedStyleAttribute;

        internal BitBase(BitSpec? spec, ClassSet baseClasses)
        {
            BaseSpec    = spec ?? new BitSpec();
            BaseClasses = baseClasses;

            // if (spec.IsStatic)
            // {
            //     _cachedClassAttribute = BitBuilder.ClassAttribute(baseClasses, spec, additionalClasses);
            //     _cachedStyleAttribute = BitBuilder.StyleAttribute(spec);
            // }
        }

        public abstract RenderFragment Renderer();
    }
}