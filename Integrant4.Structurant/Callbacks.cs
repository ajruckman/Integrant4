using Integrant4.API;
using Microsoft.JSInterop;

namespace Integrant4.Structurant
{
    public class Callbacks
    {
        public delegate IInput<TValue> InputCallback<TObject, TState, TValue>
        (
            MemberInstance<TObject, TState, TValue> inst
        ) where TObject : class where TState : class;
    }
}