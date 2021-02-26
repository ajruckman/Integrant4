using Integrant4.API;

namespace Integrant4.Element
{
    public static class Callbacks
    {
        public delegate T Callback<out T>();

        //

        public delegate API.Content Content();

        public delegate int PxWidth();

        public delegate int PxHeight();
    }

    public static class Wrappers
    {
        public static Content Call(this Callbacks.Content  v) => v.Invoke();
        public static int     Call(this Callbacks.PxWidth  v) => v.Invoke();
        public static int     Call(this Callbacks.PxHeight v) => v.Invoke();
    }
}