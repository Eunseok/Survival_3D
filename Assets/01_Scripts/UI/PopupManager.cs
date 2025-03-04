using System.Collections.Generic;
using UnityEngine;

namespace Scripts.UI
{
    /// <summary>
    /// PopupManager: 다양한 Popup UI 관리
    /// </summary>
    public class PopupManager
    {
        private readonly Dictionary<string, GameObject> _popupPrefabs = new(); // 팝업 프리팹 목록
        private readonly List<Popup> _activePopups = new(); // 활성화된 팝업 리스트
        private readonly Transform _parentTransform;

        private const string PopupPrefabPath = "Popups/"; // Resources/Popups 경로

        public PopupManager(Transform parentTransform)
        {
            _parentTransform = parentTransform;
        }

        /// <summary>
        /// 팝업 생성
        /// </summary>
        /// <typeparam name="T">Popup을 상속받은 타입</typeparam>
        /// <param name="popupName">프리팹 경로 (Resources/Popups/{popupName})</param>
        /// <param name="args">팝업에 전달할 초기 인자</param>
        public T CreatePopup<T>(string popupName, params object[] args) where T : Popup
        {
            if (!_popupPrefabs.TryGetValue(popupName, out var prefab))
            {
                prefab = Resources.Load<GameObject>(PopupPrefabPath + popupName);
                if (prefab == null)
                {
                    Debug.LogError($"Popup prefab '{popupName}' not found in Resources!");
                    return null;
                }

                _popupPrefabs[popupName] = prefab; // Prefab 캐싱
            }

            // 팝업 생성
            var popupInstance = Object.Instantiate(prefab, _parentTransform);
            var popupScript = popupInstance.GetComponent<T>();

            if (popupScript == null)
            {
                Debug.LogError($"Popup prefab '{popupName}' is not of type '{typeof(T)}'!");
                Object.Destroy(popupInstance);
                return null;
            }

            popupScript.Initialize(args); // 팝업 초기화
            _activePopups.Add(popupScript);

            return popupScript;
        }

        /// <summary>
        /// 팝업 제거
        /// </summary>
        public void ClosePopup(Popup popup)
        {
            if (_activePopups.Contains(popup))
            {
                _activePopups.Remove(popup);
                Object.Destroy(popup.gameObject);
            }
        }

        /// <summary>
        /// 모든 팝업 제거
        /// </summary>
        public void CloseAllPopups()
        {
            foreach (var popup in _activePopups)
            {
                if (popup != null)
                {
                    Object.Destroy(popup.gameObject);
                }
            }

            _activePopups.Clear();
        }
    }
}