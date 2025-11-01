using UnityEngine;

namespace KarenKrill.Logging
{
    public static class LoggerExtensions
    {
        public static void LogWarning(this ILogger logger, object message)
        {
            logger.LogWarning(string.Empty, message);
        }
    }
}
