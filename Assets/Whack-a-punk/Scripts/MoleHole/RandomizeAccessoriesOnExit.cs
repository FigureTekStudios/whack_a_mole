using UnityEngine;

public class RandomizeAccessoriesOnExit : StateMachineBehaviour
{
    public RandomMaterialAssigner materialAssigner;

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AccessoryRandomizer accessoryRandomizer = animator.GetComponentInParent<AccessoryRandomizer>();
        if (accessoryRandomizer != null)
        {
            accessoryRandomizer.RandomizeAccessories();
        }

        // Call AssignRandomMaterials after accessories are randomized
        if (materialAssigner != null)
        {
            materialAssigner.AssignRandomMaterials();
        }
    }
}