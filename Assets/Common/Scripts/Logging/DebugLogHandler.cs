using System;
using UnityEngine;

namespace KarenKrill.Logging
{
    public class DebugLogHandler : ILogHandler
    {
        public void LogException(Exception exception, UnityEngine.Object context)
        {
            Debug.LogException(exception, context);
        }
        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            Debug.LogFormat(logType, LogOption.None, context, format, args);
        }
    }
}