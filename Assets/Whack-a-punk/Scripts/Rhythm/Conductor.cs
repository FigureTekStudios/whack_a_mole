using System;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    public static Conductor Instance { get; private set; }
    
    public delegate void UpdateAudioTimeEventHandler(float audioTime, float audioTimeDelta);
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

    public int currentMeasure => _currentMeasure;
    public int currentBeatInMeasure => _currentBeatInMeasure;
    public int currentBeatInSong => _currentBeatInSong;

    private float _songPosition;
    private float _songPositionDelta;
    public float songPosition => _songPosition;

    private bool _playing;
    public bool playing => _playing;

    private bool _invokedHalfBeat;
    
    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    //private void Start()
    //{
    //    if (song != null) PlaySong(song);
    //}

    private void Update()
    {
        if (_playing)
        {
            if (GameManager.Instance.IsPaused)
                return;

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

            if (positionInBeats > _currentBeatInSong)
            {
                OnBeatHandler();
            }
        }
    }
    public void PlaySong()
    {
        if (song != null) PlaySong(song);
    }

    private void PlaySong(Song song)
	{
        StopSong();
        
        _song = song;
        audioSource.clip = song.audioStream;
        
        audioSource.Play();
        _playing = true;
        
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
    }
    
    public void PauseSong()
    {
        audioSource.Pause();
        _playing = false;
    }
    
    public void ResumeSong()
    {
        audioSource.UnPause();
        _playing = true;
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
        float newSongPosition = (float)(audioSource.timeSamples / (double)audioSource.clip.frequency);
        
        if (_songPosition > newSongPosition)
        {
            newSongPosition = _songPosition;
        }

        _songPositionDelta = newSongPosition - _songPosition;

        _songPosition = newSongPosition;
    }
    
}