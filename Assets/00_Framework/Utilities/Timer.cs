using System;
using UnityEngine;

namespace Framework.Utilities
{
    /// <summary>
    /// 간단한 타이머 클래스.
    /// </summary>
    public class Timer
    {
        private readonly float _duration; // 타이머 지속 시간 (변경 불가능)
        private float _elapsedTime; // 경과 시간
        private bool _isRunning; // 타이머 실행 여부
        private readonly bool _useUnscaledTime; // 타임스케일 적용 여부
        
        public readonly Action OnComplete; // 완료 시 호출되는 콜백

        /// <summary>
        /// 타이머 남은 시간 (초 단위).
        /// </summary>
        public float RemainingTime => Mathf.Max(0, _duration - _elapsedTime);

        /// <summary>
        /// 타이머가 완료되었는지 여부.
        /// </summary>
        public bool IsComplete => _elapsedTime >= _duration;

        /// <summary>
        /// 타이머 진행 중인지 여부.
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// 경과 시간.
        /// </summary>
        public float ElapsedTime => _elapsedTime;

        /// <summary>
        /// Timer 생성자.
        /// </summary>
        /// <param name="durationInSeconds">지속 시간 (초 단위)</param>
        /// <param name="onComplete">타이머 완료 시 호출될 콜백</param>
        /// <param name="useUnscaledTime">타임스케일 영향을 받지 않도록 설정</param>
        public Timer(float durationInSeconds, Action onComplete = null, bool useUnscaledTime = false)
        {
            if (durationInSeconds <= 0)
            {
                throw new ArgumentException("Timer duration must be greater than zero.", nameof(durationInSeconds));
            }

            _duration = durationInSeconds;
            this.OnComplete = onComplete;
            this._useUnscaledTime = useUnscaledTime;

            Reset();
        }

        /// <summary>
        /// 타이머를 시작합니다.
        /// </summary>
        public void Start()
        {
            _isRunning = true;
        }

        /// <summary>
        /// 타이머를 정지합니다. 현재 상태 유지.
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
        }

        /// <summary>
        /// 타이머를 초기 상태로 되돌립니다.
        /// </summary>
        public void Reset()
        {
            _elapsedTime = 0;
            _isRunning = false;
        }

        /// <summary>
        /// 매 프레임 호출하여 타이머를 업데이트합니다.
        /// 게임 루프 내 Update에서 호출 필요.
        /// </summary>
        public void Update()
        {
            if (!_isRunning) return;

            // deltaTime 적용
            _elapsedTime += _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            if (_elapsedTime >= _duration)
            {
                Stop();
                InvokeOnComplete();
            }
        }

        /// <summary>
        /// 타이머 완료 콜백 실행.
        /// </summary>
        private void InvokeOnComplete()
        {
            OnComplete?.Invoke();
        }

        /// <summary>
        /// 타이머 상태를 문자열로 출력합니다.
        /// </summary>
        public override string ToString()
        {
            return
                $"Timer [IsRunning: {_isRunning}, ElapsedTime: {_elapsedTime:F2}s, RemainingTime: {RemainingTime:F2}s]";
        }
    }
}