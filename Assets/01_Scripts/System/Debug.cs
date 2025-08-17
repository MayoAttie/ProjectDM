using UnityEngine;

namespace Project.Utility
{
    /// <summary>
    /// 공용 디버그 출력 래퍼
    /// (빌드 타입/로그 레벨/필터링 제어 가능)
    /// </summary>
    public static class DebugLog
    {
        public enum LogLevel
        {
            Verbose,
            Info,
            Warning,
            Error,
            None
        }

        // 현재 로그 레벨 (외부에서 조정 가능)
        public static LogLevel CurrentLogLevel = LogLevel.Verbose;

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(string message, Object context = null)
        {
            if (CurrentLogLevel <= LogLevel.Verbose)
                DebugLog.Log(message, context);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Info(string message, Object context = null)
        {
            if (CurrentLogLevel <= LogLevel.Info)
                DebugLog.Log($"<color=cyan>[INFO]</color> {message}", context);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Warning(string message, Object context = null)
        {
            if (CurrentLogLevel <= LogLevel.Warning)
                DebugLog.Warning($"<color=yellow>[WARN]</color> {message}", context);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Error(string message, Object context = null)
        {
            if (CurrentLogLevel <= LogLevel.Error)
                DebugLog.Error($"<color=red>[ERROR]</color> {message}", context);
        }
    }
}
