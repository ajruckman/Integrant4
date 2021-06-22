using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Fundament;
using Microsoft.JSInterop;

namespace Integrant4.Structurant
{
    public class StructureInstance<TObject, TState> : IAsyncDisposable
        where TObject : class
        where TState : class
    {
        private readonly List<IMemberInstance<TObject, TState>>               _memberInstances;
        private readonly Dictionary<string, IMemberInstance<TObject, TState>> _memberInstanceDictionary;

        private readonly StructureValidationState _validationState;

        public readonly Hook MemberValueChangeHook = new();

        internal readonly SemaphoreSlim WriteLock = new(1);

        internal StructureInstance
        (
            Structure<TObject, TState> definition,
            TState                     state,
            IJSRuntime?                jsRuntime
        )
        {
            _memberInstanceDictionary = new Dictionary<string, IMemberInstance<TObject, TState>>();
            _memberInstances          = new List<IMemberInstance<TObject, TState>>();

            _validationState = new StructureValidationState();

            Definition = definition;
            State      = state;
            JSRuntime  = jsRuntime;

            //

            foreach (IMember<TObject, TState> member in definition.Members)
            {
                IMemberInstance<TObject, TState> inst = member.Instantiate(this);

                inst.OnInput += () => _validationState.Invalidate();
                inst.OnValueChangeUntyped += v =>
                {
                    OnMemberValueChange?.Invoke(inst, v);
                    MemberValueChangeHook.Invoke();
                    _validationState.ValidateStructure(this);
                };

                _memberInstances.Add(inst);
                _memberInstanceDictionary[inst.Definition.ID] = inst;
            }

            // Initial validation
            _validationState.ValidateStructure(this);
        }

        public IValidationState ValidationState => _validationState;

        public Structure<TObject, TState> Definition { get; }
        public TState                     State      { get; }
        public IJSRuntime?                JSRuntime  { get; }

        public IReadOnlyList<IMemberInstance<TObject, TState>> MemberInstances => _memberInstances;

        public async ValueTask DisposeAsync()
        {
            foreach (IMemberInstance<TObject, TState> m in MemberInstances)
            {
                await m.DisposeAsync();
            }
        }

        public event Action<IMemberInstance<TObject, TState>, object?>? OnMemberValueChange;

        public IMemberInstance<TObject, TState>? Get(string id)
        {
            _memberInstanceDictionary.TryGetValue(id, out IMemberInstance<TObject, TState>? inst);
            return inst;
        }

        public MemberInstance<TObject, TState, TValue> GetTyped<TValue>(string id)
        {
            _memberInstanceDictionary.TryGetValue(id, out IMemberInstance<TObject, TState>? inst);
            if (inst == null)
                throw new ArgumentException($"No member with passed ID '{id}' has been registered.",
                    nameof(id));

            if (inst is not MemberInstance<TObject, TState, TValue> instT)
                throw new ArgumentException("TValue passed to GetTyped<TValue>() was incorrect for this member ID.",
                    nameof(TValue));

            return instT;
        }

        public void Construct(Action<Exception>? exceptionHandler = null, Action<TObject>? then = null)
        {
            var t = new Task(async () =>
            {
                await WriteLock.WaitAsync();
                try
                {
                    TObject? result = await Definition.ResultConstructor.Invoke(this);

                    then?.Invoke(result);
                }
                catch (Exception e)
                {
                    exceptionHandler?.Invoke(e);
                }
                finally
                {
                    WriteLock.Release();
                }
            });

            t.Start();
        }

        public async Task ResetAllMemberInputValues()
        {
            foreach (IMemberInstance<TObject, TState> inst in _memberInstances)
            {
                await inst.ResetInputValue();
            }
        }

        public void RefreshAllMemberInputs()
        {
            foreach (var inst in _memberInstances)
            {
                inst.RefreshInput();
            }
        }

        public void Revalidate()
        {
            _validationState.ValidateStructure(this);
        }

        // Safety methods

        public async Task EnterWriteLock() => await WriteLock.WaitAsync();
        public async Task ExitWriteLock()  => await WriteLock.WaitAsync();

        // Proxy methods

        public IReadOnlyList<IValidation>? OverallValidations() =>
            _validationState.Result?.OverallValidations;

        public IReadOnlyList<IValidation>? MemberValidations(string id) =>
            _validationState.Result?.MemberValidations[id];

        public IInput<TValue>? GetInput<TValue>(string id) =>
            GetTyped<TValue>(id).Input;

        public TInput? GetInputTyped<TValue, TInput>(string id) where TInput : class, IInput<TValue> =>
            GetTyped<TValue>(id).Input as TInput;
    }
}