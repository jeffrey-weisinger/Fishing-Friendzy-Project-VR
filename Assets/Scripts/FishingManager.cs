// This script uses code from ChatGPT and Perplexity.
using UnityEngine; 
using System.Collections;
using System.Collections.Generic;

public class FishingManager : MonoBehaviour
{
    //list of prefabs
    public List<GameObject> fishes;

    //list of inventory
    //??
    public List<Transform> inventorySlots = new List<Transform>(); // References to the slot objects

    //counter object
    public int fishCounter = 0;

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

        resetCoroutine = StartCoroutine(ResetFishingAfterDelay());
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
        //if (fromCast && fish1 != null)
        if (fromCast)
        {
            //fish1.SetActive(true);
            Debug.Log("Fish1 made visible by Cast");
            
            //randomizes fish index
            int fishIndex = CatchFish();
            GameObject currentFish = Instantiate(fishes[fishIndex], inventorySlots[fishCounter]);
            //add random fish to slot.
            fishCounter += 1;

            // fishes[CatchFish()].SetActive(true);
        }
    }

    public int CatchFish()
    {
        int fish_index = Random.Range(0, fishes.Count-1);
        return fish_index;

    }
    //use instantiate call 
        //give parent parameter 
        //spawn in local space (0, 0, 0 -- relative to the parent)

    //use dictionary for internal space management 
}
