using UnityEngine;

public class Song : MonoBehaviour
{
    [SerializeField]
    private AudioClip _audioStream;
    public AudioClip audioStream => _audioStream;

    [SerializeField]
    private int _bpm = 120;
    public int bpm => _bpm;

    public double spb => (60 / (double)_bpm);

    [SerializeField]
    private int _beatPerMeasure = 4;
    public int beatPerMeasure => _beatPerMeasure;

}