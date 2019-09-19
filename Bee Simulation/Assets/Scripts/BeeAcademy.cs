using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class BeeAcademy : Academy
{
    public override void AcademyReset() {
        // Update curriculum
        BeeEnvironment.hive_radius = resetParameters["hive_radius"];
        BeeEnvironment.use_radius = resetParameters["use_radius"];
    }
}
