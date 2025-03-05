using UnityEngine;

namespace Scripts.UI
{
    public class UIPopup : UIBase
    {
        protected virtual void Awake()
        {
            UIManager.Instance.SetCanvas(gameObject);
        }

        
        public virtual void Initialize()
        {
            Debug.Log($"[{gameObject.name}] - Initialize called.");
        }
        
        protected virtual void Start()
        {
            PlayShowAnimation();
        }

        protected virtual void PlayShowAnimation()
        {
            // transform.localScale = Vector3.one * 0.8f;
            // transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
        }

        /// <summary>
        /// 팝업이 닫힐 때 정리 작업 처리. 서브클래스에서 구현.
        /// </summary>
        public virtual void OnClose()
        {
            Debug.Log($"[{gameObject.name}] - OnClose called.");
        }

        public virtual void ClosePopup()  // 팝업이니까 고정 캔버스(Scene)과 다르게 닫는게 필요
        {
           UIManager.Instance.ClosePopup(this);
        }
    }
}