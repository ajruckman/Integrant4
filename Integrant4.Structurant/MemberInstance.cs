using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Fundament;

namespace Integrant4.Structurant
{
    public interface IMemberInstance<TObject, TState>
        where TObject : class
        where TState : class
    {
        IMember<TObject, TState> Definition { get; }

        TState State { get; }

        Task                        ResetInputValue();
        void                        RefreshInput();
        IReadOnlyList<IValidation>? Validations();

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
        private readonly Debouncer<TValue?>              _debouncer;

        public readonly Hook InputHook       = new();
        public readonly Hook ValueChangeHook = new();

        internal MemberInstance
        (
            Member<TObject, TState, TValue>    definition,
            StructureInstance<TObject, TState> structureInstance
        )
        {
            _definition       = definition;
            StructureInstance = structureInstance;

            _debouncer = new Debouncer<TValue?>
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

        public void RefreshInput()
        {
            if (Input is IRefreshableInput<TValue> refreshable)
            {
                refreshable.Refresh();
            }
        }

        public IReadOnlyList<IValidation>? Validations() =>
            _definition.ValidationGetter == null
                ? null
                : _definition.ValidationGetter.Invoke(this);

        public event Action?          OnInput;
        public event Action<object?>? OnValueChangeUntyped;

        public TState State => StructureInstance.State;

        public TValue? Value()
        {
            StructureInstance.WriteLock.Wait();
            try
            {
                return _definition.ValueGetter.Invoke(this);
            }
            finally
            {
                StructureInstance.WriteLock.Release();
            }
        }

        public void SetValue(TValue?            v) => OnInputChange(v);
        public void SetValueImmediately(TValue? v) => OnInputChangeFinal(v);

        public void AttachInput(IInput<TValue> input) => input.OnChange += OnInputChange;
        public void DetachInput(IInput<TValue> input) => input.OnChange -= OnInputChange;

        private void OnInputChange(TValue? v)
        {
            OnInput?.Invoke();
            InputHook.Invoke();
            _debouncer.Reset(v);
        }

        private void OnInputChangeFinal(TValue? v)
        {
            StructureInstance.WriteLock.Wait();
            try
            {
                _definition.ValueSetter.Invoke(this, v);
                OnValueChange?.Invoke(_definition.ValueGetter.Invoke(this));
                ValueChangeHook.Invoke();
            }
            finally
            {
                StructureInstance.WriteLock.Release();
            }
        }

        public event Action<TValue?>? OnValueChange;
    }
}