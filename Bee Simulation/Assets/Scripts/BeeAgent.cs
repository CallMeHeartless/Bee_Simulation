using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class BeeAgent : Agent
{
    // Internal references
    private RayPerception3D rayPerception;          // Used to perform ML raycasts and detect objects in the enviroment
    private Rigidbody rigidBody;                    // Used to move the bee

    // Rayperception values
    static float rayPerceptionDistance = 10.0f;
    static string[] detectableObjects = { "Flower", "Hive" };//, "Bee" };        // 3 types of object
    static float[] detectionAngles = {20.0f, 45.0f, 60.0f, 75.0f, 90.0f, 105.0f, 120.0f, 135.0f, 160.0f}; // 9 angles

    [Header("Movement Variables")]
    [SerializeField][Tooltip("The bee's forward speed (m/s)")]
    private float moveSpeed = 1.0f;                     
    [SerializeField][Tooltip("The bee's turn speed (degrees/s)")]
    private float turnSpeed = 45.0f;
    private float thrust = 0.0f;                    // The thrust moving the bee forward each fixed update [0, 1.0f]
    private Vector2 rotationVector = Vector2.zero;  // Where x = x axis rotation and y = y axis rotation for each fixed update [-1.0f, 1.0f]

    private float nectar = 0.0f;

    private void Start() {
        // Obtain references
        rayPerception = GetComponent<RayPerception3D>();
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        // Rotate the bee
        Vector2 rotateBy = rotationVector * turnSpeed * Time.fixedDeltaTime;
        transform.Rotate(rotateBy.x, rotateBy.y, 0.0f);

        // Move the bee forward
        rigidBody.AddForce(thrust * transform.forward * Time.fixedDeltaTime * moveSpeed, ForceMode.VelocityChange); // We don't really care about the bee's mass
    }

    public override void AgentAction(float[] vectorAction, string textAction) {
        // Determine how to move the bee during the next fixed update step
        MoveBee(vectorAction);
        
        // Apply a penalty to encourage movement
        SetReward(-1.0f / (float)agentParameters.maxStep);
    }

    public override void AgentReset() {
        
    }

    public override void CollectObservations() {
        // Send the agent's nectar value (1)
        AddVectorObs(nectar);

        // Send agent's velocity (3)
        AddVectorObs(rigidBody.velocity.normalized);

        // Normalise agent's rotation 
        Vector3 normalisedRotation = (transform.rotation.eulerAngles / 180.0f) - Vector3.one; // Convert to [-1, 1]

        // Send agent's rotation (3)
        AddVectorObs(normalisedRotation);

        // Rayperception information (9 * (3+2) = 45) // 36
        List<float> perception = rayPerception.Perceive(rayPerceptionDistance, detectionAngles, detectableObjects, 0, 0);
        AddVectorObs(perception);
    }

    /// <summary>
    /// Determines what the bee's thrust and rotation should be during the next fixed update
    /// </summary>
    /// <param name="moveVector">The continuous vector action parametrs, where index 0 = thrust, index 1 = x-axis rotation, and index 2 = y-axis rotation</param>
    private void MoveBee(float[] moveVector) {
        // Clamp forward movement (bees can't move backwards)
        thrust = Mathf.Clamp(moveVector[0], 0.0f, 1.0f);

        // Determine rotation
        rotationVector.x = moveVector[1];
        rotationVector.y = moveVector[2];
    }

    
}
