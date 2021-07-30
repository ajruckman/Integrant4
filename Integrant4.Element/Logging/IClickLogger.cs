using System;
using System.Runtime.CompilerServices;

namespace Integrant4.Element.Logging
{
    public interface IClickLogger
    {
        public void Log
        (
            [CallerFilePath]   string filePath   = "",
            [CallerLineNumber] int    lineNumber = 0,
            [CallerMemberName] string memberName = ""
        )
        {
            Console.WriteLine(filePath);
        }
    }
}