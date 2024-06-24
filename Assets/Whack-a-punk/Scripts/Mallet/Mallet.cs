using UnityEngine;

public class Mallet : MonoBehaviour
{
    
    [SerializeField]
    private float necessarySpeed = 0.5f;
    
    [SerializeField]
    private float necessaryHeight = 0.5f;
    
    [SerializeField]
    private GameObject malletCenter;
    
    [SerializeField]
    private Transform trackedTransform;
    [SerializeField]
    private Rigidbody rigidbody;

    private bool _hasReachedSufficientHeight;
    private float _lastTimeSufficientHeight;
    
    private void Update()
    {
        if (malletCenter.transform.position.y > necessaryHeight)
        {
            _hasReachedSufficientHeight = true;
            _lastTimeSufficientHeight = Time.time;
        }
    }
    
    private void FixedUpdate()
    {
        rigidbody.velocity = (trackedTransform.position - transform.position) / Time.fixedDeltaTime;
        
        Quaternion rotationDifference = trackedTransform.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);
        
        Vector3 rotationDifferenceInDegree = angleInDegree * rotationAxis;
        
        rigidbody.angularVelocity = rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        IHittable hittable = other.GetComponent<IHittable>();
        
        Debug.Log("collided with " + other.name);
        
        if (hittable == null) return;
        
        Debug.Log("Found hittable " + other.name);

        bool hitWithForce = _hasReachedSufficientHeight && _lastTimeSufficientHeight + necessarySpeed >= Time.time;
            
        _hasReachedSufficientHeight = false;
        
        if (hitWithForce)
        {
            Debug.Log("Not enough force to hit " + other.name);
            return;
        }
        
        hittable.Hit();
    }
}
