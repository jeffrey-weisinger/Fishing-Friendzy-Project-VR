// This script uses code from ChatGPT.

// using UnityEngine;
// using System.Collections;

// public class FishingManager : MonoBehaviour
// {
//     [Header("Bobber Setup")]
//     public Transform castPoint;
//     public GameObject bobberPrefab;
//     public GameObject followBobber;
    
//     [Header("Fish Setup")]
//     public GameObject fish1; // Drag fish1 here in Inspector

//     private GameObject currentBobber;
//     private bool isFishing = false;
//     private Coroutine resetCoroutine;

//     void Awake()
//     {
//         // Force-disable fish1 on startup
//         if (fish1 != null)
//         {
//             fish1.SetActive(false);
//             Debug.Log("Fish1 initialized as hidden");
//         }
//         else
//         {
//             Debug.LogError("Assign fish1 GameObject in Inspector!");
//         }
//     }

//     public void Cast()
//     {
//         if (isFishing) return;
//         isFishing = true;

//         // Hide follow-bobber
//         if (followBobber != null)
//             followBobber.SetActive(false);

//         // Create new bobber
//         currentBobber = Instantiate(bobberPrefab, castPoint.position, Quaternion.identity);
//         Rigidbody rb = currentBobber.GetComponent<Rigidbody>();
//         rb.AddForce(castPoint.forward * 6f + Vector3.up * 2f, ForceMode.Impulse);

//         // Start auto-reset timer
//         resetCoroutine = StartCoroutine(AutoResetRoutine());
//     }

//     private IEnumerator AutoResetRoutine()
//     {
//         float delay = Random.Range(5f, 10f);
//         yield return new WaitForSeconds(delay);
//         ResetFishing(); // Auto-reset without showing fish
//     }

//     // Call this via PlayerInput (wire in Unity Inspector)
//     public void RetrieveFish()
//     {
//         if (isFishing && fish1 != null)
//         {
//             fish1.SetActive(true);
//             Debug.Log("Fish caught and revealed!");
//         }
//         ResetFishing();
//     }

//     private void ResetFishing()
//     {
//         // Cleanup coroutine
//         if (resetCoroutine != null)
//         {
//             StopCoroutine(resetCoroutine);
//             resetCoroutine = null;
//         }

//         // Reset state
//         isFishing = false;
        
//         // Destroy bobber
//         if (currentBobber != null)
//             Destroy(currentBobber);
            
//         // Show follow-bobber
//         if (followBobber != null)
//             followBobber.SetActive(true);

//         // **Make fish invisible again when resetting**
//         if (fish1 != null)
//             fish1.SetActive(false);
//     }
// }

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

//     void Awake()
//     {
//         // Make fish invisible at start
//         fish1.SetActive(false);
//     }
// }
using UnityEngine;
using System.Collections;

public class FishingManager : MonoBehaviour
{
    [Header("Bobber Setup")]
    public Transform castPoint;
    public GameObject bobberPrefab;
    public GameObject followBobber;
    
    [Header("Fish Setup")]
    public GameObject fish1; // Will be found at runtime

    private GameObject currentBobber;
    private bool isFishing = false;
    private Coroutine resetCoroutine;

    void Awake()
    {
        // Find fish1 if not assigned in Inspector
        if (fish1 == null)
        {
            fish1 = GameObject.Find("fish1");
            Debug.Log("Looking for fish1: " + (fish1 != null ? "Found" : "Not found"));
        }
        
        // Add null check before trying to access fish1
        if (fish1 != null)
        {
            fish1.SetActive(false);
            Debug.Log("Fish1 hidden successfully");
        }
        else
        {
            Debug.LogError("Fish1 not found! Make sure it's named exactly 'fish1'");
        }
    }

    
    // Method called after timer from Cast
    private IEnumerator ResetFishingAfterDelay()
    {
        float delay = Random.Range(5f, 10f);
        yield return new WaitForSeconds(delay);
        ResetFishing(true); // Pass true to indicate it came from Cast
    }
    
    public void Cast()
    {
        if (isFishing) return;
        isFishing = true;

        if (followBobber != null)
            followBobber.SetActive(false);

        currentBobber = Instantiate(bobberPrefab, castPoint.position, Quaternion.identity);
        Rigidbody rb = currentBobber.GetComponent<Rigidbody>();
        rb.AddForce(castPoint.forward * 6f + Vector3.up * 2f, ForceMode.Impulse);

        resetCoroutine = StartCoroutine(AutoResetRoutine());
    }

    public void ResetFishing(bool fromCast = false)
    {
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }

        isFishing = false;
        if (currentBobber != null) Destroy(currentBobber);
        if (followBobber != null) followBobber.SetActive(true);
        
        // Only show fish when called from Cast
        if (fromCast && fish1 != null)
        {
            fish1.SetActive(true);
            Debug.Log("Fish1 made visible by Cast");
        }
    }
}


//     private IEnumerator AutoResetRoutine()
//     {
//         float delay = Random.Range(5f, 10f);
//         yield return new WaitForSeconds(delay);
        
//         // Show fish and reset fishing
//         if (fish1 != null)
//         {
//             fish1.SetActive(true);
//         }
        
//         ResetFishing(false); // Don't show fish again - it's already shown
//     }

//     // Call this from PlayerInput for manual resets
//     public void RetrieveFish()
//     {
//         ResetFishing(true); // Show fish when manually retrieved
//     }

//     public void ResetFishing(bool showFish = false)
//     {
//         // Cancel pending reset
//         if (resetCoroutine != null)
//         {
//             StopCoroutine(resetCoroutine);
//             resetCoroutine = null;
//         }

//         // Reset state
//         isFishing = false;
        
//         // Clean up bobber
//         if (currentBobber != null)
//             Destroy(currentBobber);
            
//         // Show follow bobber
//         if (followBobber != null)
//             followBobber.SetActive(true);
            
//         // Show fish if requested (for manual retrieves)
//         if (showFish && fish1 != null)
//         {
//             fish1.SetActive(true);
//         }
//     }
// }
