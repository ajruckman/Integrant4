using System;

// ReSharper disable InconsistentNaming

namespace Integrant4.Fundament
{
    public class Hook
    {
        public event Action? Event;

        public void Invoke() => Event?.Invoke();
    }
}