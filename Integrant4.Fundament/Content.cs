using System;
using System.Collections.Generic;
using Integrant4.API;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Fundament
{
    public delegate IRenderable DynamicContentGetter();

    public delegate IEnumerable<IRenderable> DynamicContentsGetter();

    public class DynamicContent : IRenderable
    {
        public delegate IRenderable              DynamicContentGetter();
        public delegate IEnumerable<IRenderable> DynamicContentsGetter();

        private readonly DynamicContentGetter?     _getSingle;
        private readonly DynamicContentsGetter?    _getMultiple;
        private readonly IRenderable?              _rawSingle;
        private readonly IEnumerable<IRenderable>? _rawMultiple;

        private DynamicContent(DynamicContentGetter s)
        {
            _getSingle   = s;
            _getMultiple = null;
            _rawSingle   = null;
            _rawMultiple = null;
        }

        private DynamicContent(DynamicContentsGetter s)
        {
            _getSingle   = null;
            _getMultiple = s;
            _rawSingle   = null;
            _rawMultiple = null;
        }

        private DynamicContent(IRenderable s)
        {
            _getSingle   = null;
            _getMultiple = null;
            _rawSingle   = s;
            _rawMultiple = null;
        }

        private DynamicContent(IEnumerable<IRenderable> s)
        {
            _getSingle   = null;
            _getMultiple = null;
            _rawSingle   = null;
            _rawMultiple = s;
        }

        private DynamicContent(Func<IRenderable> s)
        {
            _getSingle   = s.Invoke;
            _getMultiple = null;
            _rawSingle   = null;
            _rawMultiple = null;
        }

        private DynamicContent(Func<IEnumerable<IRenderable>> s)
        {
            _getSingle   = null;
            _getMultiple = s.Invoke;
            _rawSingle   = null;
            _rawMultiple = null;
        }

        //

        public RenderFragment Renderer() => GetOne().Renderer();

        public IRenderable GetOne()
        {
            if (_getSingle != null)
                return _getSingle.Invoke();
            if (_rawSingle != null)
                return _rawSingle;

            if (_getMultiple != null)
            {
                DynamicContentsGetter g = _getMultiple;

                void Fragment(RenderTreeBuilder b)
                {
                    int seq = -1;
                    foreach (IRenderable v in g.Invoke())
                        b.AddContent(++seq, v.Renderer());
                }

                return (Content)Fragment;
            }

            if (_rawMultiple != null)
            {
                IEnumerable<IRenderable> r = _rawMultiple;

                void Fragment(RenderTreeBuilder b)
                {
                    int seq = -1;
                    foreach (IRenderable v in r)
                        b.AddContent(++seq, v);
                }

                return (Content)Fragment;
            }

            throw new Exception();
        }

        public IEnumerable<IRenderable> GetAll()
        {
            if (_getSingle != null)
                return new[] { _getSingle.Invoke() };
            if (_rawSingle != null)
                return new[] { _rawSingle };

            if (_getMultiple != null)
                return _getMultiple.Invoke();
            if (_rawMultiple != null)
                return _rawMultiple;

            throw new Exception();
        }

        //

        public static DynamicContent New(string                   s) => new(s.AsContent());
        public static DynamicContent New(MarkupString             s) => new(s.AsContent());
        public static DynamicContent New(RenderFragment           s) => new(s.AsContent());
        public static DynamicContent New(Content                  s) => new(s);
        public static DynamicContent New(IRenderable              s) => new(s);
        public static DynamicContent New(IEnumerable<IRenderable> s) => new(s);
        public static DynamicContent New(DynamicContentGetter     s) => new(s);
        public static DynamicContent New(DynamicContentsGetter    s) => new(s);

        public static implicit operator DynamicContent(string         s) => new(s.AsContent());
        public static implicit operator DynamicContent(MarkupString   s) => new(s.AsContent());
        public static implicit operator DynamicContent(RenderFragment s) => new(s.AsContent());
        public static implicit operator DynamicContent(Content        s) => new(s);

        // We can't define an implicit conversion from 'DynamicContentGetter' or 'DynamicContentsGetter' because they
        // are delegates, but we can define them for functions of type 'IRenderable'.
        public static implicit operator DynamicContent(Func<IRenderable>              s) => new(s);
        public static implicit operator DynamicContent(Func<IEnumerable<IRenderable>> s) => new(s);

        // We can't define an implicit conversion from 'IRenderable' or 'IEnumerable<IRenderable>' because they are
        // interface types, but we can define them for arrays and lists of type 'IRenderable'.
        public static implicit operator DynamicContent(IRenderable[]     s) => new(s);
        public static implicit operator DynamicContent(List<IRenderable> s) => new(s);
    }

    public readonly struct Content : IRenderable
    {
        public readonly RenderFragment Fragment;

        //

        private Content(string? s)
        {
            Fragment = builder => builder.AddContent(0, s);
        }

        private Content(MarkupString s)
        {
            Fragment = builder => builder.AddContent(0, s);
        }

        private Content(RenderFragment s)
        {
            Fragment = s;
        }

        // private Content(RenderFragment[] s)
        // {
        //     Fragment = builder =>
        //     {
        //         int seq = -1;
        //         foreach (RenderFragment v in s)
        //             builder.AddContent(++seq, v);
        //     };
        // }
        //
        // private Content(IEnumerable<IRenderable> s)
        // {
        //     Fragment = builder =>
        //     {
        //         int seq = -1;
        //         foreach (IRenderable v in s)
        //             builder.AddContent(++seq, v.Renderer());
        //     };
        // }

        //

        public static implicit operator Content(string?        v) => new(v);
        public static implicit operator Content(MarkupString   v) => new(v);
        public static implicit operator Content(RenderFragment v) => new(v);

        // public static implicit operator Content(RenderFragment[]  v) => new(v);
        // public static implicit operator Content(IRenderable[]     v) => new(v);
        // public static implicit operator Content(List<IRenderable> v) => new(v);

        // public static implicit operator Content(Content[]     v) => v.AsContent();
        // public static implicit operator Content(List<Content> v) => v.AsContent();

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
        public static Content AsContent(this string         v) => v;
        public static Content AsContent(this MarkupString   v) => v;
        public static Content AsContent(this RenderFragment v) => v;

        // public static Content AsContent(this IEnumerable<Content> v)
        // {
        //     Content[] contents = v.ToArray();
        //
        //     if (contents.Length == 0) return "";
        //     if (contents.Length == 1) return contents[1];
        //
        //     Content result = contents[1];
        //
        //     foreach (Content child in contents[1..])
        //     {
        //         result += child;
        //     }
        //
        //     return result;
        // }

        // public static DynamicContent AsDynamicContent(this IRenderable v) =>
        //     () => v;
        //
        // public static DynamicContent AsDynamicContent(this Content v) =>
        //     () => v;
        //
        // public static DynamicContent AsDynamicContent(this RenderFragment v) =>
        //     () => v.AsContent();
        //
        // public static DynamicContent AsDynamicContent(this MarkupString v) =>
        //     () => v.AsContent();
        //
        // public static DynamicContent AsDynamicContent(this string v) =>
        //     () => v.AsContent();
        //
        // public static DynamicContents AsDynamicContents(this IRenderable v) =>
        //     () => new[] { v };
        //
        // public static DynamicContents AsDynamicContents(this Content v) =>
        //     () => new[] { v as IRenderable, };
        //
        // public static DynamicContents AsDynamicContents(this RenderFragment v) =>
        //     () => new[] { v.AsContent() as IRenderable };
        //
        // public static DynamicContents AsDynamicContents(this MarkupString v) =>
        //     () => new[] { v.AsContent() as IRenderable };
        //
        // public static DynamicContents AsDynamicContents(this string v) =>
        //     () => new[] { v.AsContent() as IRenderable };
        //
        // public static DynamicContents AsDynamicContents(this DynamicContent dynamicContent) =>
        //     () => new[] { dynamicContent.Invoke() };
    }
}