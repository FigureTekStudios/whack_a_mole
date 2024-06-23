using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] GameObject moleHolePrefab;
    [SerializeField] List<GameObject> moleHoles = new List<GameObject>(); 

    [SerializeField] int maxMoleHoles = 10; 
    private int currentMoleHoles = 0;    

    public float areaSizeX = 0; 
    public float areaSizeZ = 0;
    public Transform rayOrigin; // Transform of the invisible object


    [SerializeField] bool areaDetectionEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    void Update()
    {
        if (areaDetectionEnabled)
            AreaDetection();    
    }

    private void Init()
    {

    }

    private void GenerateBoard()
    {

    }

    // Checks surface area of game board for empty places to spawn mole hole at.
    private void AreaDetection()
    {
        // Check if yOriginTransform is assigned
        if (rayOrigin == null)
        {
            Debug.LogError("rayOrigin is not assigned.");
            return;
        }

        // Generate random x and z positions within the defined area
        float randomX = Random.Range(rayOrigin.position.x - areaSizeX / 2, rayOrigin.position.x + areaSizeX / 2);
        float randomZ = Random.Range(rayOrigin.position.z - areaSizeZ / 2, rayOrigin.position.z + areaSizeZ / 2);

        // Create the random position vector
        Vector3 randomPosition = new Vector3(randomX, rayOrigin.position.y, randomZ);

        // Shoot a ray from the random position downwards
        Ray ray = new Ray(randomPosition, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Ray hit: " + hit.collider.name + " at position: " + hit.point);
            if (hit.transform.tag == "MoleHole")
            {
                Debug.Log($"hitting moleHole: {hit.transform.name}");
            }

            else if (hit.transform.tag == "GameBoard")
            {
                Debug.Log($"hitting gameboard!");
            }
        }
        else
        {
            Debug.Log("Ray did not hit anything.");
        }

        // Optional: Draw the ray in the scene view for debugging
        Debug.DrawRay(randomPosition, Vector3.down, Color.red, 1f);
    }

    private void SpawnHole(Vector3 targetPos) 
    {
        
    }

    private void DeleteAllMoleHoles()
    {

    }

}
