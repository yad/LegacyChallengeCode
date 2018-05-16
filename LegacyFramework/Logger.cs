using System.IO;

namespace LegacyFramework
{
    public class Logger : ILogger
    {
        public void Error(string message)
        {
            throw new IOException("Logger doesn't work");
        }
    }
}
