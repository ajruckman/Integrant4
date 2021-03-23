using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Integrant4.Fundament;
using Microsoft.JSInterop;

namespace Integrant4.Structurant
{
    public class StructureInstance<TObject, TState>
        where TObject : class
        where TState : class
    {
        private readonly List<IMemberInstance<TObject, TState>>               _memberInstances;
        private readonly Dictionary<string, IMemberInstance<TObject, TState>> _memberInstanceDictionary;

        public readonly ValidationState ValidationState;

        internal StructureInstance
        (
            Structure<TObject, TState> definition,
            TState                     state,
            IJSRuntime?                jsRuntime
        )
        {
            _memberInstanceDictionary = new Dictionary<string, IMemberInstance<TObject, TState>>();
            _memberInstances          = new List<IMemberInstance<TObject, TState>>();

            ValidationState = new ValidationState();

            Definition = definition;
            State      = state;
            JSRuntime  = jsRuntime;

            //

            foreach (IMember<TObject, TState> member in definition.Members)
            {
                IMemberInstance<TObject, TState> inst = member.Instantiate(this);

                inst.OnInput += () => ValidationState.Invalidate();
                inst.OnValueChangeUntyped += v =>
                {
                    OnMemberValueChange?.Invoke(inst, v);
                    ValidationState.ValidateStructure(this);
                };

                _memberInstances.Add(inst);
                _memberInstanceDictionary[inst.Definition.ID] = inst;
            }

            // Initial validation
            ValidationState.ValidateStructure(this);
        }

        public Structure<TObject, TState> Definition { get; }
        public TState                     State      { get; }
        public IJSRuntime?                JSRuntime  { get; }

        public IReadOnlyList<IMemberInstance<TObject, TState>> MemberInstances => _memberInstances;

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

        public async Task<TObject> Construct()
        {
            return await Definition.ResultConstructor.Invoke(this);
        }

        public async Task ResetAllMemberInputValues()
        {
            foreach (IMemberInstance<TObject, TState> inst in _memberInstances)
            {
                await inst.ResetInputValue();
            }
        }

        // // Validation proxies
        //
        // public IReadOnlyList<IValidation>? OverallValidations() =>
        //     ValidationState.Result?.OverallValidations;
        //
        // public IReadOnlyList<IValidation>? MemberValidations(string id) =>
        //     ValidationState.Result?.MemberValidations[id];
    }
}