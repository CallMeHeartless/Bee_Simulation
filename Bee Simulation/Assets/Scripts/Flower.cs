using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public float maxNectar = 1.0f;
    [HideInInspector]
    public float nectar = 0.0f;     // Represents the current amount of nectar the flower has
    [HideInInspector]
    public int id;                  // A unique ID to differentiate the flower from its fellows
    [Range(4.0f, 7.0f)][Tooltip("The amount of time (in seconds) it takes before the flower starts regaining nectar")]
    public float nectarRefillDelay;

    private static float minScale = 0.5f;       // The minimum scale that can be applied to the flower on reset
    private static float maxScale = 1.5f;       // The maximum scale that can be applied to the flower on reset

    private void Start() {
        // Change this flower's scale 
        ResetFlower();
    }

    // Fixed update is being used in an effort to prevent errors in training from an accelerated timescale 
    private void FixedUpdate() {
        
    }

    /// <summary>
    /// This function is called when the environment is reset and upon initialisation
    /// </summary>
    public void ResetFlower() {
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * Random.Range(minScale, maxScale);
    }

    public void SubtractNectar(float loss) {
        // Subtract the amount lost
        nectar -= loss;

        // Clamp the value logically
        nectar = Mathf.Clamp(nectar, 0.0f, maxNectar);
    }
    
}
