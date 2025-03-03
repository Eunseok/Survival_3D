using System;
using Framework.Utilities;
using UnityEngine;

namespace Framework.Core
{
    /// <summary>
    /// 게임 상태 및 전체 흐름을 관리하는 기본 GameStateManager 클래스.
    /// </summary>
    public class GameStateManager : Singleton<GameStateManager>
    {
        public enum GameState
        {
            MainMenu,
            Playing,
            Paused,
            GameOver
        }

        [Header("Game State")] [SerializeField]
        private GameState currentState = GameState.MainMenu;

        /// <summary>
        /// 게임 상태 변경 이벤트. 외부에서 구독 가능합니다.
        /// </summary>
        public event Action<GameState> OnGameStateChanged;

        /// <summary>
        /// 현재 게임 상태를 제공.
        /// </summary>
        public GameState CurrentState => currentState;

        /// <summary>
        /// 초기화 작업.
        /// </summary>
        protected override void InitializeManager()
        {
            Debug.Log("GameStateManager Initialized");
        }

        /// <summary>
        /// 게임 상태를 변경합니다.
        /// </summary>
        /// <param name="newState">새로운 게임 상태</param>
        public void SetGameState(GameState newState)
        {
            if (currentState == newState) return;

            currentState = newState;
            InvokeGameStateChangeEvent(currentState);
        }

        /// <summary>
        /// 게임 상태 변경 이벤트를 호출합니다.
        /// </summary>
        /// <param name="state">변경된 상태</param>
        private void InvokeGameStateChangeEvent(GameState state)
        {
            Debug.Log($"Game State Changed to: {state}");
            OnGameStateChanged?.Invoke(state); // 구독된 이벤트 호출
        }

        /// <summary>
        /// 게임 종료 처리 (필요 시 확장 가능).
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("Quitting Game...");
            Application.Quit();
        }
    }
}