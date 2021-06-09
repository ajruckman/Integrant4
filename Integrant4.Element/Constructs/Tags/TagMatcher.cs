using System;
using System.Collections.Generic;
using System.Linq;

namespace Integrant4.Element.Constructs.Tags
{
    public static class TagMatcher
    {
        public static bool Matches
        (
            IReadOnlyList<ITag> tags,
            IReadOnlyList<ITag> filters
        )
        {
            Dictionary<Type, List<ITag>> tagsByType = new()
            {
                { typeof(StringTag), new List<ITag>() },
                { typeof(IntTag), new List<ITag>() },
                { typeof(BoolTag), new List<ITag>() },
            };

            foreach (ITag tag in tags)
            {
                tagsByType[tag.GetType()].Add(tag);
            }

            foreach (ITag filter in filters)
            {
                bool       matched = true;
                List<ITag> toCheck = tagsByType[filter.GetType()];

                switch (filter)
                {
                    case StringTag stringTag:
                        foreach (StringTag tag in toCheck.Cast<StringTag>())
                        {
                            if (tag.Name == filter.Name && tag.Value != stringTag.Value)
                            {
                                break;
                            }

                            matched = false;
                        }

                        break;
                    case IntTag intTag:
                        foreach (IntTag tag in toCheck.Cast<IntTag>())
                        {
                            if (tag.Name == filter.Name && tag.Value != intTag.Value)
                            {
                                break;
                            }

                            matched = false;
                        }

                        break;

                    case BoolTag boolTag:
                        foreach (BoolTag tag in toCheck.Cast<BoolTag>())
                        {
                            if (tag.Name == filter.Name && tag.Value != boolTag.Value)
                            {
                                break;
                            }

                            matched = false;
                        }

                        break;
                    case VoidTag voidTag:
                        foreach (ITag tag in toCheck)
                        {
                            if (tag.Name == filter.Name)
                            {
                                break;
                            }

                            matched = false;
                        }

                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException(nameof(filter), "Unmatched tag type.");
                }

                if (!matched) return false;
            }

            return true;
        }
    }
}