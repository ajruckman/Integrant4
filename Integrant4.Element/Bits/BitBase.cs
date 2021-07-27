using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Bits
{
    public abstract class BitBase : IBit
    {
        internal readonly SpecSet? InnerSpec;
        internal readonly SpecSet? OuterSpec;
        internal readonly string   ID;

        internal BitBase(SpecSet? outerSpec, SpecSet? innerSpec)
        {
            OuterSpec = outerSpec;
            InnerSpec = innerSpec;
            ID        = RandomIDGenerator.Generate();
        }

        internal BitBase(UnifiedSpec? spec)
        {
            OuterSpec = spec?.ToSpec();
            ID        = RandomIDGenerator.Generate();
        }

        internal BitBase(DualSpec? spec)
        {
            OuterSpec = spec?.ToOuterSpec();
            InnerSpec = spec?.ToInnerSpec();
            ID        = RandomIDGenerator.Generate();
        }

        public abstract RenderFragment Renderer();
    }

}