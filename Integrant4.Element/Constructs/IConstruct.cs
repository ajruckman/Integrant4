using Integrant4.API;

namespace Integrant4.Element.Constructs
{
    public interface IConstruct : IRenderable { }

    public interface IRefreshableConstruct : IConstruct
    {
        public void Refresh();
    }
}