using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hive : MonoBehaviour
{
    public TextMeshPro nectarScore;
    public float nectar = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate() {
        nectarScore.text = nectar.ToString("0.00");
    }


    public void OnReset() {
        nectar = 0.0f;
    }

}
