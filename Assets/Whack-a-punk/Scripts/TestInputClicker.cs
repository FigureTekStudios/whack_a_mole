using UnityEngine;

public class TestInputClicker : MonoBehaviour
{
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
                // Check if the hit object has the specific tag
                if (hit.collider.CompareTag("MoleHole"))
                {
                    // Log the name of the hit GameObject
                    Debug.Log("Clicked on: " + hit.collider.gameObject.name);
                    //hit.transform.GetComponentInParent<MoleHole>().Hit();
                    StartCoroutine(hit.transform.GetComponentInParent<MoleHole>().RevealMole());
                }
            }
        }
    }
}
