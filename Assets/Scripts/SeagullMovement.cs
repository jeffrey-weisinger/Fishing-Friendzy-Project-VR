using UnityEngine;

public class SeagullMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("How fast the seagull flies forward")]
    [SerializeField] private float speed = 25.0f;

    [Tooltip("How far the seagull flies before dissapearing")]
    [SerializeField] private float disappearDistance = 600.0f;

    [Header("Reset Behavior")]
    [Tooltip("Slight variation added to the reset position")]
    [SerializeField] private float positionVariance = 5.0f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        // Store the initial position/rotation on game start
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        // Move seagull forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Calculate the distance flown from start
        float distanceFlown = Vector3.Distance(startPosition, transform.position);
        if (distanceFlown >= disappearDistance)
        {
            ResetSeagullPosition();
        }
    }

    void ResetSeagullPosition()
    {
        // Calculate a random starting position
        Vector3 randomOffset = new Vector3(
            Random.Range(-positionVariance, positionVariance),
            Random.Range(-positionVariance / 2, positionVariance / 2),
            Random.Range(-positionVariance, positionVariance)
        );

        transform.position = startPosition + randomOffset;
        transform.rotation = startRotation;

    }
}