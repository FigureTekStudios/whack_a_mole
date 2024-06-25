
using UnityEngine;
using UnityEngine.XR.Content.Animation;

/// <summary>
/// Enables a component to react to the 'RetreatFinished' animation event.
/// </summary>
/// <seealso cref="IAnimationEventActionBegin"/>
public interface IMoleRetreatAnimationEventFinished
{
    void RetreatFinished(string label);
}

/// <summary>
/// Calls the 'ActionFinished' function on any supported component when the target animation exits.
/// </summary>
/// <seealso cref="AnimationEventActionBegin"/>
public class MoleRetreatAnimationEventFinished : StateMachineBehaviour
{
    [SerializeField]
    [Tooltip("A label identifying the animation that has finished.")]
    string m_Label;

    /// <inheritdoc />
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var eventReceiver = animator.GetComponentInParent<IMoleRetreatAnimationEventFinished>();
        eventReceiver?.RetreatFinished(m_Label);
    }
}