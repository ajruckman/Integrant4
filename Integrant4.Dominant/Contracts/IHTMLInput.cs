using System.Threading.Tasks;
using Integrant4.API;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Dominant.Contracts
{
    public interface IHTMLInput<T> : IInput<T>
    {
        RenderFragment Render();
    }

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