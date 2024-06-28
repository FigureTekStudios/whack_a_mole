using UnityEngine;

public class CircleTimer : MonoBehaviour
{
    private Material _material;
    
    [SerializeField] 
    private float maxRadius = 0.41f;
    [SerializeField] 
    private float minRadius = 0.14f;
    [SerializeField] 
    private float hideRadius = 0.14f;
    
    [SerializeField] 
    private float yellowTimeInBeats = 4;
    private float yellowTimeInSeconds;
    
    [SerializeField] 
    private float greenTimeInBeats = 2;
    private float greenTimeInSeconds;

    private bool _enabled;

    private float _timeInBeats;
    private float _timeInSeconds;

    private float _timer;
    private float _radius;
    

    private void Awake()
    {
        if (Conductor.Instance)
        {
            Conductor.Instance.UpdateAudioTime += UpdateAudioTime;
            yellowTimeInSeconds = (float)(yellowTimeInBeats * Conductor.Instance.song.spb);
            greenTimeInSeconds = (float)(greenTimeInBeats * Conductor.Instance.song.spb);
        }

        _material = GetComponent<MeshRenderer>().material;
        _material.SetFloat("_radius", hideRadius);
    }
    
    private void OnDestroy()
    {
        if (Conductor.Instance)
            Conductor.Instance.UpdateAudioTime -= UpdateAudioTime;
    }


    private void UpdateAudioTime(float audioTime, float audioTimeDelta)
    {
        if (!_enabled) return;
            
        _timer -= audioTimeDelta;

        float timeInPercentage = _timer / _timeInSeconds;
        
        float adjustedRadius = Mathf.Lerp(minRadius, maxRadius, timeInPercentage);
        
        _material.SetFloat("_radius", adjustedRadius);

        if (_timer <= greenTimeInSeconds)
        {
            _material.SetColor("_color", Color.green);
        }
        else if (_timer <= yellowTimeInSeconds)
        {
            _material.SetColor("_color", Color.yellow);
        }
        
        if (_timer <= 0)
        {
            _material.SetFloat("_radius", hideRadius);
            _enabled = false;
        }
    }
    
    public void SetTimeInBeats(float timeInBeats)
    {
        _timeInBeats = timeInBeats;
        _timeInSeconds = (float)(timeInBeats * Conductor.Instance.song.spb);
        _timer = _timeInSeconds;
        _enabled = true;
        _material.SetColor("_color", Color.red);
        _material.SetFloat("_radius", maxRadius);
    }
    
    public void StopTimer()
    {
        _timeInSeconds = 0;
        _timer = 0;
        _timeInBeats = 0;
        _enabled = false;
        _material.SetFloat("_radius", hideRadius);
    }
}
