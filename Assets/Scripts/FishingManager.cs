using UnityEngine;
using System.Collections;
using UnityEngine.UI; // For UI components

public class FishingManager : MonoBehaviour
{
    [Header("Bobber Setup")]
    public Transform castPoint;
    public GameObject bobberPrefab;
    public GameObject followBobber;
    
    [Header("Fish Setup")]
    public GameObject fish1; // Assign this in the Inspector

    private GameObject currentBobber;
    private bool isFishing = false;
    private Coroutine resetCoroutine;

    void Start()
    {
        // Try to find fish1 if not assigned in Inspector
        if (fish1 == null)
        {
            fish1 = GameObject.Find("fish1");
            Debug.Log("Looking for fish1: " + (fish1 != null ? "Found" : "Not found"));
        }
        
        // Make fish1 invisible at start
        if (fish1 != null)
        {
            fish1.SetActive(false);
            Debug.Log("Fish1 set to invisible at start");
        }
        else
        {
            Debug.LogError("Fish1 not found! Please assign it in the Inspector");
        }
    }

    public void Cast()
    {
        if (isFishing) return;
        isFishing = true;

        if (followBobber != null)
            followBobber.SetActive(false);

        // Ensure fish is hidden when casting
        if (fish1 != null)
        {
            fish1.SetActive(false);
            Debug.Log("Fish1 hidden during cast");
        }

        currentBobber = Instantiate(bobberPrefab, castPoint.position, Quaternion.identity);
        Rigidbody rb = currentBobber.GetComponent<Rigidbody>();
        rb.AddForce(castPoint.forward * 6f + Vector3.up * 2f, ForceMode.Impulse);

        Debug.Log("Bobber has been cast!");
        resetCoroutine = StartCoroutine(ResetFishingAfterDelay());
    }

    private IEnumerator ResetFishingAfterDelay()
    {
        float delay = Random.Range(5f, 10f);
        Debug.Log($"Fish will appear in {delay} seconds");
        yield return new WaitForSeconds(delay);
        ResetFishing();
    }
 
    public void ResetFishing()
    {
        // Cancel pending reset if called manually
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }

        isFishing = false;
        if (currentBobber != null) Destroy(currentBobber);
        if (followBobber != null) followBobber.SetActive(true);
        
        // Make fish1 visible when fishing is reset
        if (fish1 != null)
        {
            fish1.SetActive(true);
            Debug.Log("Fish1 made visible after fishing reset");
        }
        else
        {
            Debug.LogError("Cannot show fish1 - reference is null");
        }
    }
}



// using UnityEngine;
// using System.Collections;

// public class FishingManager : MonoBehaviour
// {
//     [Header("Bobber Setup")]
//     public Transform castPoint;
//     public GameObject bobberPrefab;
//     public GameObject followBobber;
    
//     [Header("Fish Setup")]
//     public GameObject fish1;

//     private GameObject currentBobber;
//     private bool isFishing = false;
//     private Coroutine resetCoroutine;

//     void Start()
//     {
//         // Make fish invisible at start
//         if (fish1 != null)
//             fish1.SetActive(false);
//     }

//     public void Cast()
//     {
//         if (isFishing) return;
//         isFishing = true;

//         if (followBobber != null)
//             followBobber.SetActive(false);

//         currentBobber = Instantiate(bobberPrefab, castPoint.position, Quaternion.identity);
//         Rigidbody rb = currentBobber.GetComponent<Rigidbody>();
//         rb.AddForce(castPoint.forward * 6f + Vector3.up * 2f, ForceMode.Impulse);

//         Debug.Log("Bobber has been cast!");
//         resetCoroutine = StartCoroutine(ResetFishingAfterDelay());
//     }

//     private IEnumerator ResetFishingAfterDelay()
//     {
//         float delay = Random.Range(5f, 10f);
//         Debug.Log($"Fish will appear in {delay} seconds");
//         yield return new WaitForSeconds(delay);
        
//         // Call ResetFishing with parameter indicating it was called from Cast
//         ResetFishing(true);
//     }
 
//     // Modified to accept parameter indicating if called from Cast
//     public void ResetFishing(bool calledFromCast = false)
//     {
//         // Cancel pending reset if called manually
//         if (resetCoroutine != null)
//         {
//             StopCoroutine(resetCoroutine);
//             resetCoroutine = null;
//         }

//         isFishing = false;
//         if (currentBobber != null) Destroy(currentBobber);
//         if (followBobber != null) followBobber.SetActive(true);
        
//         // Only make fish visible if called from Cast via coroutine
//         if (calledFromCast && fish1 != null)
//         {
//             fish1.SetActive(true);
//             Debug.Log("Fish made visible - reset triggered by Cast");
//         }
//     }
// }
