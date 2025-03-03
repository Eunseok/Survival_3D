using UnityEngine;

namespace Framework.UI.Scripts
{
    /// <summary>
    /// 정적인 UI의 공통 베이스 클래스
    /// </summary>
    public abstract class BaseUI : MonoBehaviour
    {
        [Header("UI Settings")] [SerializeField]
        private string uiName;

        public string UIName => uiName;

        public bool IsActive { get; private set; }

        /// <summary>
        /// UI 활성화
        /// </summary>
        public virtual void Show()
        {
            if (IsActive) return;
            gameObject.SetActive(true);
            IsActive = true;
            OnShow();
        }

        /// <summary>
        /// UI 비활성화
        /// </summary>
        public virtual void Hide()
        {
            if (!IsActive) return;
            gameObject.SetActive(false);
            IsActive = false;
            OnHide();
        }

        /// <summary>
        /// UI가 표시될 때 호출
        /// </summary>
        protected virtual void OnShow()
        {
        }

        /// <summary>
        /// UI가 숨겨질 때 호출
        /// </summary>
        protected virtual void OnHide()
        {
        }
    }
}