namespace Integrant4.Structurant
{
    public interface IMember<TObject, TState>
        where TObject : class
        where TState : class
    {
        Structure<TObject, TState> Structure                 { get; }
        string                     ID                        { get; }
        ushort                     InputDebounceMilliseconds { get; }

        IMemberInstance<TObject, TState> Instantiate(StructureInstance<TObject, TState> inst);
    }

    public partial class Member<TObject, TState, TValue> : IMember<TObject, TState>
        where TObject : class
        where TState : class
    {
        internal Member
        (
            Structure<TObject, TState>  structure,
            string                      id,
            ushort                      inputDebounceMilliseconds,
            Callbacks.ValueGetter       valueGetter,
            Callbacks.ValueSetter       valueSetter,
            Callbacks.InputGetter?      inputGetter      = null,
            Callbacks.ValidationGetter? validationGetter = null
        )
        {
            Structure                 = structure;
            ID                        = id;
            InputDebounceMilliseconds = inputDebounceMilliseconds;

            ValueGetter      = valueGetter;
            ValueSetter      = valueSetter;
            InputGetter      = inputGetter;
            ValidationGetter = validationGetter;
        }

        public Callbacks.ValueGetter       ValueGetter      { get; }
        public Callbacks.ValueSetter       ValueSetter      { get; }
        public Callbacks.InputGetter?      InputGetter      { get; }
        public Callbacks.ValidationGetter? ValidationGetter { get; }

        public Structure<TObject, TState> Structure                 { get; }
        public string                     ID                        { get; }
        public ushort                     InputDebounceMilliseconds { get; }

        public IMemberInstance<TObject, TState> Instantiate
        (
            StructureInstance<TObject, TState> inst
        ) => new MemberInstance<TObject, TState, TValue>(this, inst);
    }
}