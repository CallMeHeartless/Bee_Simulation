using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public float maxNectar = 1.0f;
    //[HideInInspector]
    public float nectar = 0.0f;     // Represents the current amount of nectar the flower has
    [HideInInspector]
    public int id;                  // A unique ID to differentiate the flower from its fellows
    [Range(4.0f, 7.0f)][Tooltip("The amount of time (in seconds) it takes before the flower starts regaining nectar")]
    public float nectarRefillDelay;
    private bool isRefilling = true;
    private Material flowerMaterial;

    private static float minScale = 0.5f;       // The minimum scale that can be applied to the flower on reset
    private static float maxScale = 1.5f;       // The maximum scale that can be applied to the flower on reset

    private void Start() {
        // Obtain references
        flowerMaterial = GetComponent<MeshRenderer>().material;

        // Change this flower's scale 
        ResetFlower();
    }

    // Fixed update is being used in an effort to prevent errors in training from an accelerated timescale 
    private void FixedUpdate() {
        // Process nectar
        if(isRefilling && nectar < maxNectar) {
            nectar += Time.fixedDeltaTime;

            // Logically clamp value
            nectar = Mathf.Clamp(nectar, 0.0f, maxNectar);

            // Stop refilling if maxed out
            if(nectar == maxNectar) {
                isRefilling = false;
            }
        }

        // Lerp material between two colours based on nectar content
        if (flowerMaterial) {
            flowerMaterial.color = Color.Lerp(Color.black, Color.white, nectar / maxNectar);
        }
    }

    /// <summary>
    /// This function is called when the environment is reset and upon initialisation
    /// </summary>
    public void ResetFlower() {
        isRefilling = true;
        nectar = maxNectar;

        // Assign a random scale
        transform.parent.localScale = Vector3.one * Random.Range(minScale, maxScale);
    }

    /// <summary>
    /// Used by bees to extract nectar from a flower
    /// </summary>
    /// <param name="loss"></param>
    public void SubtractNectar(float loss) {
        // Subtract the amount lost
        nectar -= loss;

        // Clamp the value logically
        nectar = Mathf.Clamp(nectar, 0.0f, maxNectar);

        // Start refilling once drained (after a delay)
        if(nectar == 0.0f && !isRefilling) {
            Invoke("StartRefilling", nectarRefillDelay);
        }
        // If a bee takes nectar while the flower is refilling, stop it from refilling while being drained
        else if (isRefilling) {
            isRefilling = false;
            Invoke("StartRefilling", nectarRefillDelay);
        }
    }

    /// <summary>
    /// Toggles the isRefilling boolean flag, instructing the flower to start regaining nectar
    /// </summary>
    private void StartRefilling() {
        isRefilling = true;
    }
    
}
