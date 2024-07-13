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
    [SerializeField] List<AudioClip> tauntAudioClips;

    [Header("Game Announcer VO Audio Clips")]
    [SerializeField] List<AudioClip> shockAudioClips;
    [SerializeField] List<AudioClip> onGetPointsAudioClips;

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
    public void PlayZombieIdleSFX()
    {

    }

    public void PlayZombieHitSFX()
    {

    }

    public void PlayZombieRevealSFX()
    {

    }

    public void PlayZombieRetreatSFX()
    {

    }

    public void PlayZombieTauntSFX()
    {

    }
}
