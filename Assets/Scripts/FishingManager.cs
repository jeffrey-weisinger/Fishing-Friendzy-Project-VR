using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public Transform castPoint;        // Bobber spawn position
    public GameObject bobberPrefab;    // Prefab of the bobber to be cast
    public GameObject followBobber;    // Idle bobber attached to rod tip

    private GameObject currentBobber;  // Currently active bobber
    private bool isFishing = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isFishing)
        {
            CastBobber();
        }
    }

    void CastBobber()
    {
        isFishing = true;

        // Hide the bobber attached to the rod tip
        if (followBobber != null)
            followBobber.SetActive(false);

        // Instantiate and launch the new bobber
        currentBobber = Instantiate(bobberPrefab, castPoint.position, Quaternion.identity);
        Rigidbody rb = currentBobber.GetComponent<Rigidbody>();
        rb.AddForce(castPoint.forward * 6f + Vector3.up * 2f, ForceMode.Impulse);

        Debug.Log("Bobber has been cast!");
    }

    // Can be called externally (e.g., via button press or after some time)
    public void ResetFishing()
    {
        isFishing = false;

        if (currentBobber != null)
            Destroy(currentBobber);

        if (followBobber != null)
            followBobber.SetActive(true);
    }
}
