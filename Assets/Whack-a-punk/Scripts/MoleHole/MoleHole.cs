using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MoleState
{
    idle, hit, revealing, retreating, hiding, taunt, shocked
}

public class MoleHole : MonoBehaviour, IHittable, IMoleRetreatAnimationEventFinished, IMoleRevealAnimationEventFinished
{
    [SerializeField] MoleState state = MoleState.hiding;
    private bool _hit = false;
    private int shockDuration = 3;
    public float minTimeToTaunt = 5f; // Minimum time before a mole can taunt
    public float maxTimeToTaunt = 15f; // Maximum time before a mole can taunt
    public static int maxMolesInTauntState = 3; // Maximum number of moles allowed to taunt at the same time
    private static int currentTauntingMoles = 0;
    private Coroutine tauntCoroutine;

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
    [SerializeField] Animator apertureAnimator; // Assign this in the inspector to the aperture's Animator

    private string currentAnimTriggerName = string.Empty;
    private List<string> idleAnimTriggerNames = new List<string>();
    private List<string> tauntAnimTriggerNames = new List<string>();
    private List<string> retreatAnimTriggerNames = new List<string>();

    [SerializeField] AudioSource audioSource;
    private AudioClip idleAudioClip, hitAudioClip, revealAudioClip, retreatAudioClip;
    public ParticleSystem hitParticle, revealParticle, retreatParticle;

    public Action<int, int> OnMoleHit;

    [SerializeField]
    private CircleTimer _circleTimer;

    [SerializeField]
    private float perfectTimeInBeats;
    [SerializeField]
    private float okTimeInBeats;

    public MoleState State { get => state; }

    private void Awake()
    {
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
        StateManager(MoleState.hiding);
    }

    private void StateManager(MoleState newState)
    {
        Debug.Log("StateManager: " + newState);
        state = newState;
        switch (newState)
        {
            case MoleState.idle:
                StartCoroutine(Idle());
                break;
            case MoleState.hit:
                Hit();
                break;
            case MoleState.revealing:
                if (!GameManager.Instance.PowerUpEnabled)
                    StartCoroutine(RevealMole());
                break;
            case MoleState.retreating:
                StartCoroutine(RetreatMole(_hit));
                break;
            case MoleState.hiding:
                StartCoroutine(Hide());
                break;
            case MoleState.taunt:
                if (!GameManager.Instance.PowerUpEnabled)
                    StartCoroutine(Taunt());
                break;
            case MoleState.shocked:
                StartCoroutine(Shock());
                break;
            default:
                break;
        }
    }

    public IEnumerator Idle()
    {
        Debug.Log("Entering Idle State");
        currentAnimTriggerName = GetRandomAnimation(MoleState.idle);
        StartCoroutine(SoundManager.Instance.PlayZombieIdleSFX(audioSource));
        animator.SetTrigger(currentAnimTriggerName);
        yield return new WaitForSeconds(Random.Range(2f, 5f)); // Stay idle for a random duration

        if (state == MoleState.idle)
        {
            // Only transition to taunt or retreat if still in idle state
            if (currentTauntingMoles < maxMolesInTauntState && Random.Range(0, 100) < revealLikelinessInPercent)
            {
                StateManager(MoleState.taunt);
            }
            else
            {
                StateManager(MoleState.retreating);
            }
        }
    }

    public IEnumerator Hide()
    {
        Debug.Log("Entering Hiding State");
        state = MoleState.hiding;
        currentAnimTriggerName = "Hide";
        animator.SetTrigger(currentAnimTriggerName);
        _hit = false;
        CloseAperture();
        yield return new WaitForSeconds(1f);
        if (!GameManager.Instance.GameEnded)
        {
            if (Random.Range(0, 100) <= revealLikelinessInPercent)
            {
                StateManager(MoleState.revealing);
            }
        }
    }

    public void Hit()
    {
        Debug.Log("Hitting");
        if (GameManager.Instance.IsPaused)
            return;
        if (state == MoleState.idle || state == MoleState.revealing || state == MoleState.taunt || state == MoleState.shocked)
        {
            _hit = true;
            _circleTimer.StopTimer();

            _multiplier = currentTimeRevealedInBeats >= perfectTimeInBeats ? 3 : currentTimeRevealedInBeats >= okTimeInBeats ? 2 : 1;
            SoundManager.Instance.PlaySound(hitAudioClip);
            SoundManager.Instance.PlayOnAddScoreSFX(audioSource, _multiplier);
            SoundManager.Instance.PlayZombieHitSFX(audioSource);
            OnMoleHit?.Invoke(score, _multiplier);

            // set mole to retreat.
            StateManager(MoleState.retreating);
        }
    }

    public IEnumerator RevealMole()
    {
        Debug.Log("Revealing Mole");
        state = MoleState.revealing;
        currentAnimTriggerName = "Reveal";
        animator.SetTrigger(currentAnimTriggerName);
        OpenAperture();
        _hit = false;

        _circleTimer.SetTimeInBeats(revealTimeInBeats);
        currentTimeRevealedInBeats = 0;
        StartCoroutine(SoundManager.Instance.PlayZombieRevealSFX(audioSource));
        StateManager(MoleState.idle);
        yield return null;
    }

    public IEnumerator RetreatMole(bool hit = false)
    {
        Debug.Log("Retreating Mole");
        state = MoleState.retreating;
        if (hit)
        {
            // Randomly choose between Retreat_1 and Retreat_Damaged if hit
            currentAnimTriggerName = Random.value > 0.5f ? "Retreat_1" : "Retreat_Damaged";
        }
        else
        {
            currentAnimTriggerName = "Retreat_0";
        }

        animator.SetTrigger(currentAnimTriggerName);
        StartCoroutine(SoundManager.Instance.PlayZombieRetreatSFX(audioSource));

        _circleTimer.StopTimer();

        // CloseAperture() will be called by the CloseApertureOnExit StateMachineBehaviour
        yield return new WaitForSeconds(1f); // Wait for retreat animation to finish
        StateManager(MoleState.hiding);
    }

    public IEnumerator Taunt()
    {
        Debug.Log("Entering Taunt State!");
        state = MoleState.taunt;
        currentAnimTriggerName = GetRandomAnimation(state);
        animator.SetTrigger(currentAnimTriggerName);
        SoundManager.Instance.PlayZombieTauntSFX(audioSource);

        _circleTimer.SetTimeInBeats(revealTimeInBeats);
        currentTimeRevealedInBeats = 0;

        currentTauntingMoles++;

        yield return new WaitForSeconds(3f); // Taunt duration
        currentTauntingMoles--;

        StateManager(MoleState.idle);
    }

    public IEnumerator Shock()
    {
        Debug.Log("Shocking Mole");
        StopAllCoroutines();
        _hit = false; // just in case a hit is already detected for some reason
        state = MoleState.shocked;
        currentAnimTriggerName = "Shock";
        animator.SetTrigger(currentAnimTriggerName);

        _circleTimer.StopTimer();
        yield return new WaitUntil(() => GameManager.Instance.PowerUpEnabled == false);

        StateManager(MoleState.retreating);
    }

    private void ScheduleTaunt()
    {
        float timeToTaunt = Random.Range(minTimeToTaunt, maxTimeToTaunt);
        tauntCoroutine = StartCoroutine(TauntAfterDelay(timeToTaunt));
    }

    private IEnumerator TauntAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Ensure the taunt only triggers if the mole is already up (revealing or idle)
        if ((state == MoleState.revealing || state == MoleState.idle) && currentTauntingMoles < maxMolesInTauntState)
        {
            StateManager(MoleState.taunt);
        }
        else
        {
            ScheduleTaunt(); // Reschedule taunt if conditions aren't met
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

        if (state == MoleState.hiding && GameManager.Instance.PowerUpEnabled == false)
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
            case MoleState.taunt:
                triggerAnimNames = tauntAnimTriggerNames;
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

    public void RetreatFinished(string label)
    {
        StateManager(MoleState.hiding);
    }

    public void RevealFinished(string label)
    {
        StateManager(MoleState.idle);
    }

    private void OnDestroy()
    {
        OnMoleHit -= GameManager.Instance.AddScore;
        if (Conductor.Instance)
            Conductor.Instance.OnBeat -= OnBeat;
    }

    // Methods to control the aperture
    public void OpenAperture()
    {
        Debug.Log("Opening Aperture");
        if (apertureAnimator != null)
        {
            apertureAnimator.SetTrigger("OpenTrigger");
        }
    }

    public void CloseAperture()
    {
        Debug.Log("Closing Aperture");
        if (apertureAnimator != null)
        {
            apertureAnimator.SetTrigger("CloseTrigger");
        }
    }
}
