using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float duration;

    private Color startColor;

    private void Awake()
    {
        startColor = image.color;
        image.enabled = false;
    }

    private void Start()
    {
        CharacterManager.Instance.Player.resourceController.OnTakeDamage += Flash;
    }


    private Coroutine coroutine;

    public void Flash()
    {
        if (coroutine != null) StopCoroutine(coroutine);

        image.enabled = true;
        image.color = startColor;
        coroutine = StartCoroutine(FadeAway());
    }

    private IEnumerator FadeAway()
    {
        float startAlpha = image.color.a;
        float a = startAlpha;


        while (a > 0)
        {
            a -= (startAlpha / duration) * Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, a);
            yield return null;
        }

        image.enabled = true;
    }
}