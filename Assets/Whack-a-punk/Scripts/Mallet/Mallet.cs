using UnityEngine;

public class Mallet : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        IHittable hittable = other.GetComponent<IHittable>();
        hittable?.Hit();
    }
}
