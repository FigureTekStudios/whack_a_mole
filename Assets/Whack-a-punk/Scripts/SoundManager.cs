using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private AudioSource audioSource;

    private int idleCounter = 0;
    private int idleMaxCount = 2;

    private int revealedCounter;
    private int revealedMaxCount = 2;

    private int retreatCounter;
    private int retreatMaxCount = 2;


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
            audioSource = GetComponent<AudioSource>();
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

    public IEnumerator PlayZombieIdleSFX(AudioSource source)
    {
        if (audioSource.isPlaying || source.isPlaying)
        {
            Debug.LogWarning("SoundManager: prioritizing main audio source.");
            yield return null;
        }

        if (idleCounter >= idleMaxCount) 
        {
            Debug.LogWarning("SoundManager: idle sounds Maxed out.");
            yield return null;
        }

        AudioClip clip = null;
        int randIndex = Random.Range(0, idleAudioClips.Count);
        clip = idleAudioClips[randIndex];

        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlaySound called with null clip");
            yield return null;
        }
        idleCounter++;
        source.clip = clip;
        source.Play();
        yield return new WaitUntil(() => !source.isPlaying);
        idleCounter--;
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

    public IEnumerator PlayZombieRevealSFX(AudioSource source)
    {
        if (audioSource.isPlaying || source.isPlaying)
        {
            Debug.LogWarning("SoundManager: prioritizing main audio source.");
            yield return null;
        }

        if (revealedCounter >= revealedMaxCount)
        {
            Debug.LogWarning("SoundManager: revealed sounds Maxed out.");
            yield return null;
        }

        AudioClip clip = null;
        int randIndex = Random.Range(0, revealAudioClips.Count);
        clip = revealAudioClips[randIndex];

        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlaySound called with null clip");
            yield return null;
        }

        revealedCounter++;  
        source.clip = clip;
        source.Play();
        yield return new WaitUntil(() => !source.isPlaying);
        revealedCounter--;
    }

    public IEnumerator PlayZombieRetreatSFX(AudioSource source)
    {
        if (audioSource.isPlaying || source.isPlaying)
        {
            Debug.LogWarning("SoundManager: prioritizing main audio source.");
            yield return null;
        }

        if (retreatCounter >= retreatMaxCount)
        {
            Debug.LogWarning("SoundManager: retreat sounds Maxed out.");
            yield return null;
        }

        AudioClip clip = null;
        int randIndex = Random.Range(0, retreatAudioClips.Count);
        clip = retreatAudioClips[randIndex];

        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlaySound called with null clip");
            yield return null;
        }

        retreatCounter++; 
        source.clip = clip;
        source.Play();
        yield return new WaitUntil(() => !source.isPlaying);
        retreatCounter--;
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
