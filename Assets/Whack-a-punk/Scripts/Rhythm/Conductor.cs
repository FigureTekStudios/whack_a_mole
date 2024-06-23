using System;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    
    public delegate void UpdateAudioTimeEventHandler(double audioTime, double audioTimeDelta);
    public event UpdateAudioTimeEventHandler UpdateAudioTime;
    
    public delegate void OnBeatEventHandler(int currentBeatInSong, int currentBeatInMeasure, int currentMeasure);
    public event OnBeatEventHandler OnBeat;
    public delegate void OnHalfBeatEventHandler(int currentBeatInSong, int currentBeatInMeasure, int currentMeasure);
    public event OnHalfBeatEventHandler OnHalfBeat;
    public delegate void OnMeasureEventHandler(int currentMeasure);
    public event OnMeasureEventHandler OnMeasure;

    [SerializeField]
    private Song _song;

    [SerializeField]
    private AudioSource audioSource;

    public Song song {  
        get => _song;
        set => PlaySong(value);
    }

    private int _currentMeasure = 0;
    private int _currentBeatInMeasure = 1;
    private int _currentBeatInSong = 0;
    
    private int _beatUntilStart = 0;

    public int currentMeasure => _currentMeasure;
    public int currentBeatInMeasure => _currentBeatInMeasure;
    public int currentBeatInSong => _currentBeatInSong;
    
    public int beatUntilStart => _beatUntilStart;

    private double _songPosition;
    private double _songPositionDelta;
    public double songPosition => _songPosition;

    private bool _timerBeforePlaying;
    private double _timer;

    private bool _playing;
    public bool playing => _playing;

    private bool _invokedHalfBeat;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        PlaySong(song);
        
    }

    // Update is called once per frame
    private void Update()
    {
        _timer += Time.deltaTime;

        if (_beatUntilStart > 0)
        {
            int positionInBeats = (int)Math.Floor(_timer / song.spb);

            if (!_invokedHalfBeat && ((_timer % song.spb) > song.spb / 2))
            {
                _invokedHalfBeat = true;
                OnHalfBeat?.Invoke(-beatUntilStart, -1, -1);
            }

            if (positionInBeats > song.beatsBeforeStart - _beatUntilStart)
            {
                _invokedHalfBeat = false;
                _beatUntilStart--;
                OnBeat?.Invoke(-beatUntilStart, -1, -1);

                if (_beatUntilStart == 0)
                {
                    _timer = 0;
                    _playing = true;
                    audioSource.Play();
                }
            }
        }
        else if (_playing)
        {
            if (!audioSource.isPlaying)
            {
                StopSong();
                return;
            }

            CalculateSongPosition();

            UpdateAudioTime?.Invoke(songPosition, _songPositionDelta);

            int positionInBeats = (int)Math.Floor(_songPosition / song.spb);

            if (!_invokedHalfBeat && ((_songPosition % song.spb) > song.spb / 2))
            {
                OnHalfBeatHandler();
            }

            if (positionInBeats > _currentBeatInSong - song.beatsBeforeStart)
            {
                OnBeatHandler();
            }
        }
    }

    public void PlaySong(Song song)
	{
        StopSong();
        
        _song = song;
        audioSource.clip = song.audioStream;
        
        if (song.beatsBeforeStart > 0)
        {
            _timerBeforePlaying = true;
            _beatUntilStart = song.beatsBeforeStart;
            
        } else
        {
            audioSource.Play();
            _playing = true;
        }
    }

    public void StopSong()
    {
        audioSource.Stop();
        _playing = false;
        _currentMeasure = 0;
        _currentBeatInMeasure = 1;
        _currentBeatInSong = 0;
        _songPosition = 0;
        _songPositionDelta = 0;
        _timer = 0;
        _beatUntilStart = 0;
    }
    
    private void OnHalfBeatHandler()
    {
        _invokedHalfBeat = true;
        OnHalfBeat?.Invoke(currentBeatInSong, currentBeatInMeasure, currentMeasure);
    }

    private void OnBeatHandler()
	{
        
		_currentBeatInMeasure++;
        _currentBeatInSong++;
        _invokedHalfBeat = false;


        if (_currentBeatInMeasure > _song.beatPerMeasure)
		{
			_currentBeatInMeasure = 1;
			_currentMeasure++;
            OnMeasure?.Invoke(currentMeasure);
		}

        OnBeat?.Invoke(currentBeatInSong, currentBeatInMeasure, currentMeasure);
    }

	private void CalculateSongPosition()
	{
        double newSongPosition = audioSource.timeSamples / (double)audioSource.clip.frequency;

        if (_songPosition > newSongPosition)
        {
            newSongPosition = _songPosition;
        }

        _songPositionDelta = newSongPosition - _songPosition;

        _songPosition = newSongPosition;
    }
    
}