using Integrant4.API;

namespace Integrant4.Element.Bits
{
    public interface IBit : IRenderable { }

    public interface IRefreshableBit : IBit
    {
        public void Refresh();
    }
}