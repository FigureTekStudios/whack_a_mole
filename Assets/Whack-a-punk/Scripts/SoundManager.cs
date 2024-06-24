using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton pattern implementation
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

    /// <summary>
    /// Plays an audio clip.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
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
}
