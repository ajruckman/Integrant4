using System;
using System.Timers;

namespace Integrant4.Fundament
{
    public class Debouncer<T>
    {
        public delegate void OnReset();
        public delegate void OnElapsed(T newValue);

        private readonly OnReset? _onReset;
        private readonly Timer    _debouncer;
        private readonly object   _valueLock = new();

        private bool _isSet;

        public Debouncer
        (
            OnReset?           onReset,
            OnElapsed          onElapsed,
            int                milliseconds = 200,
            Action<Exception>? errorHandler = null
        )
        {
            _onReset   = onReset;
            _debouncer = new Timer(milliseconds) { Enabled = false, AutoReset = false };
            _debouncer.Elapsed += (_, _) =>
            {
                lock (_valueLock)
                {
                    if (!_isSet) return;

                    try
                    {
                        onElapsed.Invoke(Value);
                    }
                    catch (Exception e)
                    {
                        if (errorHandler != null)
                            errorHandler.Invoke(e);
                        else
                        {
                            Console.WriteLine("Debouncer with no error handler elapsed with exception.");
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                }
            };
        }

        public T Value { get; private set; } = default!;

        public void Reset(T newValue)
        {
            _onReset?.Invoke();

            _debouncer.Stop();
            lock (_valueLock)
            {
                Value  = newValue;
                _isSet = true;
            }

            _debouncer.Start();
        }
    }
}