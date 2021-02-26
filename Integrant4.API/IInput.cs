using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Integrant4.API
{
    public interface IInput<TValue>
    {
        Task<TValue?>  GetValue();
        Task           SetValue(TValue? value);
        RenderFragment Render();

        event Action<TValue?> OnChange;
    }
}