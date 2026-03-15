using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private readonly Dictionary<string, AudioClip> audioLookup = new();
    private AudioSource sfxAudioSource;

    // There is probably a better way to make this dynamic, but doing it simple to start
    [SerializeField] private AudioClip footstep;
    [SerializeField] private AudioClip clickIn;
    [SerializeField] private AudioClip clickOut;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioLookup.Add("footstep", footstep);
            audioLookup.Add("clickIn", clickIn);
            audioLookup.Add("clickOut", clickOut);
        }
    }

    void Start()
    {
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySfx(string sfx, float volume = 1f)
    {
        if (audioLookup.TryGetValue(sfx, out var clip))
        {
            sfxAudioSource.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogWarning($"Sound '{sfx}' not found.");
        }
    }
}
