namespace Integrant4.Fundament
{
    public static class Utilities
    {
        public static string? CoalesceAndTrim(this string? v) =>
            string.IsNullOrWhiteSpace(v) ? null : v.Trim();
    }
}