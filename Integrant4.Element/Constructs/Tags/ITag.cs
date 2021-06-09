using Integrant4.Fundament;

namespace Integrant4.Element.Constructs.Tags
{
    public interface ITag
    {
        string Name { get; }

        Content Content();
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

        public Content Content() => Value.AsContent();
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

        public Content Content() => Value.ToString().AsContent();
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

        public Content Content() => (Value ? "True" : "False").AsContent();
    }

    public class VoidTag : ITag
    {
        public VoidTag(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public Content Content() => "Any".AsContent();
    }
}