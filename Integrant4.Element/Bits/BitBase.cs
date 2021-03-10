using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Bits
{
    public abstract class BitBase : IBit
    {
        internal readonly BaseSpec BaseSpec;
        internal readonly ClassSet BaseClasses;
        internal readonly string   ID;

        internal BitBase(BaseSpec? spec, ClassSet classes)
        {
            BaseSpec    = spec ?? new BaseSpec();
            BaseClasses = classes;
            ID          = RandomIDGenerator.Generate();
        }

        public abstract RenderFragment Renderer();
    }
}