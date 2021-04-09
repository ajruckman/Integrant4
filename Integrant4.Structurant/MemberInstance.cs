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

        Task                              ResetInputValue();
        Task                              RefreshInput();
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
            StructureInstance<TObject, TState> structureInstance
        )
        {
            _definition       = definition;
            StructureInstance = structureInstance;

            _debouncer = new Utility.Debouncer<TValue?>
            (
                OnInputChangeFinal,
                definition.ValueGetter.Invoke(this),
                definition.InputDebounceMilliseconds
            );

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

        public async Task ResetInputValue()
        {
            if (Input != null)
                await Input.SetValue(Value());
        }

        public async Task RefreshInput()
        {
            if (Input is IRefreshableInput<TValue> refreshable)
            {
                await refreshable.Refresh();
            }
        }

        public event Action<object?>? OnValueChangeUntyped;

        public async Task<IReadOnlyList<IValidation>?> Validations() =>
            _definition.ValidationGetter == null
                ? null
                : await _definition.ValidationGetter.Invoke(this);

        public event Action? OnInput;

        public TValue? Value()
        {
            StructureInstance.WriteLock.EnterReadLock();
            try
            {
                return _definition.ValueGetter.Invoke(this);
            }
            finally
            {
                StructureInstance.WriteLock.ExitReadLock();
            }
        }

        public void SetValue(TValue?            v) => OnInputChange(v);
        public void SetValueImmediately(TValue? v) => OnInputChangeFinal(v);

        public void AttachInput(IInput<TValue> input) => input.OnChange += OnInputChange;
        public void DetachInput(IInput<TValue> input) => input.OnChange -= OnInputChange;

        private void OnInputChange(TValue? v)
        {
            OnInput?.Invoke();
            _debouncer.Reset(v);
        }

        private void OnInputChangeFinal(TValue? v)
        {
            StructureInstance.WriteLock.EnterWriteLock();
            try
            {
                _definition.ValueSetter.Invoke(this, v);
                OnValueChange?.Invoke(Value());
            }
            finally
            {
                StructureInstance.WriteLock.ExitWriteLock();
            }
        }

        public event Action<TValue?>? OnValueChange;
    }
}