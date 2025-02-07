using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private AudioSource audioSource;

    // This is kinda a global counter to ensure only two vo SFX are playing
    // for any state.
    private int totalSFXCounter = 0;
    [SerializeField] int maxTotalSFXCount = 2;

    private int idleCounter = 0;
    [SerializeField] int idleMaxCount = 1;

    private int revealedCounter;
    [SerializeField] int revealedMaxCount = 1;

    private int retreatCounter;
    [SerializeField] int retreatMaxCount = 1;


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
            yield break;
        }

        if (totalSFXCounter >= maxTotalSFXCount)
        {
            Debug.LogWarning("SoundManager: Too many SFX playing, not playing this one.");
            yield break;
        }

        if (idleCounter >= idleMaxCount) 
        {
            Debug.LogWarning("SoundManager: idle sounds Maxed out.");
            yield break;
        }

        AudioClip clip = null;
        int randIndex = Random.Range(0, idleAudioClips.Count);
        clip = idleAudioClips[randIndex];

        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlaySound called with null clip");
            yield break;
        }
        idleCounter++;
        totalSFXCounter++;
        source.clip = clip;
        source.Play();
        yield return new WaitUntil(() => !source.isPlaying);
        yield return new WaitForSeconds(1.5f); // A slight delay to prevent a similar SFX from playing immediately. 
        idleCounter--;
        totalSFXCounter--;
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
            yield break;
        }

        if (totalSFXCounter >= maxTotalSFXCount)
        {
            Debug.LogWarning("SoundManager: Too many SFX playing, not playing this one.");
            yield break;
        }

        if (revealedCounter >= revealedMaxCount)
        {
            Debug.LogWarning("SoundManager: revealed sounds Maxed out.");
            yield break;
        }

        AudioClip clip = null;
        int randIndex = Random.Range(0, revealAudioClips.Count);
        clip = revealAudioClips[randIndex];

        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlaySound called with null clip");
            yield break;
        }

        revealedCounter++;
        totalSFXCounter++;
        source.clip = clip;
        source.Play();
        yield return new WaitUntil(() => !source.isPlaying);
        yield return new WaitForSeconds(1.5f); // A slight delay to prevent a similar SFX from playing immediately. 
        revealedCounter--;
        totalSFXCounter--;
    }

    public IEnumerator PlayZombieRetreatSFX(AudioSource source)
    {
        if (audioSource.isPlaying || source.isPlaying)
        {
            Debug.LogWarning("SoundManager: prioritizing main audio source.");
            yield break;
        }

        if (totalSFXCounter >= maxTotalSFXCount)
        {
            Debug.LogWarning("SoundManager: Too many SFX playing, not playing this one.");
            yield break;
        }

        if (retreatCounter >= retreatMaxCount)
        {
            Debug.LogWarning("SoundManager: retreat sounds Maxed out.");
            yield break;
        }

        AudioClip clip = null;
        int randIndex = Random.Range(0, retreatAudioClips.Count);
        clip = retreatAudioClips[randIndex];

        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlaySound called with null clip");
            yield break;
        }

        retreatCounter++;
        totalSFXCounter++;
        source.clip = clip;
        source.Play();
        yield return new WaitUntil(() => !source.isPlaying);
        yield return new WaitForSeconds(1.5f); // A slight delay to prevent a similar SFX from playing immediately. 
        retreatCounter--;
        totalSFXCounter--;
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

    public void PlayOnUsePowerUpVO()
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
