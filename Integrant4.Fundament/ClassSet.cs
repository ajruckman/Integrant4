using System;
using System.Collections.Generic;
using System.Linq;

namespace Integrant4.Fundament
{
    public class ClassSet
    {
        private readonly List<string> _classes;
        private          string?      _formatted;

        public ClassSet(params string[] classes)
        {
            _classes   = classes.ToList();
            _formatted = null;
        }

        private ClassSet(List<string> classes)
        {
            _classes   = classes;
            _formatted = null;
        }

        public int Length => _classes.Count;

        //

        public static implicit operator ClassSet(string[]     v) => new(v);
        public static implicit operator ClassSet(List<string> v) => new(v);

        public static ClassSet operator +(ClassSet a, ClassSet b)
        {
            ClassSet result = a.Clone();
            result.AddRange(b._classes);
            return result;
        }

        //

        public void Add(string c)
        {
            if (_formatted != null) throw new Exception("ClassSet has already been finalized.");
            _classes.Add(c);
        }

        public void AddRange(IEnumerable<string> c)
        {
            if (_formatted != null) throw new Exception("ClassSet has already been finalized.");
            _classes.AddRange(c);
        }

        public override string ToString() => _formatted ??= string.Join(' ', _classes);

        public IEnumerable<string> Values() => _classes.AsReadOnly();

        public ClassSet Clone() => new(_classes.ToList());
    }
}