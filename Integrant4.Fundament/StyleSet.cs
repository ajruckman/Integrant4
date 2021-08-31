using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Integrant4.Fundament
{
    public class StyleSet : IEnumerable
    {
        private readonly Dictionary<string, string?> _styles = new();

        private string? _formatted;
        private bool    _finalized;

        public StyleSet()
        {
            _formatted = null;
        }

        public IEnumerator GetEnumerator()
        {
            return _styles.GetEnumerator();
        }

        public void Add(string key, string? value)
        {
            if (_formatted != null) throw new Exception("StyleSet has already been finalized.");

            if (!string.IsNullOrWhiteSpace(value))
                _styles[key] = value;
        }

        public override string? ToString()
        {
            if (_finalized) return _formatted!;
            _finalized = true;

            return _formatted = _styles.Any()
                ? string.Join(";", _styles.Select(v => $"{v.Key}:{v.Value}"))
                : null;
        }
    }
}