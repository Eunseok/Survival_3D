using System.Collections.Generic;
using Framework.UI.Scripts;
using Framework.Utilities;
using UnityEngine;

namespace Framework.UI
{
    /// <summary>
    /// UIManager: 일반 UI와 PopupManager 관리
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        private readonly Dictionary<string, BaseUI> _uiDictionary = new(); // Static UI
        private PopupManager _popupManager; // PopupManager 관리

        protected override void InitializeManager()
        {
            Debug.Log("UIManager Initialized");
            _popupManager = new PopupManager(transform);
        }

        /// <summary>
        /// 일반 UI 등록
        /// </summary>
        public void RegisterUI(BaseUI ui)
        {
            // if (ui == null || _uiDictionary.ContainsKey(ui.UIName)) return;
            //
            // _uiDictionary.Add(ui.UIName, ui);
            // Debug.Log($"UI '{ui.UIName}' registered.");
        }

        /// <summary>
        /// 일반 UI 활성화
        /// </summary>
        public void ShowUI(string uiName)
        {
            if (_uiDictionary.TryGetValue(uiName, out var ui))
            {
                foreach (var otherUI in _uiDictionary.Values)
                {
                    if (otherUI != ui)
                        otherUI.Hide();
                }

                ui.Show();
            }
            else
            {
                Debug.LogError($"UI '{uiName}' not found!");
            }
        }

        /// <summary>
        /// PopupManager 핸들러 반환
        /// </summary>
        public PopupManager PopupManager => _popupManager;
    }
}