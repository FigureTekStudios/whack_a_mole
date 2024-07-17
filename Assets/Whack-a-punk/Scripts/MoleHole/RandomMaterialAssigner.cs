using System.Collections.Generic;
using UnityEngine;

public class RandomMaterialAssigner : MonoBehaviour
{
    [System.Serializable]
    public class MaterialGroup
    {
        public List<GameObject> targetObjects;
        public List<Material> possibleMaterials;
        public List<Color> multiplyColors;
    }

    public List<MaterialGroup> materialGroups;

    void Start()
    {
        AssignRandomMaterials();
    }

    public void AssignRandomMaterials()
    {
        foreach (MaterialGroup group in materialGroups)
        {
            if (group.targetObjects.Count > 0 && group.possibleMaterials.Count > 0)
            {
                Material randomMaterial = group.possibleMaterials[Random.Range(0, group.possibleMaterials.Count)];
                ApplyMaterialToGroup(group.targetObjects, randomMaterial, group.multiplyColors);
            }
        }
    }

    void ApplyMaterialToGroup(List<GameObject> objects, Material material, List<Color> colors)
    {
        Color randomColor = colors.Count > 0 ? colors[Random.Range(0, colors.Count)] : Color.white;

        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;

            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(material);
                renderer.material.SetColor("_Color", randomColor);
            }
            else
            {
                Debug.LogWarning($"No Renderer found on {obj.name}");
            }
        }
    }
}
