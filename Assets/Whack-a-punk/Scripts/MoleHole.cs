using System;
using System.Collections;
using UnityEngine;

public class MoleHole : MonoBehaviour
{
    private enum MoleState
    {
        idle, hit, revealing, retreating, hiding
    }

    private MoleState state = MoleState.hiding;

    // score that gets returned to game manager
    public int score = 10;

    private Animator animator;
    public AudioClip idleAudioClip, hitAudioClip, revealAudioClip, retreatAudioClip;
    public ParticleSystem hitParticle, revealParticle, retreatParticle;

    public Action<int> OnMoleHit;

    private void Awake()
    {
       animator = GetComponentInChildren<Animator>(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Mallet")
            Hit();
    }

    private void StateManager(MoleState state)
    {
        switch (state)
        {
            case MoleState.idle:
                break;
            case MoleState.hit:
                break; 
            case MoleState.revealing:
                break;
            case MoleState.retreating:
                break;
            case MoleState.hiding:
                break;
            default:
                break;
        }
    }

    public void Hit()
    {
        if (state == MoleState.idle || state == MoleState.revealing)
        {
            //hitAnimClip.play();
            SoundManager.Instance.PlaySound(hitAudioClip);
            OnMoleHit?.Invoke(score);
            StateManager(MoleState.retreating);
        }
    }

    public IEnumerator RevealMole()
    {
        Debug.Log("Step into revealmole()");
        animator.SetTrigger("Reveal");
        //StateManager(MoleState.revealing);
        yield return null;
    }

    public IEnumerator RetreatMole(bool hit = false)
    {
        animator.SetTrigger("Retreat");
        StateManager(MoleState.retreating);
        yield return null;
        animator.SetTrigger("Hide");
    }
}
