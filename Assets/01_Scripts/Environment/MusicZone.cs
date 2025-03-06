using Framework.Audio;
using UnityEngine;

public class MusicZone : MonoBehaviour
{
    public float fadeTime;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
           SoundManager.Instance.StartFadeIn("Boss", fadeTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.StartFadeOut(fadeTime);
        }
    }
}