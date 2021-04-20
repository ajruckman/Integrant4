using System;
using System.Timers;

namespace Integrant4.Fundament
{
    public class Debouncer<T>
    {
        public delegate void Elapsed(T newValue);

        private readonly Timer  _debouncer;
        private readonly object _valueLock = new object();

        public Debouncer
        (
            Elapsed            onElapsed,
            T                  initialValue,
            int                milliseconds = 200,
            Action<Exception>? errorHandler = null
        )
        {
            Value      = initialValue;
            _debouncer = new Timer(milliseconds);
            _debouncer.Stop();
            _debouncer.AutoReset = false;
            _debouncer.Elapsed += (_, __) =>
            {
                lock (_valueLock)
                {
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

        public T Value { get; private set; }

        public void Reset(T newValue)
        {
            _debouncer.Stop();
            lock (_valueLock)
            {
                Value = newValue;
            }

            _debouncer.Start();
        }
    }
}