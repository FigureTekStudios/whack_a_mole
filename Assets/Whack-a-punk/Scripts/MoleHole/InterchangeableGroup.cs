using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DependentAccessory
{
    public GameObject accessory; // The accessory to be randomized
    public GameObject dependency; // The dependency that needs to be visible
}

[System.Serializable]
public class InterchangeableGroup
{
    public List<GameObject> group;
}