using Microsoft.AspNetCore.Components;

namespace Integrant4.API
{
    public readonly struct Content
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

        //

        public static Content operator +(Content a, Content b)
        {
            return new RenderFragment(builder =>
            {
                builder.AddContent(0, a.Fragment);
                builder.AddContent(1, b.Fragment);
            });
        }
    }

    public static class ContentExtensions
    {
        public static Content AsContent(this RenderFragment v) => v;
        public static Content AsContent(this MarkupString   v) => v;
        public static Content AsContent(this string         v) => v;
    }
}