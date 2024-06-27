using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoleHole : MonoBehaviour, IHittable, IMoleRetreatAnimationEventFinished, IMoleRevealAnimationEventFinished
{
    private enum MoleState
    {
        idle, hit, revealing, retreating, hiding
    }

    private MoleState state = MoleState.hiding;
    
    // Likeliness in percent that the mole will reveal itself each beat
    public int revealLikelinessInPercent = 20;
    
    public int revealTimeInBeats = 16;
    private int currentTimeRevealedInBeats = 0;
    
    // speed multiplier for mole reveal time
    public int speed = 1; 
    // score that gets returned to game manager
    public int score = 10;
    
    private int _multiplier = 1;

    private Animator animator;
    public AudioClip idleAudioClip, hitAudioClip, revealAudioClip, retreatAudioClip;
    public ParticleSystem hitParticle, revealParticle, retreatParticle;

    public Action<int> OnMoleHit;

    private void Awake()
    {
       animator = GetComponentInChildren<Animator>(); 
       OnMoleHit += GameManager.Instance.AddScore;
       if (Conductor.Instance)
            Conductor.Instance.OnBeat += OnBeat;
    }

    private void Start()
    {
        StartCoroutine(RevealMole());
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

    private void OnBeat(int currentBeatInSong, int currentBeatInMeasure, int currentMeasure)
    {
        if (state == MoleState.hiding)
        {
            if (Random.Range(0, 100) > revealLikelinessInPercent) return;
            
            StartCoroutine(RevealMole());
        }

        if (state == MoleState.idle || state == MoleState.revealing)
        {
            currentTimeRevealedInBeats++;

            if (currentTimeRevealedInBeats >= revealTimeInBeats / speed)
            {
                StartCoroutine(RetreatMole());
            }
        }
    }

    public void Hit()
    {
        Debug.Log("hitting");
        if (state == MoleState.idle || state == MoleState.revealing)
        {
            //animator.SetTrigger("Hit"); // create anim for this
            SoundManager.Instance.PlaySound(hitAudioClip);
            OnMoleHit?.Invoke(score);
            StartCoroutine(RetreatMole(true));
        }
    }

    public IEnumerator RevealMole()
    {
        currentTimeRevealedInBeats = 0;
        Debug.Log("Step into revealmole()");
        animator.SetTrigger("Reveal");
        state = MoleState.revealing;
        //StateManager(MoleState.revealing);
        yield return null;
    }

    public IEnumerator RetreatMole(bool hit = false)
    {
        animator.SetTrigger("Retreat");
        StateManager(MoleState.retreating);
        state = MoleState.retreating;
        yield return null;
    }
    
    public IEnumerator Hide()
    {
        StateManager(MoleState.hiding);
        state = MoleState.hiding;
        yield return null;
    }
    
    public IEnumerator Idle()
    {
        StateManager(MoleState.idle);
        state = MoleState.idle;
        yield return null;
    }

    private void OnDestroy()
    {
        OnMoleHit -= GameManager.Instance.AddScore;
        if (Conductor.Instance)
            Conductor.Instance.OnBeat -= OnBeat;
    }

    public void RetreatFinished(string label)
    {
        StartCoroutine(Hide());
    }
    
    public void RevealFinished(string label)
    {
        StartCoroutine(Idle());
    }
}
