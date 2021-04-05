using Integrant4.API;

namespace Integrant4.Resources.Icons
{
    public interface IIcon : IRenderable
    {
        public string ID { get; }
    }
}