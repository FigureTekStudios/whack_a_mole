using UnityEngine;

public class CloseApertureOnExit : StateMachineBehaviour
{
    // This will be called when the state machine finishes evaluating this state.
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Find the MoleHole script in the parent and call the CloseAperture method.
        MoleHole moleHole = animator.GetComponentInParent<MoleHole>();
        if (moleHole != null)
        {
            moleHole.CloseAperture();
        }
    }
}
