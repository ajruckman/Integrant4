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
        private readonly object   _valueLock = new object();

        public Debouncer
        (
            OnReset?  onReset,
            OnElapsed onElapsed,
            // T                  initialValue,
            int                milliseconds = 200,
            Action<Exception>? errorHandler = null
        )
        {
            _onReset = onReset;
            // Value      = initialValue;
            _debouncer = new Timer(milliseconds) { Enabled = false, AutoReset = false };
            _debouncer.Elapsed += (_, _) =>
            {
                lock (_valueLock)
                {
                    if (Value == null) return;

                    try
                    {
                        onElapsed.Invoke(Value);
                    }
                    catch (Exception e)
                    {
                        if (errorHandler != null)
                            errorHandler.Invoke(e);
                        else
                            throw;
                    }
                }
            };
        }

        public T? Value { get; private set; }

        public void Reset(T newValue)
        {
            _onReset?.Invoke();

            _debouncer.Stop();
            lock (_valueLock)
            {
                Value = newValue;
            }

            _debouncer.Start();
        }
    }
}