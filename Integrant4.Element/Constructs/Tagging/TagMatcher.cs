using System;
using System.Collections.Generic;
using System.Linq;

namespace Integrant4.Element.Constructs.Tagging
{
    public static class TagMatcher
    {
        public static bool Matches
        (
            IReadOnlyList<ITag> tags,
            IReadOnlyList<ITag> filters,
            bool                caseInsensitive = false
        )
        {
            Dictionary<Type, List<ITag>> tagsByType = new()
            {
                {typeof(StringTag), new List<ITag>()},
                {typeof(IntTag), new List<ITag>()},
                {typeof(BoolTag), new List<ITag>()},
            };
            tagsByType.Add(typeof(AnyStringTag), tagsByType[typeof(StringTag)]);
            tagsByType.Add(typeof(AnyIntTag),    tagsByType[typeof(IntTag)]);
            tagsByType.Add(typeof(AnyBoolTag),   tagsByType[typeof(BoolTag)]);

            foreach (ITag tag in tags)
            {
                tagsByType[tag.GetType()].Add(tag);
            }

            StringComparison comp = caseInsensitive
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            foreach (ITag filter in filters)
            {
                bool       matched = true;
                List<ITag> toCheck = tagsByType[filter.GetType()];

                switch (filter)
                {
                    case StringTag stringTag:
                        foreach (StringTag tag in toCheck.Cast<StringTag>())
                        {
                            if (tag.Name.Equals(filter.Name, comp) && tag.Value.Equals(stringTag.Value, comp))
                                goto Next;
                        }

                        matched = false;
                        break;

                    case IntTag intTag:
                        foreach (IntTag tag in toCheck.Cast<IntTag>())
                        {
                            if (tag.Name.Equals(filter.Name, comp) && tag.Value == intTag.Value)
                                goto Next;
                        }

                        matched = false;
                        break;

                    case BoolTag boolTag:
                        foreach (BoolTag tag in toCheck.Cast<BoolTag>())
                        {
                            if (tag.Name == filter.Name && tag.Value == boolTag.Value)
                                goto Next;
                        }

                        matched = false;
                        break;

                    case AnyStringTag:
                    case AnyIntTag:
                    case AnyBoolTag:
                        foreach (ITag tag in toCheck)
                        {
                            if (tag.Name.Equals(filter.Name, comp))
                            {
                                goto Next;
                            }
                        }

                        matched = false;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(filter), "Unmatched tag type.");
                }

                Next:
                if (!matched) return false;
            }

            return true;
        }
    }
}