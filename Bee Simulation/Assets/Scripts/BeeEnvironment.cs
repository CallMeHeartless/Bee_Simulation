using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeEnvironment : MonoBehaviour
{
    // Internal references
    public Hive hive;
    public GameObject flowerPrefab;
    public List<GameObject> flowers = new List<GameObject>();

    [Header("Flower Variables")]
    public int flowerCount = 3; // The number of flowers that should be in the scene
    public float flowerSpawnMinRadius = 10.0f;
    public float flowerSpawnMaxRadius = 20.0f;
    [Tooltip("The number of flowers that should be in one 'cluster'")]
    public int flowerClusterNumber = 3;

    // Curriculum variables
    public static float use_radius = 1.0f;
    public static float hive_radius = 6.0f;

    private void Start() {
        // Instantiate flowers
        InstantiateFlowers();
    }

    public void ResetEnvironment() {
        NewFlowerPositions();

        // Reset flowers
        if(flowers != null) {
            foreach(GameObject flower in flowers) {
                flower.GetComponentInChildren<Flower>().ResetFlower();
            }
        }

        // Reset hive
        hive.OnReset();
    }

    /// <summary>
    /// Used to get a random position for on a plane relative to a given point. 
    /// </summary>
    /// <param author="Immersive Limit"></param>
    /// <param name="center"></param>
    /// <param name="minAngle"></param>
    /// <param name="maxAngle"></param>
    /// <param name="minRadius"></param>
    /// <param name="maxRadius"></param>
    /// <returns></returns>
    public static Vector3 ChooseRandomPosition(Vector3 center, float minAngle, float maxAngle, float minRadius, float maxRadius) {
        float radius = minRadius;

        if (maxRadius > minRadius) {
            radius = UnityEngine.Random.Range(minRadius, maxRadius);
        }

        return center + Quaternion.Euler(0.0f, UnityEngine.Random.Range(minAngle, maxAngle), 0.0f) * Vector3.forward * radius;
    }

    /// <summary>
    /// Creates the initial flowers and positions them (as specified by initial parameters)
    /// </summary>
    private void InstantiateFlowers() {
        // Flowers should spawn roughly in clusters of n around a central position
        Vector3 flowerClusterPosition = transform.position;

        for(int i = 0; i < flowerCount; ++i) {
            // Get a new cluster position for every n flower
            if(i % flowerClusterNumber == 0) {
                flowerClusterPosition = ChooseRandomPosition(transform.position, 0.0f, 360.0f, flowerSpawnMinRadius, flowerSpawnMaxRadius);
            }

            // Instantiate a new flower object
            GameObject flower = GameObject.Instantiate<GameObject>(flowerPrefab);

            // Set its position 
            flower.transform.position = ChooseRandomPosition(flowerClusterPosition, 0.0f, 360.0f, 1.0f, 3.0f);

            // Parent the flower to the environment
            flower.transform.SetParent(transform);

            // Give the flower its ID
            flower.GetComponentInChildren<Flower>().id = i;

            // Add to list
            flowers.Add(flower);

        }
    }

    /// <summary>
    /// Finds a new random position for all existing flowers
    /// </summary>
    private void NewFlowerPositions() {
        // Check to see if we need more flowers
        if(flowerCount > flowers.Count) {
            AddNewFlowers();
        }

        // Flowers should spawn roughly in clusters of n around a central position
        Vector3 flowerClusterPosition = transform.position;

        // Iterate through flowers and reset their positions
        for(int i = 0; i < flowers.Count; ++i) {
            // Get a new cluster position for every n flowers
            if(i % flowerClusterNumber == 0) {
                flowerClusterPosition = ChooseRandomPosition(transform.position, 0.0f, 360.0f, flowerSpawnMinRadius, flowerSpawnMaxRadius);
            }

            // Set the flower position
            flowers[i].transform.position = ChooseRandomPosition(flowerClusterPosition, 0.0f, 360.0f, 1.5f, 3.0f);
            // Set a random rotation
            flowers[i].transform.Rotate(new Vector3(0.0f, Random.Range(0.0f, 360.0f), 0.0f));

            // Reset nectar
            flowers[i].GetComponentInChildren<Flower>().nectar = 0.0f;    // we know the component exists - this is done here purely for efficiency, 
        }
    }

    /// <summary>
    /// Instantiates new flower prefabs if the count has increased
    /// </summary>
    private void AddNewFlowers() {
        // Safety check
        if(flowerCount <= flowers.Count) {
            Debug.LogWarning("Warning: Attempting to add new flowers when the flowerCount < flowers list count.");
            return;
        }

        // Iterate from the current count to the new count
        for(int i = flowers.Count; i < flowerCount; ++i) {
            // Create new instance
            GameObject flower = GameObject.Instantiate<GameObject>(flowerPrefab);
            flower.transform.SetParent(transform);

            // Set ID
            flower.GetComponentInChildren<Flower>().id = i;

            // Add to list
            flowers.Add(flower);
        }
    }
}
