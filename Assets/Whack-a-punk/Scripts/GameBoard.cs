using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] GameObject molePrefab;    

    public int areaSizeX = 0; 
    public int areaSizeZ = 0;
    public Transform yOriginTransform; // Transform of the invisible object

    // Start is called before the first frame update
    void Start()
    {
        
    }


    void Update()
    {
        // Check if yOriginTransform is assigned
        if (yOriginTransform == null)
        {
            Debug.LogError("yOriginTransform is not assigned.");
            return;
        }

        // Generate random x and z positions within the defined area
        float randomX = Random.Range(-areaSizeX / 2, areaSizeX / 2);
        float randomZ = Random.Range(-areaSizeZ / 2, areaSizeZ / 2);

        // Create the random position vector
        Vector3 randomPosition = new Vector3(randomX, yOriginTransform.position.y, randomZ);

        // Shoot a ray from the random position downwards
        Ray ray = new Ray(randomPosition, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Ray hit: " + hit.collider.name + " at position: " + hit.point);
        }
        else
        {
            Debug.Log("Ray did not hit anything.");
        }

        // Optional: Draw the ray in the scene view for debugging
        Debug.DrawRay(randomPosition, Vector3.down * 100f, Color.red, 1f);
    }
    void Init()
    {

    }

    void GenerateBoard()
    {

    }

    void SpawnHole(Vector3 targetPos) 
    {
        
    }

    void DeleteAllMoleHoles()
    {

    }

}
