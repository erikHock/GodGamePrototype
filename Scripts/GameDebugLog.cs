#define DEBUGGING
using UnityEngine;

public static class GameDebugLog
{
    [System.Diagnostics.Conditional("DEBUGGING")]
    public static void LogMessage(string message)
    {
        Debug.Log(message);
    }

    [System.Diagnostics.Conditional("DEBUGGING")]
    public static void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }

    [System.Diagnostics.Conditional("DEBUGGING")]
    public static void LogError(string message)
    {
        Debug.LogWarning(message);
    }
}
