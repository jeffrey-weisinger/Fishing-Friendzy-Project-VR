using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody))]
public class FishLocomotion : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Force applied in the fish's forward direction. Increase significantly when using AddForce. Works with Rigidbody Drag.")]
    public float moveSpeed = 100.0f;
    [Tooltip("Maximum speed the fish can reach. Relies on Drag primarily, but this provides a hard cap.")]
    public float maxSpeed = 20.0f; 
    [Tooltip("Speed at which the fish turns (pitch and yaw).")]
    public float turnSpeed = 0.8f;
    [Tooltip("How close the fish needs to be to its target destination to consider it reached.")]
    public float destinationThreshold = 1.5f;

    [Header("Behavior Timing")]
    public float minSwimTime = 3.0f;
    public float maxSwimTime = 10.0f;
    public float minPauseTime = 1.0f;
    public float maxPauseTime = 2.0f;

    [Header("Swimming Boundaries")]
    public Vector3 boundaryCenter = new Vector3(1704.69995f, -5.0f, 1108.40002f);
    public float boundaryRadius = 200.0f;
    public float waterSurfaceYLevel = 1.0f;
    public float waterBottomYLevel = -40.0f;

    public float verticalAvoidanceBuffer = 1.0f; 

    [Header("Physics Settings")]
    [Tooltip("Rigidbody linear drag. Higher values resist movement and reduce sideways drift.")]
    public float rigidbodyDrag = 5.0f;
    [Tooltip("Rigidbody angular drag. Resists rotation.")]
    public float rigidbodyAngularDrag = 2.0f;

    private Rigidbody rb;
    private Vector3 targetPosition;
    private FishState currentState = FishState.Idle;
    private Coroutine currentBehaviorCoroutine;

    private enum FishState
    {
        Idle,
        Swimming,
        Pausing
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Rigidbody Configuration
        rb.useGravity = false;
        rb.drag = rigidbodyDrag;
        rb.angularDrag = rigidbodyAngularDrag;

        // Allow Y movement and X rotation (Pitch), keep Z rotation (Roll) frozen
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;

        // Validate boundary Y levels
        if (waterBottomYLevel >= waterSurfaceYLevel)
        {
            Debug.LogError("Water Bottom Y Level must be less than Water Surface Y Level!", this);
            waterBottomYLevel = waterSurfaceYLevel - 1.0f;
        }

        // Start within the defined volume
        transform.position = GetRandomPointInVolume(boundaryCenter, boundaryRadius, waterSurfaceYLevel, waterBottomYLevel);
        rb.position = transform.position; // Ensure Rigidbody position matches

        StartBehavior();
    }

    void FixedUpdate()
    {
        // Boundary checks
        if (currentState != FishState.Idle)
        {
            if (CheckBoundary())
            {
                // Boundary hit, target adjusted, ensure swimming state
                currentState = FishState.Swimming;
            }
        }

        // Perform actions based on state
        if (currentState == FishState.Swimming)
        {
            MoveTowardsTarget();
            ApplySpeedLimit();
        }
        else if (currentState == FishState.Pausing)
        {
            // Rely primarily on drag to slow down.
             rb.velocity *= 0.98f; // Damping multiplier
             rb.angularVelocity *= 0.95f;
        }
    }

    // Behavior Control
    void StartBehavior()
    {
        if (currentBehaviorCoroutine != null)
        {
            StopCoroutine(currentBehaviorCoroutine);
        }
        currentBehaviorCoroutine = StartCoroutine(BehaviorRoutine());
    }

    IEnumerator BehaviorRoutine()
    {
        currentState = FishState.Idle;

        while (true)
        {
            bool shouldSwim = Random.Range(0, 8) > 0;

            if (shouldSwim)
            {
                currentState = FishState.Swimming;
                targetPosition = GetRandomPointInVolume(boundaryCenter, boundaryRadius, waterSurfaceYLevel, waterBottomYLevel);
                float swimDuration = Random.Range(minSwimTime, maxSwimTime);
                float swimTimer = 0f;

                while (swimTimer < swimDuration && Vector3.Distance(rb.position, targetPosition) > destinationThreshold)
                {
                    if (currentState != FishState.Swimming) yield break; // Exit if state changed (ex: by boundary)

                    swimTimer += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                currentState = FishState.Pausing;
                float pauseDuration = Random.Range(minPauseTime, maxPauseTime);
                // Ensure velocity is low before waiting
                rb.velocity *= 0.5f; // Reduction before pause starts fully
                rb.angularVelocity *= 0.5f;
                yield return new WaitForSeconds(pauseDuration);
            }

            currentState = FishState.Idle;
            yield return null;
        }
    }

    // Movement & Rotation
    void MoveTowardsTarget()
    {
        if (targetPosition == null) return;

        Vector3 directionToTarget = (targetPosition - rb.position).normalized;

        if (directionToTarget != Vector3.zero)
        {
            // Rotation
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newRotation);

            // Movement
            // Applies force along the fish's current forward direction.
            rb.AddForce(transform.forward * moveSpeed, ForceMode.Acceleration);
        }
    }

    // Cap the speed
    void ApplySpeedLimit()
    {
         if (rb.velocity.magnitude > maxSpeed)
         {
              rb.velocity = rb.velocity.normalized * maxSpeed;
         }
    }


    // Boundary Management
    bool CheckBoundary()
    {
        bool boundaryHit = false;
        Vector3 currentPos = rb.position;
        Vector3 newTarget = targetPosition;

        // Vertical Boundaries
        if (currentPos.y > waterSurfaceYLevel)
        {
            newTarget = new Vector3(currentPos.x, waterSurfaceYLevel - verticalAvoidanceBuffer, currentPos.z);
            // Downward force impulse for quicker correction
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            boundaryHit = true;
        }
        else if (currentPos.y < waterBottomYLevel)
        {
            newTarget = new Vector3(currentPos.x, waterBottomYLevel + verticalAvoidanceBuffer, currentPos.z);
             // Upward force impulse
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            boundaryHit = true;
        }

        // Horizontal Boundary
        float horizontalDist = HorizontalDistance(currentPos, boundaryCenter);
        if (horizontalDist > boundaryRadius)
        {
            Vector3 horizontalCenter = new Vector3(boundaryCenter.x, currentPos.y, boundaryCenter.z);
            Vector3 directionToCenter = (horizontalCenter - currentPos).normalized;
             directionToCenter.y = 0; // Ensure horizontal correction

            float targetY = boundaryHit ? newTarget.y : currentPos.y;
            // Aim towards center, slightly inside radius
            newTarget = horizontalCenter + directionToCenter * (boundaryRadius * 0.9f);
            newTarget.y = targetY;

            boundaryHit = true;
        }

        // Update target if boundary was hit
        if (boundaryHit)
        {
            targetPosition = newTarget;
            return true;
        }

        return false;
    }

    float HorizontalDistance(Vector3 a, Vector3 b)
    {
        return Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
    }

    Vector3 GetRandomPointInVolume(Vector3 center, float radius, float topY, float bottomY)
    {
        Vector2 randomXZ = Random.insideUnitCircle * radius;
        float randomY = Random.Range(bottomY, topY);
        return new Vector3(center.x + randomXZ.x, randomY, center.z + randomXZ.y);
    }

    void OnDrawGizmosSelected()
    {
        if (waterBottomYLevel >= waterSurfaceYLevel) return;

        Color gizmoColor = Color.cyan;

#if UNITY_EDITOR
        Handles.color = gizmoColor;
        Vector3 topCenter = new Vector3(boundaryCenter.x, waterSurfaceYLevel, boundaryCenter.z);
        Vector3 bottomCenter = new Vector3(boundaryCenter.x, waterBottomYLevel, boundaryCenter.z);

        Handles.DrawWireDisc(topCenter, Vector3.up, boundaryRadius);
        Handles.DrawWireDisc(bottomCenter, Vector3.up, boundaryRadius);

        int segments = 12;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * (360f / segments) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * boundaryRadius;
            float z = Mathf.Sin(angle) * boundaryRadius;
            Handles.DrawLine(topCenter + new Vector3(x, 0, z), bottomCenter + new Vector3(x, 0, z));
        }
#else
        Gizmos.color = gizmoColor;
#endif

        if (currentState == FishState.Swimming)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetPosition, 0.5f);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, targetPosition);
        }

        // Visualize current velocity
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + rb.velocity);
    }
}