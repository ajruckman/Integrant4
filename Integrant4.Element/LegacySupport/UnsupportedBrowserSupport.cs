using Microsoft.AspNetCore.Http;

namespace Integrant4.Element.LegacySupport
{
    public class UnsupportedBrowserSupport
    {
        public static bool IsIE(HttpRequest request)
        {
            string userAgent = request.Headers["User-Agent"];

            return userAgent.Contains("MSIE") || userAgent.Contains("Trident");
        }

        public static readonly string StylesheetPath = "/_content/Integrant4.Element/css/UnsupportedBrowserNotice.css";
    }
}