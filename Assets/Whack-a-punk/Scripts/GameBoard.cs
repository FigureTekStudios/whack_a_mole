using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] GameObject moleHolePrefab;
    private List<GameObject> moleHoles = new List<GameObject>(); 

    [SerializeField] int maxMoleHoles = 10;
    private int moleHoleCount = 0;    

    // this should match the surface size of the game board mesh
    public float areaSizeX = 0; 
    public float areaSizeZ = 0;

    private Transform rayOrigin; // Transform of the invisible object

    private bool areaDetectionEnabled = false;
    public float sphereRadius = 1f;
    private bool drawSphere = false;
    private Vector3 lastSpherePosition;

    private float edgeBuffer = .25f;

    private Transform moleHolesParent;

    private void Start()
    {
        Init();
    }

    void Update()
    {
        if (areaDetectionEnabled)
            AreaDetection();

        if (Input.GetKeyDown(KeyCode.D))
            DeleteAllMoleHoles();

        if (Input.GetKeyDown(KeyCode.S)) 
            StartCoroutine(GenerateGameBoard());  
    }

    private void Init()
    {
        moleHolesParent = GameObject.Find("MoleHolesParent").transform;
        rayOrigin = GameObject.Find("SurfaceCheckOrigin").transform;
    }

    private IEnumerator GenerateGameBoard()
    {
        Debug.Log("Generating game board...");

        areaDetectionEnabled = true;    
        yield return new WaitUntil(() => moleHoleCount == maxMoleHoles);
        areaDetectionEnabled = false;

        Debug.Log("Game board generated!");
    }

    // Checks surface area of game board for empty places to spawn mole hole at.
    private void AreaDetection()
    {
        // Check if rayOrigin object is assigned
        if (rayOrigin == null)
        {
            Debug.LogError("rayOrigin is not assigned.");
            areaDetectionEnabled = false;
            return;
        }

        if (moleHoleCount == maxMoleHoles)
        {
            Debug.Log("moleHoleCount reached maxed count");
            areaDetectionEnabled = false;
            return;
        }


        // Calculate the effective area size considering the edge buffer
        float effectiveAreaSizeX = areaSizeX - 2 * edgeBuffer;
        float effectiveAreaSizeZ = areaSizeZ - 2 * edgeBuffer;

        // Generate random x and z positions within the defined area
        float randomX = Random.Range(rayOrigin.position.x - effectiveAreaSizeX / 2, rayOrigin.position.x + effectiveAreaSizeX / 2);
        float randomZ = Random.Range(rayOrigin.position.z - effectiveAreaSizeZ / 2, rayOrigin.position.z + effectiveAreaSizeZ / 2);

        // Create the random position vector
        Vector3 randomPosition = new Vector3(randomX, rayOrigin.position.y, randomZ);

        // Shoot a ray from the random position downwards
        Ray ray = new Ray(randomPosition, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Store the last sphere position for drawing
            lastSpherePosition = hit.point;
            drawSphere = true;

            // Perform a sphere check at the hit point
            Collider[] hitColliders = Physics.OverlapSphere(hit.point, sphereRadius);
            bool canSpawn = true;

            foreach (var hitCollider in hitColliders)
            {
                //Debug.Log("Sphere hit: " + hitCollider.name + " at position: " + hit.point);

                if (hitCollider.CompareTag("MoleHole"))
                {
                    Debug.Log($"hitting moleHole: {hitCollider.transform.name}");
                    canSpawn = false;
                    break;
                }
            }

            if (canSpawn && moleHoleCount <= maxMoleHoles)
            {
                // Calculate the rotation based on the hit normal
                Quaternion hitRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                Debug.Log($"{hit.transform.name} hit rotation is: {hitRotation}");
                SpawnHole(hit.point, hitRotation);

            }
        }
        else
        {
            Debug.Log("Ray did not hit anything.");
            drawSphere = false;
        }

#if UNITY_EDITOR
        // Optional: Draw the ray in the scene view for debugging
        Debug.DrawRay(randomPosition, Vector3.down, Color.red, 1f);
#endif

    }

    private void SpawnHole(Vector3 targetPos, Quaternion targetRot) 
    {
        var moleHole = Instantiate(moleHolePrefab, targetPos, transform.rotation);

        Vector3 adjustment = new Vector3(0, moleHole.GetComponentInChildren<CapsuleCollider>().bounds.extents.y, 0);
        moleHole.transform.position = new Vector3( targetPos.x + adjustment.x, targetPos.y, targetPos.z + adjustment.z) ;   
        moleHole.transform.parent = moleHolesParent;
        
        moleHoles.Add(moleHole);    
        moleHoleCount++;
    }

    private void DeleteAllMoleHoles()
    {
        if (moleHoleCount == 0 || moleHoles.Count == 0)
        {
            Debug.Log("Can't delete mole holes! no enabled mole holes to delete.");
            return;
        }

        foreach (var moleHole in moleHoles)
            Destroy(moleHole);

        moleHoles.Clear();
        moleHoleCount = 0;  
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (drawSphere)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(lastSpherePosition, sphereRadius);
        }
    }
#endif
}
