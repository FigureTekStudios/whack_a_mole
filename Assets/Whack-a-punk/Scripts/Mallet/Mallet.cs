using UnityEngine;

public class Mallet : MonoBehaviour
{
    
    [SerializeField]
    private float necessarySpeed = 0.5f;
    
    [SerializeField]
    private float necessaryHeight = 0.5f;
    
    [SerializeField]
    private GameObject malletCenter;
    
    private float _lastHighPosition ;
    private float _lastHighPositionTime;

    private void Start()
    {
        _lastHighPosition = malletCenter.transform.position.y;
        _lastHighPositionTime = Time.time;
    }
    
    private void Update()
    {
        if (malletCenter.transform.position.y > _lastHighPosition)
        {
            _lastHighPosition = malletCenter.transform.position.y;
            _lastHighPositionTime = Time.time;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IHittable hittable = other.GetComponentInParent<IHittable>(); // look at parent mole hole for this interface

        if (hittable == null) { other.GetComponent<IHittable>(); } 
        
        Debug.Log("collided with " + other.name);
        
        if (hittable == null) return;
        
        Debug.Log("Found hittable " + other.name);

        bool hitWithForce = _lastHighPosition < other.bounds.max.y + necessaryHeight
                            || _lastHighPositionTime + necessarySpeed < Time.time;
            
        _lastHighPosition = transform.position.y;
        _lastHighPositionTime = Time.time;
        
        if (hitWithForce)
        {
            Debug.Log("Not enough force to hit " + other.name);
            return;
        }
        
        hittable.Hit();
    }
}
