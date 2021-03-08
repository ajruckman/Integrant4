using Integrant4.API;

namespace Integrant4.Element.Inputs.Wrappers
{
    public class InputReference<T>
    {
        public IInput<T>? Input { get; private set; }

        public void Set(IInput<T> input)
        {
            Input = input;
        }
    }
}