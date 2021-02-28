using System.Timers;

namespace Integrant4.Structurant
{
    internal static class Utility
    {
        internal class Debouncer<T>
        {
            public delegate void OnElapsed(T newValue);

            private readonly Timer  _debouncer;
            private readonly object _valueLock = new object();

            public Debouncer(OnElapsed onElapsed, T initialValue, int milliseconds = 200)
            {
                Value      = initialValue;
                _debouncer = new Timer(milliseconds);
                _debouncer.Stop();
                _debouncer.AutoReset = false;
                _debouncer.Elapsed += (_, __) =>
                {
                    lock (_valueLock)
                    {
                        onElapsed.Invoke(Value);
                    }
                };
            }

            internal T Value { get; private set; }

            internal void Reset(T newValue)
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
}