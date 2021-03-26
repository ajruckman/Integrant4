using System;
using System.Threading.Tasks;

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

    public interface IRefreshableInput<TValue> : IInput<TValue>
    {
        public Task Refresh();
    }
}