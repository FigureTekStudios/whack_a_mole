using UnityEngine;

public class TestInputClicker : MonoBehaviour
{
    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Check if the left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            // Create a ray from the camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit))
            {
                // Log the name of the hit GameObject
                Debug.Log("Clicked on: " + hit.collider.gameObject.name);
                IHittable hittable = hit.collider.GetComponentInParent<IHittable>(); // look at parent mole hole for this interface
                                                               
                if (hittable == null) { hit.collider.GetComponent<IHittable>(); }
                
                // Check if the hit object has the IHittable to hit      
                if (hittable == null) return;

                audioSource.Play();
                hittable.Hit();
            }
        }
    }
}
