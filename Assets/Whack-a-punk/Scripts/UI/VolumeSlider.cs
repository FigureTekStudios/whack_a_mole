using UnityEngine;
using UnityEngine.Audio;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private GameObject handle;

    [SerializeField]
    private GameObject startPosition;
    [SerializeField]
    private GameObject endPosition;

    private float _sliderLength;
    
    private Vector3 _lastPosition;

    private void Update()
    {
        if (handle.transform.position == _lastPosition) return;
        
        float sliderValue = Vector3.Distance(startPosition.transform.position, handle.transform.position) / _sliderLength;
        sliderValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        SetVolume(sliderValue);
        _lastPosition = handle.transform.position;
    }


    private void Awake()
    {
        _sliderLength = Vector3.Distance(startPosition.transform.position, endPosition.transform.position);

        float currentVolume;
        audioMixer.GetFloat("MusicVolume", out currentVolume);
        
        float sliderValue = Mathf.Pow(10, currentVolume / 20);

        handle.transform.position =
            Vector3.Lerp(startPosition.transform.position, endPosition.transform.position, sliderValue);
    }

    public void SetVolume(float sliderValue)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
    }
}
