using System;
using UnityEngine;

public class InGameLogHandler : ILogHandler
{
    private readonly ILogHandler defaultLogHandler = Debug.unityLogger.logHandler;

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        string message = string.Format(format, args);
        LogToInGameDebug(logType, message);
        defaultLogHandler.LogFormat(logType, context, format, args);
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        LogToInGameDebug(LogType.Exception, exception.ToString());
        defaultLogHandler.LogException(exception, context);
    }

    private void LogToInGameDebug(LogType logType, string message)
    {
        if (InGameDebug.Instance != null)
        {
            switch (logType)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    InGameDebug.Instance.LogError(message);
                    break;
                case LogType.Warning:
                    InGameDebug.Instance.LogWarning(message);
                    break;
                case LogType.Log:
                default:
                    InGameDebug.Instance.Log(message);
                    break;
            }
        }
    }
}