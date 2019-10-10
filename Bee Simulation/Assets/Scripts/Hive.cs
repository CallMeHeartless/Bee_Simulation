using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hive : MonoBehaviour
{
    public TextMeshPro nectarScore;
    public float nectar = 0.0f;


    private void FixedUpdate() {
        // Update the score [Consider refactoring this to be a function called during the Hive Interaction]
        nectarScore.text = nectar.ToString("0.00");
    }


    public void OnReset() {
        nectar = 0.0f;
    }

}
