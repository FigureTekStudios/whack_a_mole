using UnityEngine;

public class HittableCube : MonoBehaviour, IHittable
{
    
    public void Hit()
    {
        Destroy(gameObject);
    }
}
