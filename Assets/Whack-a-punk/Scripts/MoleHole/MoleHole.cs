using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoleHole : MonoBehaviour, IHittable, IMoleRetreatAnimationEventFinished, IMoleRevealAnimationEventFinished
{
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

    [SerializeField] Animator animator;
    public AudioClip idleAudioClip, hitAudioClip, revealAudioClip, retreatAudioClip;
    public ParticleSystem hitParticle, revealParticle, retreatParticle;

    public Action<int, int> OnMoleHit;
    
    [SerializeField]
    private CircleTimer _circleTimer;

    [SerializeField] 
    private float perfectTimeInBeats;
    [SerializeField]
    private float okTimeInBeats;

    private void Awake()
    {
       //animator = GetComponentInChildren<Animator>(); 
       OnMoleHit += GameManager.Instance.AddScore;
       if (Conductor.Instance)
            Conductor.Instance.OnBeat += OnBeat;
    }

    private void Start()
    {
        StartCoroutine(RetreatMole());
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
        if (!GameManager.Instance.GameStarted || GameManager.Instance.GameEnded) return;
        if (state == MoleState.idle || state == MoleState.revealing)
        {
            currentTimeRevealedInBeats++;

            if (currentTimeRevealedInBeats >= revealTimeInBeats / speed)
            {
                StartCoroutine(RetreatMole());
            }
        }
        
        if (state == MoleState.hiding)
        {
            if (Random.Range(0, 100) > revealLikelinessInPercent) return;
            
            StartCoroutine(RevealMole());
        }
    }

    public void Hit()
    {
        Debug.Log("hitting");
        if (GameManager.Instance.IsPaused)
            return;
        if (state == MoleState.idle || state == MoleState.revealing)
        {
            _circleTimer.StopTimer();
            
            _multiplier = currentTimeRevealedInBeats >= perfectTimeInBeats ? 3 : currentTimeRevealedInBeats >= okTimeInBeats ? 2 : 1;
            SoundManager.Instance.PlaySound(hitAudioClip);
            OnMoleHit?.Invoke(score, _multiplier);
            //animator.SetTrigger("Hit"); // create anim for this

            StartCoroutine(RetreatMole(true));
        }
    }

    public IEnumerator RevealMole()
    {
        _circleTimer.SetTimeInBeats(revealTimeInBeats);
        
        currentTimeRevealedInBeats = 0;
        
        Debug.Log("Step into revealmole()");
        animator.SetTrigger("Reveal");
        state = MoleState.revealing;
        //StateMa   nager(MoleState.revealing);
        //yield return new WaitUntil(() => this.animator.GetCurrentAnimatorStateInfo(0).IsName("Reveal"));

        yield return null;
    }

    public IEnumerator RetreatMole(bool hit = false)
    {
        if (hit)
            animator.SetTrigger("Retreat_Damaged");
        else
            animator.SetTrigger("Retreat_0");
        StateManager(MoleState.retreating);
        state = MoleState.retreating;
        _circleTimer.StopTimer();
        yield return new WaitUntil(() => this.animator.GetCurrentAnimatorStateInfo(0).IsName("Retreat_0"));
        StartCoroutine(Hide());
        //yield return null;
    }
    
    public IEnumerator Hide()
    {
        animator.SetTrigger("Hide");
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


public enum MoleState
{
    idle, hit, revealing, retreating, hiding
}