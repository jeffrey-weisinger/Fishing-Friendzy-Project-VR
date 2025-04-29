// using UnityEngine;

// public class FishingManager : MonoBehaviour
// {
//     [Header("Bobber Setup")]
//     public Transform castPoint;
//     public GameObject bobberPrefab;
//     public GameObject followBobber;

//     private GameObject currentBobber;
//     private bool isFishing = false;

//     // This gets wired up by the PlayerInput → Unity Event
//     public void Cast()
//     {
//         if (isFishing) return;
//         isFishing = true;

//         if (followBobber != null)
//             followBobber.SetActive(false);

//         currentBobber = Instantiate(bobberPrefab, castPoint.position, Quaternion.identity);
//         var rb = currentBobber.GetComponent<Rigidbody>();
//         rb.AddForce(castPoint.forward * 6f + Vector3.up * 2f, ForceMode.Impulse);

//         Debug.Log("Bobber has been cast!");
//     }

//     public void ResetFishing()
//     {
//         isFishing = false;
//         if (currentBobber != null) Destroy(currentBobber);
//         if (followBobber  != null) followBobber.SetActive(true);
//     }
// }
 
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    [Header("Bobber Setup")]
    public Transform castPoint;       
    public GameObject bobberPrefab;   
    public GameObject followBobber;   

    private GameObject currentBobber; 
    private bool isFishing = false;

    // → Bind this to your PlayerInput “Cast” event
    public void Cast()
    {
        if (isFishing) return;
        isFishing = true;

        if (followBobber != null)
            followBobber.SetActive(false);

        currentBobber = Instantiate(
            bobberPrefab,
            castPoint.position,
            Quaternion.identity
        );
        currentBobber.GetComponent<Rigidbody>()
            .AddForce(castPoint.forward * 6f + Vector3.up * 2f,
                      ForceMode.Impulse);

        Debug.Log("Bobber has been cast!");

        // automatically retrieve after 5 seconds
        Invoke(nameof(ResetFishing), 5f);
    }

    // → Bind this to your PlayerInput “Retrieve” event (or call manually)
    public void ResetFishing()
    {
        if (!isFishing) return;
        isFishing = false;

        // cancel the pending auto-invoke if it’s still pending
        CancelInvoke(nameof(ResetFishing));

        if (currentBobber != null)
            Destroy(currentBobber);

        if (followBobber != null)
            followBobber.SetActive(true);

        Debug.Log("Bobber has been retrieved!");
    }
}

 