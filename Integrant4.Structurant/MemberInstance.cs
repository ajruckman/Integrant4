using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Integrant4.API;

namespace Integrant4.Structurant
{
    public interface IMemberInstance<TObject, TState>
        where TObject : class
        where TState : class
    {
        IMember<TObject, TState> Definition { get; }

        void                              ResetInputValue();
        Task<IReadOnlyList<IValidation>?> Validations();

        /// <summary>
        /// Non-debounced event called as soon as new input is received.
        /// </summary>
        event Action? OnInput;

        /// <summary>
        /// Untyped event that an IStructureInstance can subscribe to without knowing TValue.
        /// </summary>
        event Action<object?>? OnValueChangeUntyped;
    }

    public class MemberInstance<TObject, TState, TValue> : IMemberInstance<TObject, TState>
        where TObject : class
        where TState : class
    {
        private readonly Member<TObject, TState, TValue> _definition;
        private readonly Utility.Debouncer<TValue?>      _debouncer;

        internal MemberInstance
        (
            Member<TObject, TState, TValue>    definition,
            StructureInstance<TObject, TState> structureStructureInstance
        )
        {
            _definition = definition;
            _debouncer = new Utility.Debouncer<TValue?>
            (
                OnInputChangeDebounced,
                definition.ValueGetter.Invoke(this),
                definition.InputDebounceMilliseconds
            );

            StructureInstance = structureStructureInstance;

            if (definition.InputGetter != null)
            {
                Input = definition.InputGetter.Invoke(this);
                AttachInput(Input);
            }

            OnValueChange += v => OnValueChangeUntyped?.Invoke(v);
        }

        public StructureInstance<TObject, TState> StructureInstance { get; }
        public IInput<TValue>?                    Input             { get; }

        public IMember<TObject, TState> Definition => _definition;

        public void ResetInputValue()
        {
            Input?.SetValue(Value());
        }

        public event Action<object?>? OnValueChangeUntyped;

        public async Task<IReadOnlyList<IValidation>?> Validations() =>
            _definition.ValidationGetter == null
                ? null
                : await _definition.ValidationGetter.Invoke(this);

        public event Action? OnInput;

        public TValue? Value() => _definition.ValueGetter.Invoke(this);

        public void SetValue(TValue?            v) => OnInputChange(v);
        public void SetValueImmediately(TValue? v) => OnInputChangeDebounced(v);

        public void AttachInput(IInput<TValue> input) => input.OnChange += OnInputChange;
        public void DetachInput(IInput<TValue> input) => input.OnChange -= OnInputChange;

        private void OnInputChange(TValue? v)
        {
            OnInput?.Invoke();
            _debouncer.Reset(v);
        }

        private void OnInputChangeDebounced(TValue? v)
        {
            _definition.ValueSetter.Invoke(this, v);
            OnValueChange?.Invoke(Value());
        }

        public event Action<TValue?>? OnValueChange;
    }
}