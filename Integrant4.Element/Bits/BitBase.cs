using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Bits
{
    public abstract class BitBase : IBit
    {
        internal readonly SpecSet? InnerSpec;
        internal readonly SpecSet? OuterSpec;
        internal readonly string   ID;
        internal readonly string?  LoggingID;

        internal BitBase(IUnifiedSpec spec, string? loggingID = null)
        {
            OuterSpec = spec.ToSpec();
            ID        = RandomIDGenerator.Generate();
            LoggingID = loggingID;
        }

        internal BitBase(IDualSpec spec, string? loggingID = null)
        {
            OuterSpec = spec.ToOuterSpec();
            InnerSpec = spec.ToInnerSpec();
            ID        = RandomIDGenerator.Generate();
            LoggingID = loggingID;
        }

        public abstract RenderFragment Renderer();
    }
}