using System.Collections.Generic;
using UnityEngine;

public class AccessoryRandomizer : MonoBehaviour
{
    public List<GameObject> accessoryGroups; // List of accessory group GameObjects that end with "_AccessoryGroup"
    public List<InterchangeableGroup> interchangeableGroups; // List of lists for interchangeable groups
    public List<GameObject> randomChanceGroups; // List of accessory groups with random chance of visibility
    public List<DependentAccessory> dependentAccessories; // List of dependent accessories

    // Method to randomize accessory visibility
    public void RandomizeAccessories()
    {
        // Handle accessory groups
        foreach (GameObject group in accessoryGroups)
        {
            bool showGroup = Random.value > 0.5f;
            SetGroupVisibility(group, showGroup);
        }

        // Handle interchangeable groups
        foreach (InterchangeableGroup groupList in interchangeableGroups)
        {
            int randomIndex = Random.Range(0, groupList.group.Count);
            for (int i = 0; i < groupList.group.Count; i++)
            {
                bool isVisible = i == randomIndex;
                SetGroupVisibilityOrMesh(groupList.group[i], isVisible);
            }
        }

        // Handle random chance groups
        foreach (GameObject group in randomChanceGroups)
        {
            bool showGroup = Random.value > 0.5f;
            SetGroupVisibility(group, showGroup);
        }

        // Handle dependent accessories
        foreach (DependentAccessory dependent in dependentAccessories)
        {
            bool isVisible = Random.value > 0.5f;
            SetGroupVisibilityWithDependency(dependent.accessory, isVisible, dependent.dependency);
        }
    }

    // Method to set visibility of a group and its children with SkinnedMeshRenderer
    private void SetGroupVisibility(GameObject group, bool isVisible)
    {
        SkinnedMeshRenderer[] renderers = group.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            renderer.enabled = isVisible;
        }
    }

    // Method to set visibility of a single mesh or a group
    private void SetGroupVisibilityOrMesh(GameObject obj, bool isVisible)
    {
        SkinnedMeshRenderer renderer = obj.GetComponent<SkinnedMeshRenderer>();
        if (renderer != null)
        {
            renderer.enabled = isVisible;
        }
        else
        {
            SetGroupVisibility(obj, isVisible);
        }
    }

    // Method to set visibility with dependency check
    private void SetGroupVisibilityWithDependency(GameObject accessory, bool isVisible, GameObject dependency)
    {
        if (isVisible)
        {
            SkinnedMeshRenderer dependencyRenderer = dependency.GetComponent<SkinnedMeshRenderer>();
            if (dependencyRenderer != null && dependencyRenderer.enabled)
            {
                SetGroupVisibilityOrMesh(accessory, true);
            }
            else
            {
                SetGroupVisibilityOrMesh(accessory, false);
            }
        }
        else
        {
            SetGroupVisibilityOrMesh(accessory, false);
        }
    }
}
