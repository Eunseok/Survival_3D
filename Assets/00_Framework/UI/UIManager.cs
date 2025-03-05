using System.Collections.Generic;
using DefaultNamespace;
using Framework.Utilities;
using UnityEngine;

namespace Scripts.UI
{
    public class UIManager : Singleton<UIManager>
    {
        private const string HUDPath = "UI/Hud/";
        private const string PopupPath = "UI/Popup/";

        private int _currentOrder = 10; // 현재까지 최근에 사용한 오더
        private readonly Stack<UIPopup> _popupStack = new Stack<UIPopup>();
        private UIHud _hudUI;
        public UIHud HudUI => _hudUI;
        

        protected override void InitializeManager()
        {
            Debug.Log("UIManager Initialized");
        }

        private GameObject Root
        {
            get
            {
                var root = GameObject.Find("@UI_Root") ?? new GameObject { name = "@UI_Root" };
                return root;
            }
        }

        public void SetCanvas(GameObject go, bool sort = true)
        {
            var canvas = go.GetOrAddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = sort ? _currentOrder++ : 0;
        }

        public T ShowHud<T>() where T : UIHud
        {
            return  ShowHud(typeof(T).Name) as T;
        }

        public UIHud ShowHud(string hudName)
        {
            var prefab = LoadUIResource(HUDPath, hudName);
            if (prefab == null)
                return null;

            return CreateHudInstance(prefab);
        }

        private UIHud CreateHudInstance(GameObject prefab)
        {
            var instance = Instantiate(prefab, Root.transform);
            return EnableUIComponent<UIHud>(instance);
        }

        public T ShowPopup<T>() where T : UIPopup
        {
            return ShowPopup(typeof(T).Name) as T;
        }

        public UIPopup ShowPopup(string popupName)
        {
            var prefab = LoadUIResource(PopupPath, popupName);
            if (prefab == null)
                return null;

            return CreatePopupInstance(prefab);
        }

        private UIPopup CreatePopupInstance(GameObject prefab)
        {
            var instance = Instantiate(prefab, Root.transform);
            return EnableUIComponent<UIPopup>(instance);
        }

        private GameObject LoadUIResource(string path, string resourceName)
        {
            var resource = Resources.Load($"{path}{resourceName}", typeof(GameObject)) as GameObject;
            if (resource == null)
                Debug.LogError($"UI Resource '{resourceName}' not found in path '{path}'");
            return resource;
        }

        private T EnableUIComponent<T>(GameObject obj) where T : UIBase
        {
            var component = obj.GetComponent<T>();
            if (component is UIPopup popup)
                _popupStack.Push(popup);
            else if (component is UIHud hud)
                _hudUI = hud;

            obj.SetActive(true);
            return component;
        }

        public void ClosePopup(UIPopup popup)
        {
            if (_popupStack.Count == 0 || _popupStack.Peek() != popup)
            {
                Debug.LogWarning("Close Popup Failed: Mismatched popup or empty stack.");
                return;
            }

            ClosePopup();
        }

        public void ClosePopup()
        {
            if (_popupStack.Count == 0)
                return;

            var popup = _popupStack.Pop();
            Destroy(popup.gameObject);
            _currentOrder--;
        }

        public void CloseAllPopup()
        {
            while (_popupStack.Count > 0)
                ClosePopup();
        }
    }
}