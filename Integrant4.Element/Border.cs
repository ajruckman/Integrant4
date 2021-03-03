namespace Integrant4.Element
{
    public readonly struct Border
    {
        public readonly string? Color;
        public readonly string? Pixels;

        public Border(string? color, string? pixels)
        {
            Color  = color;
            Pixels = pixels;
        }
    }
}