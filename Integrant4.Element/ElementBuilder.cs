using System;
using System.Collections.Generic;
using System.Linq;
using Integrant4.Fundament;

namespace Integrant4.Element
{
    internal static class ElementBuilder
    {
        internal static string ClassAttribute(ClassSet baseSet, BaseSpec spec, IEnumerable<string>? additional)
        {
            ClassSet c = baseSet.Clone();

            if (additional != null)
                c.AddRange(additional);
            if (spec.Classes != null)
                c.AddRange(spec.Classes.Invoke());

            if (spec.IsDisabled?.Invoke() == true)
                c.Add("I4E-Bit--Disabled");
            if (spec.IsRequired?.Invoke() == true)
                c.Add("I4E-Bit--Required");
            if (!string.IsNullOrEmpty(spec.HighlightColor?.Invoke()))
                c.Add("I4E-Bit--Highlighted");

            return c.ToString();
        }

        internal static string? StyleAttribute(BaseSpec spec, IEnumerable<string>? additional)
        {
            List<string> result = new();

            if (spec.Margin != null)
            {
                Size v = spec.Margin.Invoke();
                result.Add($"margin: {v.Serialize()};");
            }

            if (spec.Padding != null)
            {
                Size v = spec.Padding.Invoke();
                result.Add($"padding: {v.Serialize()};");
            }

            if (spec.BackgroundColor != null)
            {
                result.Add($"background-color: {spec.BackgroundColor.Invoke()};");
            }

            if (spec.ForegroundColor != null)
            {
                result.Add($"color: {spec.ForegroundColor.Invoke()};");
            }

            if (spec.Height != null)
            {
                result.Add($"height: {spec.Height.Invoke().Serialize()};");
            }

            if (spec.HeightMax != null)
            {
                result.Add($"max-height: {spec.HeightMax.Invoke().Serialize()};");
            }

            if (spec.Width != null)
            {
                result.Add($"width: {spec.Width.Invoke().Serialize()};");
            }

            if (spec.WidthMax != null)
            {
                result.Add($"max-width: {spec.WidthMax.Invoke().Serialize()};");
            }

            if (spec.FontWeight != null)
            {
                result.Add($"font-weight: {(int) spec.FontWeight.Invoke()};");
            }

            if (spec.TextAlign != null)
            {
                string align = spec.TextAlign.Invoke() switch
                {
                    TextAlign.Left   => "left",
                    TextAlign.Center => "center",
                    TextAlign.Right  => "right",
                    _                => throw new ArgumentOutOfRangeException(),
                };

                result.Add($"text-align: {align};");
            }

            if (spec.Display != null)
            {
                string display = spec.Display.Invoke() switch
                {
                    Display.Undefined   => "unset",
                    Display.Inline      => "inline",
                    Display.InlineBlock => "inline-block",
                    Display.Block       => "block",
                    _                   => throw new ArgumentOutOfRangeException(),
                };
                result.Add($"display: {display};");
            }

            //

            if (!spec.Scaled)
            {
                if (spec.FontSize != null)
                {
                    result.Add($"font-size: {spec.FontSize.Invoke()}rem;");
                }
            }
            else
            {
                if (spec.Scale != null)
                {
                    result.Add($"font-size: {spec.Scale.Invoke()}rem;");
                }
            }

            //

            if (additional != null)
                result.AddRange(additional);

            return result.Any() ? string.Join(' ', result) : null;
        }

        internal static string? ContentStyleAttribute(BaseSpec spec)
        {
            List<string> result = new(3);

            if (spec.FontSize != null)
                result.Add($"font-size: {spec.FontSize.Invoke()}rem;");

            if (spec.FlexAlign != null)
                result.Add($"align-items: {spec.FlexAlign.Invoke().Serialize()}");

            if (spec.FlexJustify != null)
                result.Add($"justify-content: {spec.FlexJustify.Invoke().Serialize()}");

            return result.Any() ? string.Join(' ', result) : null;
        }
    }
}