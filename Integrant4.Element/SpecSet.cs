using System;
using System.Collections.Generic;
using System.Linq;
using Integrant4.Fundament;

namespace Integrant4.Element
{
    internal partial class SpecSet
    {
        internal ClassSet? BaseClasses { get; init; }
        internal bool      Scaled      { get; init; }

        internal Callbacks.IsVisible?  IsVisible      { get; init; }
        internal Callbacks.IsDisabled? IsDisabled     { get; init; }
        internal Callbacks.IsRequired? IsRequired     { get; init; }
        internal Callbacks.IsChecked?  IsChecked      { get; init; }
        internal Callbacks.Classes?    Classes        { get; init; }
        internal Callbacks.HREF?       HREF           { get; init; }
        internal Callbacks.Color?      HighlightColor { get; init; }
        internal Callbacks.Data?       Data           { get; init; }
        internal Callbacks.Tooltip?    Tooltip        { get; init; }

        internal Callbacks.Size?        Margin          { get; init; }
        internal Callbacks.Size?        Padding         { get; init; }
        internal Callbacks.Color?       BackgroundColor { get; init; }
        internal Callbacks.Color?       ForegroundColor { get; init; }
        internal Callbacks.Unit?        Height          { get; init; }
        internal Callbacks.Unit?        HeightMax       { get; init; }
        internal Callbacks.Unit?        Width           { get; init; }
        internal Callbacks.Unit?        WidthMax        { get; init; }
        internal Callbacks.REM?         FontSize        { get; init; }
        internal Callbacks.FontWeight?  FontWeight      { get; init; }
        internal Callbacks.TextAlign?   TextAlign       { get; init; }
        internal Callbacks.Display?     Display         { get; init; }
        internal Callbacks.FlexAlign?   FlexAlign       { get; init; }
        internal Callbacks.FlexJustify? FlexJustify     { get; init; }
        internal Callbacks.Scale?       Scale           { get; init; }
    }

    internal partial class SpecSet
    {
        internal string? ClassAttribute(IEnumerable<string>? additional)
        {
            ClassSet c = BaseClasses?.Clone() ?? new ClassSet();

            if (Classes != null)
                c.AddRange(Classes.Invoke());

            if (IsDisabled?.Invoke() == true)
                c.Add("I4E-Bit--Disabled");
            if (IsRequired?.Invoke() == true)
                c.Add("I4E-Bit--Required");
            if (!string.IsNullOrEmpty(HighlightColor?.Invoke()))
                c.Add("I4E-Bit--Highlighted");

            if (additional != null)
                c.AddRange(additional);

            return c.ToString();
        }

        internal string? StyleAttribute(IEnumerable<string>? additional)
        {
            List<string> result = new();

            if (Margin != null)
            {
                Size v = Margin.Invoke();
                result.Add($"margin: {v.Serialize()};");
            }

            if (Padding != null)
            {
                Size v = Padding.Invoke();
                result.Add($"padding: {v.Serialize()};");
            }

            if (BackgroundColor != null)
            {
                result.Add($"background-color: {BackgroundColor.Invoke()};");
            }

            if (ForegroundColor != null)
            {
                result.Add($"color: {ForegroundColor.Invoke()};");
            }

            if (Height != null)
            {
                result.Add($"height: {Height.Invoke().Serialize()};");
            }

            if (HeightMax != null)
            {
                result.Add($"max-height: {HeightMax.Invoke().Serialize()};");
            }

            if (Width != null)
            {
                result.Add($"width: {Width.Invoke().Serialize()};");
            }

            if (WidthMax != null)
            {
                result.Add($"max-width: {WidthMax.Invoke().Serialize()};");
            }

            if (FontWeight != null)
            {
                result.Add($"font-weight: {(int) FontWeight.Invoke()};");
            }

            if (TextAlign != null)
            {
                string align = TextAlign.Invoke() switch
                {
                    Element.TextAlign.Left   => "left",
                    Element.TextAlign.Center => "center",
                    Element.TextAlign.Right  => "right",
                    _                        => throw new ArgumentOutOfRangeException(),
                };

                result.Add($"text-align: {align};");
            }

            if (Display != null)
            {
                string display = Display.Invoke() switch
                {
                    Element.Display.Undefined   => "unset",
                    Element.Display.Inline      => "inline",
                    Element.Display.InlineBlock => "inline-block",
                    Element.Display.Block       => "block",
                    _                           => throw new ArgumentOutOfRangeException(),
                };
                result.Add($"display: {display};");
            }

            if (FlexAlign != null)
                result.Add($"align-items: {FlexAlign.Invoke().Serialize()}");

            if (FlexJustify != null)
                result.Add($"justify-content: {FlexJustify.Invoke().Serialize()}");

            //

            if (!Scaled)
            {
                if (FontSize != null)
                {
                    result.Add($"font-size: {FontSize.Invoke()}rem;");
                }
            }
            else
            {
                if (Scale != null)
                {
                    result.Add($"font-size: {Scale.Invoke()}rem;");
                }
            }

            //

            if (additional != null)
                result.AddRange(additional);

            return result.Any() ? string.Join(' ', result) : null;
        }
    }
}