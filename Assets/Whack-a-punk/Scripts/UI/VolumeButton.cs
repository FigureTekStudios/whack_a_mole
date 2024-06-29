using UnityEngine;
using UnityEngine.Audio;

public class VolumeButton : MonoBehaviour, IHittable
{
    [SerializeField] private bool up;
    
    [SerializeField]
    private float step = 0.2f;
    
    [SerializeField]
    private AudioMixer audioMixer;
    
    [SerializeField]
    private GameObject moveDownPoint;
    private float minHeight;

    [SerializeField] private float timeToMoveUp;
    [SerializeField] private float timeToMoveDown;
    [SerializeField] private float stayDownTime;
    
    private float maxHeight;

    private float _timer;
    
    private ButtonState state = ButtonState.idle;
    
    private void Start()
    {
        maxHeight = transform.position.y;
        minHeight = moveDownPoint.transform.position.y;
    }

    private void Update()
    {
        if (state == ButtonState.idle) return;
        
        _timer += Time.deltaTime;

        switch (state)
        {
            case ButtonState.moveDown:
                MoveDown();
                break;
            case ButtonState.moveUp:
                MoveUp();
                break;
            case ButtonState.stayDown:
                if (_timer >= stayDownTime)
                {
                    state = ButtonState.moveUp;
                    _timer = 0;
                }
                break;
        }
    }
    
    private void MoveDown()
    {
        float timeInPercent = _timer / timeToMoveUp;
        timeInPercent = Mathf.Clamp(timeInPercent, 0, 1);
        
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(maxHeight, minHeight, timeInPercent), transform.position.z);
        
        if (transform.position.y <= minHeight)
        {
            state = ButtonState.stayDown;
            _timer = 0;
        }
    }
    
    private void MoveUp()
    {
        float timeInPercent = _timer / timeToMoveUp;
        timeInPercent = Mathf.Clamp(timeInPercent, 0, 1);
        
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(minHeight, maxHeight, timeInPercent), transform.position.z);
        
        if (transform.position.y >= maxHeight)
        {
            transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z);
            state = ButtonState.idle;
            _timer = 0;
        }
    }

    public void Hit()
    {
        if (state != ButtonState.idle) return;
        
        state = ButtonState.moveDown;
        
        audioMixer.GetFloat("MusicVolume", out float currentVolume);
        float _volumeValue = Mathf.Pow(10, currentVolume / 20);
        
        _volumeValue = up? _volumeValue + step : _volumeValue - step;
        _volumeValue = Mathf.Clamp(_volumeValue, 0.0001f, 1f);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(_volumeValue) * 20);
    }
}

public enum ButtonState
{
    idle, moveDown, moveUp, stayDown
}