using System.Threading.Tasks;

namespace Integrant4.Element.Inputs
{
    public interface IInputRequirable
    {
        Task<bool> IsRequired();
        Task       Require();
        Task       Unrequire();
    }

    public interface IInputDisableable
    {
        Task<bool> IsDisabled();
        Task       Disable();
        Task       Enable();
    }

    public interface IInputWithPlaceholder
    {
        Task<string?> GetPlaceholder();
        Task          SetPlaceholder(string? placeholder);
    }
}