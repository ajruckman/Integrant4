using Integrant4.API;

namespace Integrant4.Element.Inputs.Wrappers
{
    public class InputReference<T>
    {
        public IWritableInput<T>? Input { get; private set; }

        public void Set(IWritableInput<T> input)
        {
            Input = input;
        }
    }
}