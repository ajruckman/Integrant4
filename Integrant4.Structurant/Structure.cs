using System;
using System.Collections.Generic;
using Microsoft.JSInterop;

namespace Integrant4.Structurant
{
    public partial class Structure<TObject, TState>
        where TObject : class
        where TState : class
    {
        private readonly Dictionary<string, IMember<TObject, TState>> _memberDictionary;
        private readonly List<IMember<TObject, TState>>               _members;

        private bool _instantiated;

        public Structure
        (
            Callbacks.ResultConstructor resultConstructor,
            Callbacks.ValidationGetter? validationGetter = null
        )
        {
            _members          = new List<IMember<TObject, TState>>();
            _memberDictionary = new Dictionary<string, IMember<TObject, TState>>();

            ResultConstructor = resultConstructor;
            ValidationGetter  = validationGetter;
        }

        public Callbacks.ResultConstructor ResultConstructor { get; }
        public Callbacks.ValidationGetter? ValidationGetter  { get; }

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
            string                                                      id,
            Member<TObject, TState, TValue>.Callbacks.ValueGetter       valueGetter,
            Member<TObject, TState, TValue>.Callbacks.ValueSetter       valueSetter,
            ushort                                                      inputDebounceMilliseconds = 200,
            Member<TObject, TState, TValue>.Callbacks.InputGetter?      inputGetter               = null,
            Member<TObject, TState, TValue>.Callbacks.ValidationGetter? validationGetter          = null
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

            Member<TObject, TState, TValue> member = new Member<TObject, TState, TValue>
            (
                this,
                id,
                valueGetter,
                valueSetter,
                inputDebounceMilliseconds,
                inputGetter,
                validationGetter
            );
            
            _members.Add(member);
            _memberDictionary[member.ID] = member;
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
}