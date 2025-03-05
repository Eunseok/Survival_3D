using System;
using _01_Scripts.UI;
using Framework.Utilities;
using Scripts.UI;
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
            Gameplay,
        }

        [Header("Game State")] [SerializeField]
        private GameState currentState = GameState.Gameplay;

        /// <summary>
        /// 게임 상태 변경 이벤트. 외부에서 구독 가능합니다.
        /// </summary>
        public event Action OnGameStateChanged;

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

        private void Start()
        {
            UIManager.Instance.ShowHud<HUDGame>();
            //LoadSceneByGameState(currentState);
        }

        /// <summary>
        /// 게임 상태를 변경합니다.
        /// </summary>
        /// <param name="newState">새로운 게임 상태</param>
        public void SetGameState(GameState newState)
        {
            if (currentState == newState) return;

            currentState = newState;
            LoadSceneByGameState(currentState);
        }
        
        public void LoadSceneByGameState(GameState state)
        {
            string sceneName;

            switch (state)
            {
                case GameState.Gameplay:
                    sceneName = "SampleScene"; // 메인 메뉴 씬 이름
                    UIManager.Instance.ShowHud<HUDGame>();
                    break;
                default:
                    Debug.LogWarning("Unhandled GameState!"); 
                    return;
            }

            // 씬 로드
            Debug.Log($"Loading Scene: {sceneName}");
            SceneLoader.Instance.LoadScene(sceneName, OnGameStateChanged);
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