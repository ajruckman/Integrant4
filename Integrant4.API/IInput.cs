using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Integrant4.API
{
    public interface IInput<TValue> : IRenderable
    {
        Task<TValue?> GetValue();
        Task          SetValue(TValue? value);

        event Action<TValue?> OnChange;
    }

    public interface IValidatableInput<TValue> : IInput<TValue>
    {
        // delegate 
    }
}