using UnityEngine;

namespace Framework.UI.Scripts
{
    /// <summary>
    /// 메인 메뉴 UI 클래스
    /// </summary>
    public class MainMenu : BaseUI
    {
        /// <summary>
        /// 게임 시작 버튼 동작
        /// </summary>
        public void StartGame()
        {
            Debug.Log("Start Game button clicked!");
            UIManager.Instance.ShowUI("LoadingScreen");
        }

        /// <summary>
        /// 게임 종료 버튼 동작
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("Quit Game button clicked!");
            Application.Quit();
        }

        /// <summary>
        /// 메인 메뉴가 활성화될 때 추가 동작
        /// </summary>
        protected override void OnShow()
        {
            Debug.Log($"MainMenu '{UIName}' is now shown.");
        }

        /// <summary>
        /// 메인 메뉴가 비활성화될 때 추가 동작
        /// </summary>
        protected override void OnHide()
        {
            Debug.Log("Main Menu is now hidden.");
        }
    }
}