using UnityEngine;

public class Song : MonoBehaviour
{
    [SerializeField]
    private AudioClip _audioStream;
    public AudioClip audioStream => _audioStream;

    [SerializeField]
    private int _bpm = 120;
    public int bpm => _bpm;

    private double _spb;
    public double spb => _spb;

    [SerializeField]
    private int _beatPerMeasure = 4;
    public int beatPerMeasure => _beatPerMeasure;

    public int beatsBeforeStart;

    private void Start()
    {
        _spb = 60 / (double)_bpm;
    }
}