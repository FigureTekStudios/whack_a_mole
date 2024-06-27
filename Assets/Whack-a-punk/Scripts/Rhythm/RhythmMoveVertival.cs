using System;
using UnityEngine;

public class RhythmMoveVertical : MonoBehaviour
{
    [SerializeField]
    private float maxAddedHeight = 1.5f;

    private float _maxHeight;
    [SerializeField]
    private float minAddedHeight = 0.5f;
    private float _minHeight;
    
    private Vector3 _startPosition;

    private float _timer;
    
    private bool goingUp = true;
    
    private void Start()
    {
        Conductor.Instance.OnBeat += OnBeat;
        
        _startPosition = transform.position;
        _maxHeight = _startPosition.y + maxAddedHeight;
        _minHeight = _startPosition.y + minAddedHeight;
    }
    
    private void OnDestroy()
    {
        Conductor.Instance.OnBeat -= OnBeat;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        
        float timeInPercentage = (float)(_timer / Conductor.Instance.song.spb);
        timeInPercentage = Math.Clamp(timeInPercentage, 0, 1);
        // Bezier Curve
        // timeInPercentage = timeInPercentage * timeInPercentage * (3.0f - 2.0f * timeInPercentage);
        
        // parametric
        float sqr = timeInPercentage * timeInPercentage;
        timeInPercentage = sqr / (2.0f * (sqr - timeInPercentage) + 1.0f);
        
        // going up is in exponential speed
        float height = goingUp ? Mathf.Lerp(_minHeight, _maxHeight, timeInPercentage) : Mathf.Lerp(_maxHeight, _minHeight, timeInPercentage);
        
        transform.position = new Vector3(_startPosition.x, height, _startPosition.z);
        
    }

    private void OnBeat(int currentBeatInSong, int currentBeatInMeasure, int currentMeasure)
    {
        goingUp = !goingUp;
        _timer = 0;

        if (goingUp)
        {
            transform.position = new Vector3(_startPosition.x, _minHeight, _startPosition.z);
        }
        else
        {
            transform.position = new Vector3(_startPosition.x, _maxHeight, _startPosition.z);
        }
        
    }
}
