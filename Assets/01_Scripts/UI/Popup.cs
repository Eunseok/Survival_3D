using UnityEngine;

namespace Framework.UI.Scripts
{
    /// <summary>
    /// Popup 베이스 클래스
    /// </summary>
    public abstract class Popup : MonoBehaviour
    {
        /// <summary>
        /// 팝업 초기화
        /// </summary>
        public virtual void Initialize(params object[] args)
        {
        }

        /// <summary>
        /// 팝업 닫기 버튼
        /// </summary>
        public void Close()
        {
            UIManager.Instance.PopupManager.ClosePopup(this);
        }
    }
}