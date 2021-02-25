using System;
using System.Collections.Generic;
using Integrant4.API;
using Microsoft.JSInterop;

namespace Integrant4.Structurant
{
    public class Structure<TObject, TState>
        where TObject : class
        where TState : class
    {
        public delegate TObject ResultConstructorCallback(StructureInstance<TObject, TState> instance);

        private readonly Dictionary<string, IMember<TObject, TState>> _memberDictionary;
        private readonly List<IMember<TObject, TState>>               _members;

        internal readonly ResultConstructorCallback ResultConstructor;

        private bool _instantiated;

        public Structure(ResultConstructorCallback resultConstructor)
        {
            _members          = new List<IMember<TObject, TState>>();
            _memberDictionary = new Dictionary<string, IMember<TObject, TState>>();

            ResultConstructor = resultConstructor;
        }

        public IReadOnlyList<IMember<TObject, TState>> Members => _members;

        public IMember<TObject, TState>? Get(string id)
        {
            _memberDictionary.TryGetValue(id, out IMember<TObject, TState>? member);
            return member;
        }

        public Member<TObject, TState, TValue>? GetTyped<TValue>(string id)
        {
            _memberDictionary.TryGetValue(id, out IMember<TObject, TState>? member);
            return member as Member<TObject, TState, TValue>;
        }

        /// <summary>
        /// Register a new member.
        /// </summary>
        /// <typeparam name="TValue">The type of the value that this member controls.</typeparam>
        public void Register<TValue>
        (
            string                                            id,
            Callbacks.InputCallback<TObject, TState, TValue>? inputCallback = null
        )
        {
            if (_instantiated)
            {
                throw new Exception("Attempted to register member after structure has been instantiated.");
            }

            if (_memberDictionary.ContainsKey(id))
            {
                throw new Exception("Attempted to register member with the same ID as an existing member.");
            }

            Member<TObject, TState, TValue> member = new(this, id, inputCallback);
            _members.Add(member);
            _memberDictionary[id] = member;
        }

        public StructureInstance<TObject, TState> Instantiate
        (
            TState      state,
            IJSRuntime? jsRuntime
        )
        {
            _instantiated = true;
            return new(this, state, jsRuntime);
        }
    }

    public class StructureInstance<TObject, TState>
        where TObject : class
        where TState : class
    {
        public readonly IJSRuntime? JSRuntime;

        private readonly Dictionary<string, IMemberInstance<TObject, TState>> _memberInstanceDictionary;
        private readonly List<IMemberInstance<TObject, TState>>               _memberInstances;

        internal StructureInstance
        (
            Structure<TObject, TState> definition,
            TState                     state,
            IJSRuntime?                jsRuntime
        )
        {
            Definition = definition;
            JSRuntime  = jsRuntime;

            _memberInstanceDictionary = new Dictionary<string, IMemberInstance<TObject, TState>>();
            _memberInstances          = new List<IMemberInstance<TObject, TState>>();

            State = state;

            //

            foreach (IMember<TObject, TState> member in definition.Members)
            {
                IMemberInstance<TObject, TState> inst = member.Instantiate(this);

                inst.OnValueChangeUntyped += v => OnMemberValueChange?.Invoke(inst, v);

                _memberInstances.Add(inst);
                _memberInstanceDictionary[inst.Definition.ID] = inst;
            }
        }

        public Structure<TObject, TState> Definition { get; }

        public IReadOnlyList<IMemberInstance<TObject, TState>> MemberInstances => _memberInstances;

        public TState State { get; }

        public event Action<IMemberInstance<TObject, TState>, object?> OnMemberValueChange;

        public IMemberInstance<TObject, TState>? Get(string id)
        {
            _memberInstanceDictionary.TryGetValue(id, out IMemberInstance<TObject, TState>? inst);
            return inst;
        }

        public MemberInstance<TObject, TState, TValue>? GetTyped<TValue>(string id)
        {
            _memberInstanceDictionary.TryGetValue(id, out IMemberInstance<TObject, TState>? inst);
            return inst as MemberInstance<TObject, TState, TValue>;
        }

        public TObject Construct()
        {
            return Definition.ResultConstructor.Invoke(this);
        }

        public void ResetAllMemberInputValues()
        {
            foreach (IMemberInstance<TObject, TState> inst in _memberInstances)
            {
                inst.ResetInputValue();
            }
        }
    }

    public interface IMember<TObject, TState>
        where TObject : class
        where TState : class
    {
        Structure<TObject, TState> Structure { get; }
        string                     ID        { get; }

        IMemberInstance<TObject, TState> Instantiate
        (
            StructureInstance<TObject, TState> structureInstance
        );
    }

    public class Member<TObject, TState, TValue> : IMember<TObject, TState>
        where TObject : class
        where TState : class
    {
        internal Member
        (
            Structure<TObject, TState>                        structure,
            string                                            id,
            Callbacks.InputCallback<TObject, TState, TValue>? inputCallback
        )
        {
            ID            = id;
            InputCallback = inputCallback;
            Structure     = structure;
        }

        public Callbacks.InputCallback<TObject, TState, TValue>? InputCallback { get; }

        public Structure<TObject, TState> Structure { get; }
        public string                     ID        { get; }

        public IMemberInstance<TObject, TState> Instantiate
        (
            StructureInstance<TObject, TState> structureInstance
        ) => new MemberInstance<TObject, TState, TValue>(this, structureInstance);
    }

    public interface IMemberInstance<TObject, TState>
        where TObject : class
        where TState : class
    {
        IMember<TObject, TState> Definition { get; }

        void ResetInputValue();

        event Action<object?>? OnValueChangeUntyped;
    }

    public class MemberInstance<TObject, TState, TValue> : IMemberInstance<TObject, TState>
        where TObject : class
        where TState : class
    {
        private readonly Member<TObject, TState, TValue> _member;

        internal MemberInstance
        (
            Member<TObject, TState, TValue>    member,
            StructureInstance<TObject, TState> structureStructureInstance
        )
        {
            _member           = member;
            StructureInstance = structureStructureInstance;

            if (member.InputCallback != null)
            {
                Input = member.InputCallback.Invoke(this);
                Input.OnChange += v =>
                {
                    Value = v;
                    OnValueChangeUntyped?.Invoke(Value);
                    OnValueChange?.Invoke(Value);
                };
            }
        }

        public StructureInstance<TObject, TState> StructureInstance { get; }

        public TValue? Value { get; private set; }

        // public void AttachInput(IInput<TValue> input)
        // {
        //     input.OnChange += v => Value = v;
        // }

        //

        public IInput<TValue>? Input { get; }

        public IMember<TObject, TState> Definition => _member;

        // public IInput<TValue>? Input()
        // {
        //     if (_inputCallback == null) return null;
        //
        //     IInput<TValue> input = _inputCallback.Invoke();
        //     AttachInput(input);
        //
        //     return input;
        // }

        public void ResetInputValue()
        {
            Input?.SetValue(Value);
        }

        public event Action<object?>? OnValueChangeUntyped;

        public event Action<TValue?>? OnValueChange;
    }
}