using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Element.Inputs
{
    internal static class InputBuilder
    {
        internal static void ApplyOuterAttributes<T>
        (
            InputBase<T>      inputBase,
            RenderTreeBuilder builder,
            ref int           seq,
            string[]?         additionalClasses
        )
        {
            builder.AddAttribute(++seq, "id", inputBase.ID);
            builder.AddAttribute(++seq, "class",
                ElementBuilder.ClassAttribute(inputBase.BaseClasses, inputBase.BaseSpec, additionalClasses));

            ++seq;
            if (inputBase.BaseSpec.IsVisible?.Invoke() == false)
                builder.AddAttribute(seq, "hidden", true);

            ++seq;
            if (inputBase.BaseSpec.Tooltip != null)
                builder.AddAttribute(seq, "data-i4e.tooltip", inputBase.BaseSpec.Tooltip.Invoke());

            if (inputBase.BaseSpec.Data != null)
                foreach ((string name, Callbacks.DataValue getter) in inputBase.BaseSpec.Data.Invoke())
                    builder.AddAttribute(++seq, "data-" + name, getter.Invoke());
        }

        internal static void ApplyInnerAttributes<T>
        (
            InputBase<T>      inputBase,
            RenderTreeBuilder builder,
            ref int           seq,
            string[]?         additionalStyles
        )
        {
            builder.AddAttribute(++seq, "id", $"{inputBase.ID}.Inner");
            builder.AddAttribute(++seq, "style",
                ElementBuilder.StyleAttribute(inputBase.BaseSpec, additionalStyles));
        }
    }
}