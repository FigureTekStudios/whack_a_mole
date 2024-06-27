using UnityEngine;

public class NeonShader : MonoBehaviour
{
    
    [SerializeField]
    private Color[] colors;
    
    private Material _material;
    
    private int _currentColorIndex = 0;
    
    private void Start()
    {
        Conductor.Instance.OnBeat += OnBeat;
        _material = GetComponent<MeshRenderer>().material;
    }
    
    private void OnDestroy()
    {
        Conductor.Instance.OnBeat += OnBeat;
    }
    
    private void OnBeat(int currentBeatInSong, int currentBeatInMeasure, int currentMeasure)
    {
        _currentColorIndex++;
        _currentColorIndex %= colors.Length;
        
        _material.SetColor("_Color", colors[_currentColorIndex]);
    }
}
