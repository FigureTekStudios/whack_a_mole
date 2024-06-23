using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleHole : MonoBehaviour
{
    private enum MoleState
    {
        idle, hit, revealing, retreating
    }
    private MoleState moleState;

    // score that gets returned to game manager
    public int score = 10;

    public AnimationClip idleClip, hitClip, revealClip, retreatClip;

    public Action<int> OnMoleHit;

    private void Init()
    {
       
    }

    private void Hit()
    {
        OnMoleHit?.Invoke(score);   
    }

    private void RevealMole()
    { 
    }
    
    private void RetreatMole()
    {

    }
}
