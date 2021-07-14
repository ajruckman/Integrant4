using System;
using System.Threading.Tasks;

namespace Integrant4.API
{
    public interface IInput<TValue> : IRenderable
    {
        TValue?       GetValue();
        Task<TValue?> ReadValue();

        event Action<TValue?> OnChange;
    }

    public interface IWritableInput<TValue> : IInput<TValue>
    {
        Task SetValue(TValue? value, bool invokeOnChange = true);
    }

    public interface IRefreshableInput<TValue> : IInput<TValue>
    {
        public void Refresh();
    }

    public interface IWritableRefreshableInput<TValue> : IWritableInput<TValue>
    {
        public void Refresh();
    }
}