using Superset.Web.Resources;

namespace Integrant4.Dominant
{
    public static class ResourceSets
    {
        public static readonly ResourceSet Interop = new ResourceSet
        (
            $"{nameof(Integrant4)}.{nameof(Dominant)}",
            nameof(Interop),
            scripts: new[] {"Dominant.js"}
        );
    }
}