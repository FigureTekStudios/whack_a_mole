using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoleHole : MonoBehaviour, IHittable, IMoleRetreatAnimationEventFinished, IMoleRevealAnimationEventFinished
{
    [SerializeField] MoleState state = MoleState.hiding;
    
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
    private string currentAnimTriggerName = string.Empty;
    private List<string> idleAnimTriggerNames = new List<string>();
    private List<string> tauntAnimTriggerNames = new List<string>();
    private List<string> retreatAnimTriggerNames = new List<string>();

    public Action<int, int> OnMoleHit;
    
    [SerializeField]
    private CircleTimer _circleTimer;

    [SerializeField] 
    private float perfectTimeInBeats;
    [SerializeField]
    private float okTimeInBeats;

    //public string CurrentAnimTriggerName { get => currentAnimTriggerName; }

    private void Awake()
    {
        //animator = GetComponentInChildren<Animator>(); 
        idleAnimTriggerNames.Add("Idle_0");
        idleAnimTriggerNames.Add("Idle_1");

        tauntAnimTriggerNames.Add("Taunt_0");
        tauntAnimTriggerNames.Add("Taunt_1");

        retreatAnimTriggerNames.Add("Retreat_0");
        retreatAnimTriggerNames.Add("Retreat_1");

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

    private string GetRandomAnimation(MoleState targetState)
    {
        string animName = "";
        List<string> triggerAnimNames = new List<string>();

        switch (targetState)
        {
            case MoleState.idle:
                triggerAnimNames = idleAnimTriggerNames;
                break;
            case MoleState.retreating:
                triggerAnimNames = retreatAnimTriggerNames;
                break;
            case MoleState.hiding:
                triggerAnimNames = retreatAnimTriggerNames;
                break;
            default:
                break;
        }
        if (triggerAnimNames == null || triggerAnimNames.Count == 0)
        {
            return null; // or handle this case as needed
        }
        Dictionary<string, int> animationWeights = triggerAnimNames.ToDictionary(anim => anim, anim => 1);
        
        var moleHoles = GameManager.Instance.MoleHoles;
        // Increase weights for animations that aren't currently playing
        foreach (GameObject moleHole in moleHoles)
        {
            if (moleHole.GetComponent<MoleHole>().state == targetState)
            {
                if (animationWeights.ContainsKey(moleHole.GetComponent<MoleHole>().currentAnimTriggerName))
                {
                    animationWeights[moleHole.GetComponent<MoleHole>().currentAnimTriggerName]++;
                }
            }
        }

        // Normalize weights (higher weight means lower probability)
        int totalWeight = animationWeights.Values.Sum();
        List<string> weightedList = new List<string>();
        foreach (var kvp in animationWeights)
        {
            int weight = totalWeight - kvp.Value + 1; // Add 1 to avoid zero probability
            for (int i = 0; i < weight; i++)
            {
                weightedList.Add(kvp.Key);
            }
        }

        // Select a random animation from the weighted list
        int randomIndex = Random.Range(0, weightedList.Count);
        
        return animName = weightedList[randomIndex];
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
        Debug.Log("Step into revealmole()");

        //StateManager(MoleState.revealing);
        state = MoleState.revealing;
        currentAnimTriggerName = "Reveal"; 
        animator.SetTrigger(currentAnimTriggerName);

        _circleTimer.SetTimeInBeats(revealTimeInBeats);
        
        currentTimeRevealedInBeats = 0;
           
        //yield return new WaitUntil(() => this.animator.GetCurrentAnimatorStateInfo(0).IsName("Reveal"));
        StartCoroutine(Idle());  
        yield return null;
    }

    public IEnumerator RetreatMole(bool hit = false)
    {
        //StateManager(MoleState.retreating);
        state = MoleState.retreating;
        if (hit)
            currentAnimTriggerName = "Retreat_Damaged";
        else
            currentAnimTriggerName = GetRandomAnimation(state);

        animator.SetTrigger(currentAnimTriggerName);

        _circleTimer.StopTimer();
        yield return new WaitUntil(() => this.animator.GetCurrentAnimatorStateInfo(0).IsName(currentAnimTriggerName));
        StartCoroutine(Hide());
        //yield return null;
    }
    
    public IEnumerator Hide()
    {
        StateManager(MoleState.hiding);
        state = MoleState.hiding;
        animator.SetTrigger("Hide");

        yield return null;
    }
    
    public IEnumerator Idle()
    {
        //StateManager(MoleState.idle);
        state = MoleState.idle;
        currentAnimTriggerName = GetRandomAnimation(state);
        animator.SetTrigger(currentAnimTriggerName);
        yield return null;
    }

    public IEnumerator Taunt()
    {
        //StateManager(MoleState.taunt);
        state = MoleState.taunt;
        currentAnimTriggerName = GetRandomAnimation(state);
        animator.SetTrigger(currentAnimTriggerName);
        yield return null;
    }


    public IEnumerator Shock()
    {
        //StateManager(MoleState.shocked);
        state = MoleState.shocked;
        currentAnimTriggerName = "Shock";
        animator.SetTrigger(currentAnimTriggerName);
        // Sound.Play();
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
    idle, hit, revealing, retreating, hiding, taunt, shocked
}