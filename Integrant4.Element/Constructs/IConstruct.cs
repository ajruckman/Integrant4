using System.Threading.Tasks;
using Integrant4.API;

namespace Integrant4.Element.Constructs
{
    public interface IConstruct : IRenderable { }

    public interface IRefreshableConstruct : IConstruct
    {
        public Task Refresh();
    }
}