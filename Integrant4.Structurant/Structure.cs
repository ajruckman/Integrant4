using System;
using System.Collections.Generic;
using Microsoft.JSInterop;

namespace Integrant4.Structurant
{
    public class Structure<TObject, TState>
        where TObject : class
        where TState : class
    {
        private readonly Dictionary<string, IMember<TObject, TState>> _memberDictionary;
        private readonly List<IMember<TObject, TState>>               _members;

        private bool _instantiated;

        public Structure()
        {
            _members          = new List<IMember<TObject, TState>>();
            _memberDictionary = new Dictionary<string, IMember<TObject, TState>>();
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

        public void Register<TValue>(Member<TObject, TState, TValue> member)
        {
            if (_instantiated)
            {
                throw new Exception("Attempted to register member after structure has been instantiated.");
            }

            if (_memberDictionary.ContainsKey(member.ID))
            {
                throw new Exception("Attempted to register member with the same ID as an existing member.");
            }

            _members.Add(member);
            _memberDictionary[member.ID] = member;
        }

        public StructureInstance<TObject, TState> Instantiate
        (
            TState     state,
            IJSRuntime jsRuntime
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
        private readonly IJSRuntime                                           _jsRuntime;
        private readonly Dictionary<string, IMemberInstance<TObject, TState>> _memberInstanceDictionary;
        private readonly List<IMemberInstance<TObject, TState>>               _memberInstances;

        internal StructureInstance
        (
            Structure<TObject, TState> definition,
            TState                     state,
            IJSRuntime                 jsRuntime
        )
        {
            Definition = definition;
            _jsRuntime = jsRuntime;

            _memberInstanceDictionary = new Dictionary<string, IMemberInstance<TObject, TState>>();
            _memberInstances          = new List<IMemberInstance<TObject, TState>>();

            State = state;

            //

            foreach (IMember<TObject, TState> member in definition.Members)
            {
                IMemberInstance<TObject, TState> inst = member.Instantiate(this);
                _memberInstances.Add(inst);
                _memberInstanceDictionary[inst.Definition.ID] = inst;
            }
        }

        public Structure<TObject, TState> Definition { get; }

        public IReadOnlyList<IMemberInstance<TObject, TState>> MemberInstances => _memberInstances;

        public TState State { get; }

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
    }

    public interface IMember<TObject, TState>
        where TObject : class
        where TState : class
    {
        public Structure<TObject, TState> Structure { get; }
        public string                     ID        { get; }

        public IMemberInstance<TObject, TState> Instantiate
        (
            StructureInstance<TObject, TState> structureInstance
        );
    }

    public class Member<TObject, TState, TValue> : IMember<TObject, TState>
        where TObject : class
        where TState : class
    {
        public Member(Structure<TObject, TState> structure, string id)
        {
            ID        = id;
            Structure = structure;
        }

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
        public IMember<TObject, TState> Definition { get; }
    }

    public class MemberInstance<TObject, TState, TValue> : IMemberInstance<TObject, TState>
        where TObject : class
        where TState : class
    {
        private readonly Member<TObject, TState, TValue>    _member;
        private readonly StructureInstance<TObject, TState> _structureInstance;

        internal MemberInstance
        (
            Member<TObject, TState, TValue>    member,
            StructureInstance<TObject, TState> structureInstance
        )
        {
            _member            = member;
            _structureInstance = structureInstance;
        }

        public IMember<TObject, TState> Definition => _member;
    }
}