using UnityEngine;
using System;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine.UI;

public abstract class UIBase : MonoBehaviour
{
    private readonly Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();
    
    
    // 자동 매핑 함수
    protected void AutoBind<T>(Type enumType) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(enumType);
        UnityEngine.Object[] mappedObjects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), mappedObjects);

        for (int i = 0; i < names.Length; i++)
        {
            mappedObjects[i] = FindObject<T>(names[i]);

            if (mappedObjects[i] == null)
                Debug.Log($"Failed to bind({names[i]})");
        }
    }

    private T FindObject<T>(string objName) where T : UnityEngine.Object
    {
        return typeof(T) == typeof(GameObject)
            ? Util.FindChild(gameObject, objName, true) as T
            : Util.FindChild<T>(gameObject, objName, true);
    }

    private T Get<T>(int index) where T : UnityEngine.Object
    {
        return _objects.TryGetValue(typeof(T), out var mappedObjects) ? mappedObjects[index] as T : null;
    }

    public GameObject GetObject(int index) => Get<GameObject>(index);
    public TextMeshProUGUI GetTextFromGameObject(int index) => Get<GameObject>(index).GetComponent<TextMeshProUGUI>();
    public Button GetButtonFromGameObject(int index) => Get<GameObject>(index).GetComponent<Button>();
    public Image GetImageFromGameObject(int index) => Get<GameObject>(index).GetComponent<Image>();
    protected TextMeshProUGUI GetText(int index) => Get<TextMeshProUGUI>(index);
    protected Button GetButton(int index) => Get<Button>(index);
    protected Image GetImage(int index) => Get<Image>(index);
}