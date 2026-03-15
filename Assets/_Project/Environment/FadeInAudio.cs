using UnityEngine;
using System.Collections;

public class MusicFadeInThenLoop : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeDuration = 2f;
    public float targetVolume = 1f;

    void Start()
    {
        audioSource.volume = 0f;
        audioSource.Play();

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

}
