using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
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

    public AnimationClip idleAnimClip, hitAnimClip, revealAnimClip, retreatAnimClip;
    public AudioClip idleAudioClip, hitAudioClip, revealAudioClip, retreatAudioClip;
    public ParticleSystem hitParticle, revealParticle, retreatParticle;

    public Action<int> OnMoleHit;

    private void Init()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Mallet")
            Hit();

    }

    private void Hit()
    {
        OnMoleHit?.Invoke(score);   
    }

    private IEnumerator RevealMole()
    { 
        yield return null;
    }
    
    private IEnumerator RetreatMole()
    {
        yield return null;
    }

}
