using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AudioManager;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioList;
    [SerializeField] private AudioClip[] wolfAttackAudio;
    [SerializeField] private AudioClip[] rapAttackAudio;
    private static AudioManager instance;
    public AudioSource SFXSource;
    public AudioSource BGMSource;
    public AudioLowPassFilter BGMlowPassFilter;
    //Random pitch variation
    public float LowPitchRange = 0.95f;
    public float HighPitchRange = 1.05f;

    //Dictionary<SoundType, AudioClip> _mySoundDictionary = new();
    private bool isPaused = false;

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
    public static void PlayWolfAttackSFX()
    {
        int randomIndex = UnityEngine.Random.Range(0, instance.wolfAttackAudio.Length);
        AudioClip clip = instance.wolfAttackAudio[randomIndex];

        float randomPitch = UnityEngine.Random.Range(instance.LowPitchRange, instance.HighPitchRange);
        instance.SFXSource.pitch = randomPitch;

        
        instance.SFXSource.PlayOneShot(clip);
    }
    public static void PlayRapAttackSFX()
    {
        int randomIndex = UnityEngine.Random.Range(0, instance.rapAttackAudio.Length);
        AudioClip clip = instance.rapAttackAudio[randomIndex];

        float randomPitch = UnityEngine.Random.Range(instance.LowPitchRange, instance.HighPitchRange);
        instance.SFXSource.pitch = randomPitch;


        instance.SFXSource.PlayOneShot(clip);
    }
    IEnumerator WaitBetweenAttack()
    {
        yield return new WaitForSeconds(1f);
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
    public static void TogglePauseBGM(AudioClip clip)
    {
        if(instance.isPaused)
        {
            instance.BGMlowPassFilter.enabled = true;
            instance.BGMlowPassFilter.cutoffFrequency = 500f;
        }
        else
        {
            instance.BGMlowPassFilter.enabled = false;
            instance.BGMlowPassFilter.cutoffFrequency = 5000f;
        }
    }
}