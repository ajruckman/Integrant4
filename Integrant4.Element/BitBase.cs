using System;
using Integrant4.Fundament;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element
{
    public abstract class BitBase : IBit
    {
        protected static readonly Exception ReconstructedException = new(
            "This Bit was reconstructed in the render tree. "                                        +
            "Construct and assign and instance of this Bit once outside of the render tree builder " +
            "and call its '.Render()' method to render it into the tree."
        );

        internal readonly BitSpec Spec;

        private readonly ClassSet _constantClasses;

        private string? _cachedStyle;

        internal BitBase(BitSpec spec, ClassSet constantClasses)
        {
            Spec             = spec;
            _constantClasses = constantClasses;
        }

        public abstract RenderFragment Renderer();

        protected internal string Classes(string[]? additional = null)
        {
            ClassSet c = _constantClasses.Clone();

            if (additional != null)
                c.AddRange(additional);

            if (Spec.IsDisabled?.Invoke() == true)
                c.Add("Integrant.Element.Bit:Disabled");

            if (Spec.Classes != null)
                c.AddRange(Spec.Classes.Invoke());

            return c.ToString();
        }

        protected internal string? Styles(bool initial, string[]? additional = null)
        {
            if (Spec.IsStatic && !initial)
                return _cachedStyle;

            string? r = BitBuilder.StyleAttribute(Spec, additional);

            if (Spec.IsStatic)
                _cachedStyle = r;

            return r;
        }
    }
}