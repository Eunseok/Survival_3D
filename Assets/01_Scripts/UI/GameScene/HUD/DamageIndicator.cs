using System.Collections;
using Scripts.Characters;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float duration;

    private Color _startColor;

    private void Awake()
    {
        _startColor = image.color;
        image.enabled = false;
    }

    private void Start()
    {
        CharacterManager.Instance.Player.ResourceHandler.OnTakeDamage += Flash;
    }


    private Coroutine _coroutine;

    private void Flash()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);

        image.enabled = true;
        image.color = _startColor;
        _coroutine = StartCoroutine(FadeAway());
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