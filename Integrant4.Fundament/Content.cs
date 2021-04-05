using System.Collections.Generic;
using System.Linq;
using Integrant4.API;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Fundament
{
    public readonly struct Content : IRenderable
    {
        public readonly RenderFragment Fragment;

        //

        private Content(RenderFragment s) => Fragment = s;
        private Content(MarkupString   s) => Fragment = builder => builder.AddContent(0, s);
        private Content(string?        s) => Fragment = builder => builder.AddContent(0, s);

        //

        public static implicit operator Content(RenderFragment v) => new(v);
        public static implicit operator Content(MarkupString   v) => new(v);
        public static implicit operator Content(string?        v) => new(v);

        public static implicit operator Content(Content[]     v) => v.AsContent();
        public static implicit operator Content(List<Content> v) => v.AsContent();

        //

        public static Content operator +(Content a, Content b)
        {
            return new RenderFragment(builder =>
            {
                builder.AddContent(0, a.Fragment);
                builder.AddContent(1, b.Fragment);
            });
        }

        public RenderFragment Renderer() => Fragment;
    }

    public static class ContentExtensions
    {
        public static Content AsContent(this RenderFragment v) => v;
        public static Content AsContent(this MarkupString   v) => v;
        public static Content AsContent(this string         v) => v;

        public static Content AsContent(this IEnumerable<Content> v)
        {
            Content[] contents = v.ToArray();

            if (contents.Length == 0) return "";
            if (contents.Length == 1) return contents[1];

            Content result = contents[1];

            foreach (Content child in contents[1..])
            {
                result += child;
            }

            return result;
        }
    }
}