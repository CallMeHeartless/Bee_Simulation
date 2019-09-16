using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeEnvironment : MonoBehaviour
{
    // Internal references
    public GameObject hive;
    public GameObject flowerPrefab;
    public List<GameObject> flowers = new List<GameObject>();

    [Header("Flower Variables")]
    public int flowerCount = 3; // The number of flowers that should be in the scene
    public float flowerSpawnMinRadius = 10.0f;
    public float flowerSpawnMaxRadius = 20.0f;
    [Tooltip("The number of flowers that should be in one 'cluster'")]
    public int flowerClusterNumber = 3;

    private void Start() {
        // Instantiate flowers
        InstantiateFlowers();
    }

    public void ResetEnvironment() {
        
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

        }
    }
}
