using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioManager;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioList;
    private static AudioManager instance;
    public AudioSource SFXSource;
    public AudioSource BGMSource;
    //Random pitch variation
    public float LowPitchRange = 0.95f;
    public float HighPitchRange = 1.05f;

    //Dictionary<SoundType, AudioClip> _mySoundDictionary = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        SFXSource = GetComponent<AudioSource>();
    }
    public static void PlaySFX(AudioClip clip)
    {
        instance.SFXSource.pitch = 1;
        instance.SFXSource.PlayOneShot(clip);
    }
    public static void PlaySFXvariation(AudioClip clip)
    {
        float randomPitch = UnityEngine.Random.Range(instance.LowPitchRange, instance.HighPitchRange);
        instance.SFXSource.pitch = randomPitch;
        instance.SFXSource.PlayOneShot(clip);
    }
}