using System;
using System.Threading.Tasks;
using Integrant4.API;
using Microsoft.AspNetCore.Components;

namespace Integrant4.Fundament
{
    public class InputAdapter<TIn, TOut> : IInput<TOut>
    {
        public delegate TOut AdaptTo(TIn? from);

        private readonly IInput<TIn> _input;
        private readonly AdaptTo     _to;

        public InputAdapter(IInput<TIn> input, AdaptTo to)
        {
            _input = input;
            _to    = to;

            _input.OnChange += v => OnChange?.Invoke(_to.Invoke(v));
        }

        public RenderFragment Renderer() => _input.Renderer();

        public async Task<TOut?> GetValue()
        {
            TIn? from = await _input.GetValue();
            return _to.Invoke(from);
        }

        public event Action<TOut?>? OnChange;
    }

    public class RefreshableInputAdapter<TIn, TOut> : IRefreshableInput<TOut>
    {
        public delegate TOut AdaptTo(TIn? from);

        private readonly IRefreshableInput<TIn> _input;
        private readonly AdaptTo                _to;

        public RefreshableInputAdapter(IRefreshableInput<TIn> input, AdaptTo to)
        {
            _input = input;
            _to    = to;

            _input.OnChange += v => OnChange?.Invoke(_to.Invoke(v));
        }

        public void Refresh() => _input.Refresh();

        public RenderFragment Renderer() => _input.Renderer();

        public async Task<TOut?> GetValue()
        {
            TIn? from = await _input.GetValue();
            return _to.Invoke(from);
        }

        public event Action<TOut?>? OnChange;
    }

    public class WritableInputAdapter<TIn, TOut> : IWritableInput<TOut>
    {
        public delegate TOut AdaptTo(TIn? from);

        public delegate TIn AdaptFrom(TOut? to);

        private readonly IWritableInput<TIn> _input;
        private readonly AdaptTo             _to;
        private readonly AdaptFrom           _from;

        public WritableInputAdapter(IWritableInput<TIn> input, AdaptTo to, AdaptFrom from)
        {
            _input = input;
            _to    = to;
            _from  = from;

            _input.OnChange += v => OnChange?.Invoke(_to.Invoke(v));
        }

        public Task SetValue(TOut? value) => _input.SetValue(_from.Invoke(value));

        public RenderFragment Renderer() => _input.Renderer();

        public async Task<TOut?> GetValue()
        {
            TIn? from = await _input.GetValue();
            return _to.Invoke(from);
        }

        public event Action<TOut?>? OnChange;
    }

    public class WritableRefreshableInputAdapter<TIn, TOut> : IWritableRefreshableInput<TOut>
    {
        public delegate TOut AdaptTo(TIn? from);

        public delegate TIn AdaptFrom(TOut? to);

        private readonly IWritableRefreshableInput<TIn> _input;
        private readonly AdaptTo                        _to;
        private readonly AdaptFrom                      _from;

        public WritableRefreshableInputAdapter(IWritableRefreshableInput<TIn> input, AdaptTo to, AdaptFrom from)
        {
            _input = input;
            _to    = to;
            _from  = from;

            _input.OnChange += v => OnChange?.Invoke(_to.Invoke(v));
        }

        public void Refresh() => _input.Refresh();

        public Task SetValue(TOut? value) => _input.SetValue(_from.Invoke(value));

        public RenderFragment Renderer() => _input.Renderer();

        public async Task<TOut?> GetValue()
        {
            TIn? from = await _input.GetValue();
            return _to.Invoke(from);
        }

        public event Action<TOut?>? OnChange;
    }
}