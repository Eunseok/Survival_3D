using System;
using System.Collections.Generic;

namespace Framework.Core
{
    /// <summary>
    /// 전역 이벤트 시스템. 타입 안전성과 파라미터를 지원하는 제네릭 기반 설계.
    /// </summary>
    public static class EventSystem
    {
        // 이벤트 테이블
        private static readonly Dictionary<Type, Delegate> EventTable = new Dictionary<Type, Delegate>();

        /// <summary>
        /// 이벤트를 구독합니다.
        /// </summary>
        /// <typeparam name="T">이벤트 타입(Action 또는 Action제네릭)</typeparam>
        /// <param name="listener">구독할 리스너</param>
        public static void Subscribe<T>(Action<T> listener)
        {
            var eventType = typeof(T);

            if (EventTable.TryGetValue(eventType, out var existingDelegate))
            {
                EventTable[eventType] = (Action<T>)existingDelegate + listener;
            }
            else
            {
                EventTable[eventType] = listener;
            }
        }

        /// <summary>
        /// 파라미터 없는 이벤트를 구독합니다.
        /// </summary>
        /// <param name="listener">구독할 리스너</param>
        public static void Subscribe(Action listener)
        {
            Subscribe<object>(_ => listener());
        }

        /// <summary>
        /// 이벤트 구독을 취소합니다.
        /// </summary>
        /// <typeparam name="T">이벤트 타입(Action 또는 Action제네릭")</typeparam>
        /// <param name="listener">제거할 리스너</param>
        public static void Unsubscribe<T>(Action<T> listener)
        {
            var eventType = typeof(T);

            if (EventTable.TryGetValue(eventType, out var existingDelegate))
            {
                var currentDelegate = (Action<T>)existingDelegate;
                currentDelegate -= listener;

                if (currentDelegate == null)
                {
                    EventTable.Remove(eventType);
                }
                else
                {
                    EventTable[eventType] = currentDelegate;
                }
            }
        }

        /// <summary>
        /// 파라미터 없는 이벤트 구독 취소.
        /// </summary>
        /// <param name="listener">제거할 리스너</param>
        public static void Unsubscribe(Action listener)
        {
            Unsubscribe<object>(_ => listener());
        }

        /// <summary>
        /// 이벤트를 호출합니다.
        /// </summary>
        /// <typeparam name="T">호출할 이벤트 타입</typeparam>
        /// <param name="param">이벤트에 전달될 파라미터</param>
        public static void Invoke<T>(T param)
        {
            var eventType = typeof(T);

            if (EventTable.TryGetValue(eventType, out var existingDelegate))
            {
                ((Action<T>)existingDelegate)?.Invoke(param);
            }
        }

        /// <summary>
        /// 파라미터 없는 이벤트 호출.
        /// </summary>
        public static void Invoke()
        {
            Invoke<object>(null);
        }

        /// <summary>
        /// 현재 등록된 이벤트 목록 디버그 정보.
        /// </summary>
        public static void PrintDebugInfo()
        {
            Console.WriteLine("===== EventSystem Debug Info =====");
            foreach (var eventType in EventTable.Keys)
            {
                Console.WriteLine(
                    $"Event Type: {eventType.Name}, Delegate Count: {EventTable[eventType].GetInvocationList().Length}");
            }

            Console.WriteLine("===================================");
        }
    }
}