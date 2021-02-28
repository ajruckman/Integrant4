using System.Collections.Generic;
using System.Threading.Tasks;
using Integrant4.API;

namespace Integrant4.Structurant
{
    public partial class Structure<TObject, TState>
    {
        public static class Callbacks
        {
            public delegate TObject ResultConstructor(StructureInstance<TObject, TState> inst);

            public delegate Task<IReadOnlyList<IValidation>> ValidationGetter(StructureInstance<TObject, TState> inst);
        }
    }

    public partial class Member<TObject, TState, TValue>
    {
        public static class Callbacks
        {
            public delegate TValue? ValueGetter(MemberInstance<TObject, TState, TValue> inst);

            public delegate void ValueSetter(MemberInstance<TObject, TState, TValue> inst, TValue v);

            public delegate IInput<TValue> InputGetter(MemberInstance<TObject, TState, TValue> inst);

            public delegate Task<IReadOnlyList<IValidation>> ValidationGetter(
                MemberInstance<TObject, TState, TValue> inst);
        }
    }
}