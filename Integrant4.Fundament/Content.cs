using System;
using System.Collections.Generic;
using Integrant4.API;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Integrant4.Fundament
{
    public class ContentRef : IRenderable
    {
        private readonly IRenderable?                    _rawSingle;
        private readonly IEnumerable<IRenderable>?       _rawMultiple;
        private readonly Func<IRenderable>?              _getSingle;
        private readonly Func<IEnumerable<IRenderable>>? _getMultiple;

        //

        private ContentRef(IRenderable              s) => _rawSingle = s;
        private ContentRef(IEnumerable<IRenderable> s) => _rawMultiple = s;

        private ContentRef(Func<IRenderable>              s) => _getSingle = s.Invoke;
        private ContentRef(Func<IEnumerable<IRenderable>> s) => _getMultiple = s.Invoke;

        //

        public RenderFragment Renderer() => GetOne().Renderer();

        public IRenderable GetOne()
        {
            if (_rawSingle != null)
                return _rawSingle;
            if (_getSingle != null)
                return _getSingle.Invoke();

            if (_rawMultiple != null)
            {
                void Fragment(RenderTreeBuilder b)
                {
                    int seq = -1;
                    foreach (IRenderable v in _rawMultiple)
                        b.AddContent(++seq, v);
                }

                return (Content) Fragment;
            }

            if (_getMultiple != null)
            {
                void Fragment(RenderTreeBuilder b)
                {
                    int seq = -1;
                    foreach (IRenderable v in _getMultiple.Invoke())
                        b.AddContent(++seq, v.Renderer());
                }

                return (Content) Fragment;
            }

            throw new Exception();
        }

        public IEnumerable<IRenderable> GetAll()
        {
            if (_rawSingle != null)
                return new[] { _rawSingle };
            if (_getSingle != null)
                return new[] { _getSingle.Invoke() };

            if (_rawMultiple != null)
                return _rawMultiple;
            if (_getMultiple != null)
                return _getMultiple.Invoke();

            throw new Exception();
        }

        //

        public static ContentRef Static(string                   s) => new(s.AsContent());
        public static ContentRef Static(MarkupString             s) => new(s.AsContent());
        public static ContentRef Static(RenderFragment           s) => new(s.AsContent());
        public static ContentRef Static(Content                  s) => new(s);
        public static ContentRef Static(IRenderable              s) => new(s);
        public static ContentRef Static(IEnumerable<IRenderable> s) => new(s);

        public static ContentRef Dynamic(Func<string>                   s) => new(() => s.Invoke().AsContent());
        public static ContentRef Dynamic(Func<MarkupString>             s) => new(() => s.Invoke().AsContent());
        public static ContentRef Dynamic(Func<RenderFragment>           s) => new(() => s.Invoke().AsContent());
        public static ContentRef Dynamic(Func<Content>                  s) => new(() => s.Invoke());
        public static ContentRef Dynamic(Func<IRenderable>              s) => new(s);
        public static ContentRef Dynamic(Func<IEnumerable<IRenderable>> s) => new(s);

        // public static implicit operator ContentRef(string         s) => new(s.AsContent());
        // public static implicit operator ContentRef(MarkupString   s) => new(s.AsContent());
        // public static implicit operator ContentRef(RenderFragment s) => new(s.AsContent());
        // public static implicit operator ContentRef(Content        s) => new(s);
        //
        // // public static implicit operator ContentRef(string                         s) => new(s);
        // // public static implicit operator ContentRef(MarkupString                   s) => new(s);
        // // public static implicit operator ContentRef(RenderFragment                 s) => new(s);
        // // public static implicit operator ContentRef(Content                        s) => new(s);
        // // public static implicit operator ContentRef(Func<IRenderable>              s) => new(s);
        // // public static implicit operator ContentRef(Func<IEnumerable<IRenderable>> s) => new(s);
        //
        // // We can't define an implicit conversion from 'IRenderable' or 'IEnumerable<IRenderable>' because they are
        // // interface types, but we can define them for arrays and lists of type 'IRenderable'.
        // public static implicit operator ContentRef(IRenderable[]     s) => new(s);
        // public static implicit operator ContentRef(List<IRenderable> s) => new(s);
    }

    public readonly struct Content : IRenderable
    {
        public readonly RenderFragment Fragment;

        private Content(string?        s) => Fragment = builder => builder.AddContent(0, s);
        private Content(MarkupString   s) => Fragment = builder => builder.AddContent(0, s);
        private Content(RenderFragment s) => Fragment = s;

        public static implicit operator Content(string?        v) => new(v);
        public static implicit operator Content(MarkupString   v) => new(v);
        public static implicit operator Content(RenderFragment v) => new(v);

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

        public static ContentRef AsStatic(this string                   v) => ContentRef.Static(v);
        public static ContentRef AsStatic(this MarkupString             v) => ContentRef.Static(v);
        public static ContentRef AsStatic(this RenderFragment           v) => ContentRef.Static(v);
        public static ContentRef AsStatic(this Content                  v) => ContentRef.Static(v);
        public static ContentRef AsStatic(this IRenderable              v) => ContentRef.Static(v);
        public static ContentRef AsStatic(this IEnumerable<IRenderable> v) => ContentRef.Static(v);

        public static ContentRef AsStatic(this Func<string>                   v) => ContentRef.Dynamic(v);
        public static ContentRef AsStatic(this Func<MarkupString>             v) => ContentRef.Dynamic(v);
        public static ContentRef AsStatic(this Func<RenderFragment>           v) => ContentRef.Dynamic(v);
        public static ContentRef AsStatic(this Func<Content>                  v) => ContentRef.Dynamic(v);
        public static ContentRef AsStatic(this Func<IRenderable>              v) => ContentRef.Dynamic(v);
        public static ContentRef AsStatic(this Func<IEnumerable<IRenderable>> v) => ContentRef.Dynamic(v);
    }
}