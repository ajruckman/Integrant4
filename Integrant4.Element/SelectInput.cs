using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Integrant4.API;
using Integrant4.Element.Inputs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Integrant4.Element
{
    public interface IOption<out TValue>
    {
        TValue? Value            { get; }
        Content OptionContent    { get; }
        Content SelectionContent { get; }
        bool    Selected         { get; }
        bool    Disabled         { get; }
    }

    public class Option<TValue> : IOption<TValue>
    {
        public Option(TValue? value, Content optionText, Content selectionContent, bool selected, bool disabled)
        {
            Value            = value;
            OptionContent    = optionText;
            SelectionContent = selectionContent;
            Selected         = selected;
            Disabled         = disabled;
        }

        public TValue? Value            { get; }
        public Content OptionContent    { get; }
        public Content SelectionContent { get; }
        public bool    Selected         { get; }
        public bool    Disabled         { get; }
    }

    public class SelectInput<TValue> : InputBase<TValue?>, ICachingInput
    {
        public delegate List<IOption<TValue>> OptionGetter();

        private readonly OptionGetter _optionGetter;
        private readonly object       _optionCacheLock = new();

        private List<IOption<TValue>>? _optionCache;

        public SelectInput(IJSRuntime jsRuntime, TValue? value, OptionGetter optionGetter) : base(jsRuntime, value)
        {
            _optionGetter = optionGetter;
        }

        public void InvalidateCache()
        {
            lock (_optionCacheLock)
            {
                _optionCache = null;
            }
        }

        private List<IOption<TValue>> Options()
        {
            lock (_optionCacheLock)
            {
                return _optionCache ??= _optionGetter.Invoke();
            }
        }

        public override RenderFragment Render()
        {
            void Fragment(RenderTreeBuilder builder)
            {
                int seq = -1;

                builder.OpenElement(++seq, "select");

                lock (_optionCacheLock)
                {
                    int selected = Options().FindIndex(v => v.Selected);

                    int seqI = -1;
                    for (var i = 0; i < Options().Count; i++)
                    {
                        IOption<TValue> option = Options()[i];

                        builder.OpenElement(++seqI, "option");
                        builder.AddAttribute(++seqI, "value", i);

                        ++seqI;
                        if (selected == i) builder.AddAttribute(seqI, "selected", true);

                        ++seqI;
                        if (option.Disabled) builder.AddAttribute(seqI, "disabled", true);

                        builder.AddContent(++seqI, option.OptionContent);
                        builder.CloseElement();
                    }
                }
                
                builder.CloseElement();
            }

            return Fragment;
        }

        protected override string Serialize(TValue? v)
        {
            throw new System.NotImplementedException();
        }

        protected override TValue? Deserialize(string? v)
        {
            throw new System.NotImplementedException();
        }

        protected override TValue? Nullify(TValue? v)
        {
            throw new System.NotImplementedException();
        }
    }
}