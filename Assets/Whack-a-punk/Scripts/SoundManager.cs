using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private AudioSource audioSource;

    [Header("Zombie Punk VO Audio Clips")]
    [SerializeField] List<AudioClip> idleAudioClips;
    [SerializeField] List<AudioClip> hitAudioClips;
    [SerializeField] List<AudioClip> revealAudioClips;
    [SerializeField] List<AudioClip> retreatAudioClips;
    [SerializeField] List<AudioClip> retreatDamagedAudioClips;
    [SerializeField] List<AudioClip> tauntAudioClips;

    [Header("Game Announcer VO Audio Clips")]
    [SerializeField] List<AudioClip> onAddScoreAudioClips;
    [SerializeField] List<AudioClip> onUsePowerUpAudioClips;
    [SerializeField] List<AudioClip> onObtainedPowerUpAudioClips;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlaySound called with null clip");
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();
    }

    public void PlayZombieIdleSFX(AudioSource source)
    {
        AudioClip clip = null;
        int randIndex = Random.Range(0, idleAudioClips.Count);
        clip = idleAudioClips[randIndex];

        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlaySound called with null clip");
            return;
        }

        source.clip = clip;
        source.Play();
    }

    public void PlayZombieHitSFX(AudioSource source)
    {
        AudioClip clip = null;
        int randIndex = Random.Range(0, hitAudioClips.Count);
        clip = hitAudioClips[randIndex];

        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlaySound called with null clip");
            return;
        }

        source.clip = clip;
        source.Play();
    }

    public void PlayZombieRevealSFX(AudioSource source)
    {
        AudioClip clip = null;
        int randIndex = Random.Range(0, revealAudioClips.Count);
        clip = revealAudioClips[randIndex];

        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlaySound called with null clip");
            return;
        }

        source.clip = clip;
        source.Play();
    }

    public void PlayZombieRetreatSFX(AudioSource source)
    {
        AudioClip clip = null;
        int randIndex = Random.Range(0, retreatAudioClips.Count);
        clip = retreatAudioClips[randIndex];

        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlaySound called with null clip");
            return;
        }

        source.clip = clip;
        source.Play();
    }

    public void PlayZombieTauntSFX(AudioSource source)
    {
        AudioClip clip = null;
        int randIndex = Random.Range(0, tauntAudioClips.Count);
        clip = tauntAudioClips[randIndex];

        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlaySound called with null clip");
            return;
        }

        source.clip = clip;
        source.Play();
    }

    public void PlayOnAddScoreSFX(AudioSource source, int multiplier = 1)
    {
        if (multiplier == 2)
            PlaySound(onAddScoreAudioClips[0]);
        else if (multiplier == 3)
            PlaySound(onAddScoreAudioClips[1]);
    }

    public void PlayOnUsePowerUpSFX()
    {
        int randIndex = Random.Range(0, onUsePowerUpAudioClips.Count);
        PlaySound(onUsePowerUpAudioClips[randIndex]);

    }

    public void PlayOnObtainedPowerUpSFX()
    {
        int randIndex = Random.Range(0, onObtainedPowerUpAudioClips.Count);
        PlaySound(onUsePowerUpAudioClips[randIndex]);
    }
}
