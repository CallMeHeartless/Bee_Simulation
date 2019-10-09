using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class BeeAgent : Agent
{
    // Internal references
    private RayPerception3D rayPerception;          // Used to perform ML raycasts and detect objects in the enviroment
    private Rigidbody rigidBody;                    // Used to move the bee
    private Material beeMaterial;                   // Used to help visualise the bee's status

    // External References
    public BeeEnvironment beeEnvironment;           // Used to instruct the environment when to reset
    public Hive theHive;

    // Rayperception values
    static float rayPerceptionDistance = 20.0f;
    static string[] detectableObjects = { "Flower", "Hive" };//, "Bee" };        // 3 types of object
    static float[] detectionAngles = {20.0f, 45.0f, 60.0f, 75.0f, 90.0f, 105.0f, 120.0f, 135.0f, 160.0f}; // 9 angles

    [Header("Movement Variables")]
    [SerializeField][Tooltip("The bee's forward speed (m/s)")]
    private float moveSpeed = 1.0f;                     
    [SerializeField][Tooltip("The bee's turn speed (degrees/s)")]
    private float turnSpeed = 45.0f;
    private float thrust = 0.0f;                    // The thrust moving the bee forward each fixed update [0, 1.0f]
    private Vector2 rotationVector = Vector2.zero;  // Where x = x axis rotation and y = y axis rotation for each fixed update [-1.0f, 1.0f]

    public float nectarDrain;
    public float nectar = 0.0f;
    private static float maxNectar = 3.0f;
    private Vector3 lastKnownFlower = Vector3.zero;
    private Vector3 hivePosition = Vector3.zero;

    private void Start() {
        // Obtain references
        rayPerception = GetComponent<RayPerception3D>();
        rigidBody = GetComponent<Rigidbody>();
        nectarDrain = Time.fixedDeltaTime;
        beeMaterial = GetComponentInChildren<MeshRenderer>().material;
        hivePosition = theHive.transform.position;
    }

    private void FixedUpdate() {
        // Rotate the bee
        Vector2 rotateBy = rotationVector * turnSpeed * Time.fixedDeltaTime;
        transform.Rotate(rotateBy.x, rotateBy.y, 0.0f);

        // Move the bee forward
        rigidBody.velocity = transform.forward * thrust * moveSpeed; // We don't really care about the bee moving in a realistic way here

        // Penalise the bee for not moving
        if(thrust == 0.0f) {
            SetReward(-1.0f / agentParameters.maxStep);
        }

        // Visual update
        if (beeMaterial) {
            beeMaterial.color = Color.Lerp(Color.yellow, Color.blue, (nectar / maxNectar));
        }

        // Curriculum training
        if(BeeEnvironment.use_radius == 1.0f && nectar > 0.0f) {
            CurriculumTraining();
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction) {
        // Determine how to move the bee during the next fixed update step
        MoveBee(vectorAction);
        
        // Apply a penalty to encourage movement
        SetReward(-1.0f / (float)agentParameters.maxStep);
    }

    public override void AgentReset() {
        // Empty nectar
        nectar = 0.0f;

        // null last known flower
        lastKnownFlower = hivePosition;

        // Instruct environment to reset
        beeEnvironment.ResetEnvironment();

        // Reset position and rotation
        transform.position = hivePosition + Vector3.forward;
        transform.rotation = Quaternion.identity;

        // Reset colour
        beeMaterial.color = Color.yellow;
    }

    public override void CollectObservations() {
        // Send the agent's nectar value (1)
        AddVectorObs(nectar);

        // Send the agent's max nectar value (1)
        AddVectorObs(maxNectar);

        // Send agent's velocity (3) [Note: this is the same as the forward vector of the transform]
        AddVectorObs(transform.forward); //rigidBody.velocity.normalized

        // Normalise agent's rotation 
        Vector3 normalisedRotation = (transform.rotation.eulerAngles / 180.0f) - Vector3.one; // Convert to [-1, 1]

        // Send agent's rotation (3)
        AddVectorObs(normalisedRotation);

        // Send distance to hive (1)
        AddVectorObs(Vector3.Distance(transform.position, hivePosition));

        // Send direction to hive (3)
        AddVectorObs((transform.position - hivePosition).normalized);

        // Rayperception information (9 * (3+2) = 45) // 36
        List<float> perception = rayPerception.Perceive(rayPerceptionDistance, detectionAngles, detectableObjects, 0, 0);
        AddVectorObs(perception);
        // Look down (36)
        //List<float> downwardPerception = rayPerception.Perceive(rayPerceptionDistance, detectionAngles, detectableObjects, 0, -1.0f);
        //AddVectorObs(downwardPerception);
        //// Look up (36)
        //List<float> upwardPerception = rayPerception.Perceive(rayPerceptionDistance, detectionAngles, detectableObjects, 0, 1.0f);
        //AddVectorObs(upwardPerception);
    }

    /// <summary>
    /// Determines what the bee's thrust and rotation should be during the next fixed update
    /// </summary>
    /// <param name="moveVector">The continuous vector action parametrs, where index 0 = thrust, index 1 = x-axis rotation, and index 2 = y-axis rotation</param>
    private void MoveBee(float[] moveVector) {
        // Clamp forward movement (bees can't move backwards)
        thrust = Mathf.Clamp(moveVector[0], 0.0f, 1.0f);
        //thrust = moveVector[0];

        // Determine rotation
        //rotationVector.x = moveVector[1];
        rotationVector.y = moveVector[1]; // formerly 2
    }

    /// <summary>
    /// The mechanism through which bees will interact with objects - by bumping into them
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay(Collision collision) {
        // Check for hive
        if (collision.gameObject.CompareTag("Hive")) {

            // Hive interaction
            Hive hive = collision.gameObject.GetComponentInParent<Hive>();
            if (hive) { // Safety check
                HiveInteraction(hive);
            }
        }
        // Check for flower
        else if (collision.gameObject.CompareTag("Flower")) {

            // Flower interaction
            Flower flower = collision.gameObject.GetComponent<Flower>();
           
            if (flower) {   // Safety check
                FlowerInteraction(flower);
            }
        }
        //else if (collision.gameObject.CompareTag("Bee")) {
        //    // Bee interaction
        //}
    }

    /// <summary>
    /// This is used to add a flower to the list of recently visited flowers
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision) {
        
    }

    /// <summary>
    /// Attempts to give the bee's nectar to the hive, rewarding the agent if successful
    /// </summary>
    /// <param name="hive"></param>
    private void HiveInteraction(Hive hive) {
        // Only process this interaction if the bee has nectar
        if (nectar > 0.0f) {
            // Add the bee's nectar to the hive
            hive.nectar += nectar;

            // Reward the bee for being good and returning nectar to the hive
            SetReward(nectar);

            // Remove the nectar from the bee
            nectar = 0.0f;

            // Change colour back  
            beeMaterial.color = Color.yellow;
        }
    }

    /// <summary>
    /// Extracts nectar from a provided flower (if possible), adding it to the bee's store.
    /// </summary>
    /// <param name="flower"></param>
    private void FlowerInteraction(Flower flower) {
        // End interaction if the flower does not have nectar
        if(flower.nectar <= 0.0f || nectar >=maxNectar) {
            return;
        }

        // Take some nectar from the flower
        flower.SubtractNectar(Time.fixedDeltaTime);//Time.fixedDeltaTime

        // Give it to the bee
        nectar += Time.fixedDeltaTime;
        //nectar = 3.0f;

        // Clamp logically
        nectar = Mathf.Clamp(nectar, 0.0f, maxNectar);

        // Debug colour change when the bee is full on nectar
        if(nectar == maxNectar) {
            beeMaterial.color = Color.blue;
        }

        // Reward the bee
        SetReward(Time.fixedDeltaTime);//
    }

    /// <summary>
    /// This function is used to help the bee agents learn - during curriculum training, the bee does not have to completely return to the hive to get a reward
    /// This behaviour is encouraged by rewarding the bee when they get close enough, and gradually reducing the distance needed
    /// </summary>
    private void CurriculumTraining() {
        // Find distance to hive
        float distanceToHive = Vector3.Distance(transform.position, theHive.transform.position);

        // Check if we are within our training distance
        if(distanceToHive <= BeeEnvironment.hive_radius) {
            HiveInteraction(theHive);
        }
    }

}
