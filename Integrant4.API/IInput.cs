using System;
using System.Threading.Tasks;

namespace Integrant4.API
{
    public interface IInput<TValue>
    {
        Task<TValue?> GetValue();
        Task          SetValue(TValue? value);

        event Action<TValue?> OnChange;
    }
}