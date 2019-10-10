using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using UnityEngine.SceneManagement;

public class BeeAcademy : Academy
{
    private void Update() {
        // Allow manual resetting of the scene
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public override void AcademyReset() {
        // Update curriculum
        BeeEnvironment.hive_radius = resetParameters["hive_radius"];
        BeeEnvironment.use_radius = resetParameters["use_radius"];
    }
}
