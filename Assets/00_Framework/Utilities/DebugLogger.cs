using UnityEngine;

namespace Framework.Utilities
{
    /// <summary>
    /// 로그 메시지를 관리하고 Unity 콘솔에 출력하는 유틸리티 클래스.
    /// </summary>
    public static class DebugLogger
    {
        /// <summary>
        /// 현재 디버그 로그 활성화 상태.
        /// </summary>
        public static bool EnableDebugLogs { get; set; } = true;

        /// <summary>
        /// 로그 레벨 열거형.
        /// </summary>
        private enum LogLevel
        {
            Info,
            Warning,
            Error,
            AssertFailed
        }

        /// <summary>
        /// 일반 정보 메시지를 Unity 콘솔에 출력.
        /// </summary>
        /// <param name="message">로그 메시지</param>
        public static void Log(string message)
        {
            LogMessage(LogLevel.Info, message);
        }

        /// <summary>
        /// 경고 메시지를 Unity 콘솔에 출력.
        /// </summary>
        /// <param name="message">로그 메시지</param>
        public static void LogWarning(string message)
        {
            LogMessage(LogLevel.Warning, message);
        }

        /// <summary>
        /// 오류 메시지를 Unity 콘솔에 출력.
        /// </summary>
        /// <param name="message">로그 메시지</param>
        public static void LogError(string message)
        {
            LogMessage(LogLevel.Error, message);
        }

        /// <summary>
        /// 조건부 검사를 수행하고, 실패할 경우 오류 메시지를 출력.
        /// </summary>
        /// <param name="condition">검사할 조건</param>
        /// <param name="message">조건 실패 시 출력할 메시지</param>
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                LogMessage(LogLevel.AssertFailed, message);
            }
        }

        /// <summary>
        /// 로그 메시지를 Unity 콘솔에 출력.
        /// </summary>
        /// <param name="level">로그 레벨</param>
        /// <param name="message">출력할 메시지</param>
        private static void LogMessage(LogLevel level, string message)
        {
            if (!EnableDebugLogs) return;

            string prefix = GetLogPrefix(level);
            switch (level)
            {
                case LogLevel.Info:
                    Debug.Log(prefix + message);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(prefix + message);
                    break;
                case LogLevel.Error:
                case LogLevel.AssertFailed:
                    Debug.LogError(prefix + message);
                    break;
            }
        }

        /// <summary>
        /// 로그 메시지의 접두사를 반환.
        /// </summary>
        /// <param name="level">로그 레벨</param>
        /// <returns>로그 접두사</returns>
        private static string GetLogPrefix(LogLevel level)
        {
            return level switch
            {
                LogLevel.Info => "[INFO]: ",
                LogLevel.Warning => "[WARNING]: ",
                LogLevel.Error => "[ERROR]: ",
                LogLevel.AssertFailed => "[ASSERT FAILED]: ",
                _ => "[UNKNOWN]: "
            };
        }
    }
}