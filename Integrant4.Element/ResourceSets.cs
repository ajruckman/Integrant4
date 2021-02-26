using Superset.Web.Resources;

namespace Integrant4.Element
{
    public static class ResourceSets
    {
        public static class Inputs
        {
            public static readonly ResourceSet Interop = new ResourceSet
            (
                $"{nameof(Integrant4)}.{nameof(Element)}",
                $"{nameof(Inputs)}.{nameof(Interop)}",
                scripts: new[] {"Inputs.js"}
            );
        }
    }
}