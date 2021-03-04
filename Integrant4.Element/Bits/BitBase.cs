using System;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Bits
{
    public abstract class BitBase : IBit
    {
        internal readonly BitSpec  BaseSpec;
        internal readonly ClassSet BaseClasses;
        internal readonly string   ID;

        internal BitBase(BitSpec? spec, ClassSet baseClasses)
        {
            BaseSpec    = spec ?? new BitSpec();
            BaseClasses = baseClasses;
            ID          = RandomIDGenerator.Generate();

            EnsureServicesAvailable();
        }

        public abstract RenderFragment Renderer();

        protected void QueueTooltip()
        {
            if (BaseSpec.Tooltip == null) return;
            
            BaseSpec.ElementService!.AddJob(v => Interop.CreateBitTooltips(v, ID));
        }

        private void EnsureServicesAvailable()
        {
            if (BaseSpec.ElementService != null) return;

            if (BaseSpec.Tooltip != null)
                throw new Exception($"Bit has a tooltip callback set, but no ElementService was supplied.");
        }
    }
}