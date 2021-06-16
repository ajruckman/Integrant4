using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Element.Constructs.Tagging
{
    public static class TagExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RenderFragment Render(this ITag tag) => builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "I4E-Construct-TagSelector-Tag");

            builder.OpenElement(2, "div");
            builder.AddContent(4, tag.Name + ":");
            builder.CloseElement();

            builder.OpenElement(4, "div");
            builder.AddContent(5, tag.Content());
            builder.CloseElement();

            builder.CloseElement();
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RenderFragment RenderSelectable(this ITag tag, TagSelector selector) => builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "I4E-Construct-TagSelector-AddableTag");
            builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(selector, () => selector.AddTag(tag)));

            builder.OpenElement(3, "div");
            builder.AddContent(4, tag.Name + ":");
            builder.CloseElement();

            builder.OpenElement(5, "div");
            builder.AddContent(6, tag.Content());
            builder.CloseElement();

            builder.CloseElement();
        };
    }

    public interface ITag
    {
        string Name { get; }

        string Content();
    }

    public interface ITag<out TValue> : ITag
    {
        TValue Value { get; }
    }

    public class StringTag : ITag<string>
    {
        public StringTag(string name, string value)
        {
            Name  = name;
            Value = value;
        }

        public string Name  { get; }
        public string Value { get; }

        public string Content() => Value;
    }

    public class IntTag : ITag<long>
    {
        public IntTag(string name, long value)
        {
            Name  = name;
            Value = value;
        }

        public string Name  { get; }
        public long   Value { get; }

        public string Content() => Value.ToString();
    }

    public class BoolTag : ITag<bool>
    {
        public BoolTag(string name, bool value)
        {
            Name  = name;
            Value = value;
        }

        public string Name  { get; }
        public bool   Value { get; }

        public string Content() => (Value ? "True" : "False");
    }

    public class AnyStringTag : ITag
    {
        public AnyStringTag(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public string Content() => "Any";
    }

    public class AnyIntTag : ITag
    {
        public AnyIntTag(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public string Content() => "Any";
    }

    public class AnyBoolTag : ITag
    {
        public AnyBoolTag(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public string Content() => "Any";
    }
}