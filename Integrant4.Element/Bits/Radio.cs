using System;
using System.Linq;
using Integrant4.Fundament;
using Integrant4.Resources.Icons;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Bits
{
    public partial class Radio<TKey> : IBit where TKey : IEquatable<TKey>
    {
        private readonly Spec? _spec;

        public Radio(OptionGetter optionGetter, Spec? spec = null)
        {
            _optionGetter = optionGetter;
            _spec         = spec;

            _selection = _spec?.Value == null
                ? Options().First().Key
                : _spec.Value.Invoke();
        }
    }

    public partial class Radio<TKey>
    {
        public delegate (TKey, Content)[] OptionGetter();

        private readonly OptionGetter _optionGetter;
        private readonly object       _optionsLock = new();

        private (TKey Key, Content Option)[]? _options;
        private TKey                          _selection;

        public TKey GetValue() => _selection;

        public event Action<TKey>? OnChange;

        private (TKey Key, Content Content)[] Options()
        {
            lock (_optionsLock)
            {
                _options ??= _optionGetter.Invoke();
                if (_options.Length < 2)
                    throw new Exception(
                        "OptionGetter passed to Radio Bit returned fewer than 2 options.");
                return _options;
            }
        }

        private void SetValue(TKey value)
        {
            _selection = value;
            OnChange?.Invoke(value);
            _refresher?.Invoke();
        }

        public void Invalidate()
        {
            lock (_optionsLock)
            {
                _options = null;
            }

            _refresher?.Invoke();
        }
    }

    public partial class Radio<TKey>
    {
        private WriteOnlyHook? _refresher;

        public RenderFragment Renderer() => Latch.Create(builder =>
            {
                int seq = -1;

                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class", "I4E-Bit I4E-Bit-Radio");

                foreach ((TKey key, Content content) in Options())
                {
                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-Radio-Option");
                    builder.AddAttribute(++seq, "data-selected", key.Equals(_selection));
                    builder.AddAttribute(++seq, "onclick",
                        EventCallback.Factory.Create(this, () => SetValue(key)));

                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-Radio-Option-Button");
                    builder.OpenComponent<BootstrapIcon>(++seq);
                    builder.AddAttribute(++seq, "ID",   key.Equals(_selection) ? "record-circle-fill" : "circle");
                    builder.AddAttribute(++seq, "Size", (ushort) 16);
                    builder.CloseComponent();
                    builder.CloseElement();

                    builder.OpenElement(++seq, "div");
                    builder.AddAttribute(++seq, "class", "I4E-Bit-Radio-Option-Content");
                    builder.AddContent(+seq, content.Renderer());
                    builder.CloseElement();

                    builder.CloseElement();
                }

                builder.CloseElement();
            },
            v => _refresher = v);
    }

    public partial class Radio<TKey>
    {
        public class Spec
        {
            public Callbacks.Callback<TKey>? Value { get; init; }
        }
    }
}