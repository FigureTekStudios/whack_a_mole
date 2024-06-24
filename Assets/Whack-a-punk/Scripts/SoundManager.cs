using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static AudioSource audioSource;
    private static GameObject soundManager; 
  
    public static void Initialize()
    {
        if (soundManager == null)
        {
            soundManager = new GameObject("SoundManager");
            audioSource = soundManager.AddComponent<AudioSource>();
            Object.DontDestroyOnLoad(soundManager);
        }
    }

    /// <summary>
    /// Plays an audio clip.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    public static void PlaySound(AudioClip clip)
    {
        if (audioSource == null)
        {
            Debug.LogWarning("audio source is null");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlaySound called with null clip");
            return;
        }

        audioSource.PlayOneShot(clip);
    }
}
