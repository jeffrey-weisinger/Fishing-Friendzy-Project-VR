// This script uses code from ChatGPT and Perplexity.
using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class FishingManager : MonoBehaviour
{
    // Fish prefabs for inventory
    public List<GameObject> fishes;
   
    // Inventory slot references
    public List<Transform> inventorySlots = new List<Transform>();
   
    // Track how many fish we've caught
    public int fishCounter = 0;

    [Header("Popup")]
    public GameObject victoryPopup; 
    [Header("Effects")]
    public GameObject waterSplashPrefab;
    public GameObject celebrationEffectPrefab; // Add this line
    public GameObject victoryPopupPrefab;      // assign your new prefab
    public GameObject boat;  
    [Header("Bobber Setup")]
    public Transform castPoint;
    public GameObject bobberPrefab;
    public GameObject followBobber;
   
    [Header("Fish Setup")]
    public GameObject fish1;



    [Header("Line Renderer Setup")]
    public LineRenderer fishingLine;
    public Transform rodTip;




    // Private variables
    private GameObject currentBobber;
    private bool isFishing = false;
    private Coroutine fishingSequenceCoroutine;
    private Coroutine bobbingCoroutine;
    private Coroutine lineUpdateCoroutine;




    void Awake()
    {
        victoryPopup.SetActive(false); // keep it hidden at first

        // Find fish1 if not assigned in Inspector
        if (fish1 == null)
        {
            fish1 = GameObject.Find("fish1");
            Debug.Log("Looking for fish1: " + (fish1 != null ? "Found" : "Not found"));
        }
       
        if (fish1 != null)
        {
            fish1.SetActive(false);
            Debug.Log("Fish1 hidden successfully");
        }
        else
        {
            Debug.LogError("Fish1 not found! Make sure it's named exactly 'fish1'");
        }




        // Check fishing line setup
        if (fishingLine == null)
        {
            Debug.LogError("Fishing Line Renderer not assigned in the Inspector!");
        }
        else
        {
            fishingLine.enabled = false;
            fishingLine.positionCount = 2;
        }




        if (rodTip == null)
        {
            Debug.LogError("Rod Tip Transform not assigned in the Inspector!");
        }
    }




    // Start fishing when cast is called
    public void Cast()
    {
        if (isFishing) return;
        isFishing = true;




        if (followBobber != null)
            followBobber.SetActive(false);




        // Create bobber and apply initial force
        currentBobber = Instantiate(bobberPrefab, castPoint.position, Quaternion.identity);
        Rigidbody rb = currentBobber.GetComponent<Rigidbody>();
        rb.AddForce(castPoint.forward * 6f + Vector3.up * 2f, ForceMode.Impulse);




        // Set up and enable the fishing line
        if (fishingLine != null && rodTip != null)
        {
            fishingLine.enabled = true;
            // Start continuously updating the line
            lineUpdateCoroutine = StartCoroutine(UpdateFishingLine());
        }
        else
        {
            Debug.LogError("Cannot draw fishing line - LineRenderer or RodTip not set!");
        }




        // Start the complete fishing sequence
        fishingSequenceCoroutine = StartCoroutine(CompleteFishingSequence());
    }




    // Continuously update the fishing line to prevent shakiness
private IEnumerator UpdateFishingLine()
{
    while (isFishing && currentBobber != null)
    {
        yield return new WaitForEndOfFrame();
        fishingLine.SetPosition(0, rodTip.position);
        fishingLine.SetPosition(1, currentBobber.transform.position);
    }
}
    // Main fishing sequence
    private IEnumerator CompleteFishingSequence()
    {
        // Wait for bobber to settle
        yield return new WaitForSeconds(2.3f);
       
        if (currentBobber != null)
        {
            // Store base position after settling
            GameObject splashEffect = Instantiate(waterSplashPrefab, currentBobber.transform.position, Quaternion.identity);
            splashEffect.GetComponent<ParticleSystem>().Play();
            Destroy(splashEffect, 2f); // Destroy after effect finishes

            Vector3 basePosition = currentBobber.transform.position;
           
            // Start bobbing motion
            bobbingCoroutine = StartCoroutine(BobberBobbingMotion(currentBobber, 0.4f, 1.5f, basePosition));
           
            // Wait random time before fish bites
            float waitForBite = Random.Range(7f, 12f);
            yield return new WaitForSeconds(waitForBite);
           
            // Stop bobbing when fish bites
            if (bobbingCoroutine != null)
            {
                StopCoroutine(bobbingCoroutine);
                bobbingCoroutine = null;
            }
           
            // Get current position for bite animation
            Vector3 startPosition = currentBobber.transform.position;
           
            // Quick downward motion - fish bite
            Vector3 downPosition = startPosition + new Vector3(0, -0.6f, 0);
            yield return StartCoroutine(MoveBobber(startPosition, downPosition, 0.4f));
           
            // Pause while fish is "caught"
            yield return new WaitForSeconds(1.0f);
           
            // Quick upward motion
            Vector3 upPosition = startPosition + new Vector3(0, 0.5f, 0);
            yield return StartCoroutine(MoveBobber(downPosition, upPosition, 0.3f));
           
            // Brief pause before finishing
            yield return new WaitForSeconds(0.2f);
           
            // Add fish to inventory
            if (fishes != null && fishes.Count > 0 && fishCounter < inventorySlots.Count)
            {
                int fishIndex = Random.Range(0, fishes.Count);
                GameObject caughtFish = Instantiate(fishes[fishIndex], inventorySlots[fishCounter]);
                caughtFish.transform.localPosition = Vector3.zero;
                fishCounter++;
            }
            // if (fishCounter >= 1) // Change to your max fish count
            // {
            //     // Play celebration effect
            //     GameObject celebrationEffect = Instantiate(celebrationEffectPrefab, currentBobber.transform.position, Quaternion.identity);
            //     celebrationEffect.GetComponent<ParticleSystem>().Play();
            //     Destroy(celebrationEffect, 3f); // Adjust time as needed
            // }
        if (fishCounter >= 1)
        {
            ShowVictoryPopup();
            // 1) Decide how far and in what direction from the camera you want it
            var cam = Camera.main; 
            float distanceInFront = 6f;       // adjust as needed
            float heightAbove     = 10f;       // adjust as needed

            // 2) Compute the world‐space spawn point
            Vector3 spawnPos = cam.transform.position
                            + cam.transform.forward * distanceInFront
                            + Vector3.up           * heightAbove;

            // 3) Instantiate and play
            var effect = Instantiate(
                celebrationEffectPrefab,
                spawnPos,
                Quaternion.LookRotation(cam.transform.forward)
            );
            var ps = effect.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();

            // 4) Clean up
            Destroy(effect, 3f);
        }

        }
       
        // Reset fishing state
        ResetFishing();
    }
   
    // Smooth bobbing motion for the bobber
    private IEnumerator BobberBobbingMotion(GameObject bobber, float amplitude, float frequency, Vector3 basePosition)
    {
        // Adjust starting position to be at the top of the bob cycle
        Vector3 bobCenter = new Vector3(basePosition.x, basePosition.y + amplitude/2, basePosition.z);
        float startTime = Time.time;
       
        while (isFishing && bobber != null)
        {
            float elapsed = Time.time - startTime;
           
            // Use cosine wave which starts high and goes down first
            float yOffset = amplitude * Mathf.Cos(frequency * elapsed);
           
            Vector3 newPosition = bobCenter;
            newPosition.y = bobCenter.y - amplitude/2 + yOffset;
           
            bobber.transform.position = newPosition;
            yield return null;
        }
    }
   
    // Smooth movement between two points
    private IEnumerator MoveBobber(Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;
       
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = t * t * (3f - 2f * t); // Smooth step easing
           
            currentBobber.transform.position = Vector3.Lerp(start, end, easedT);
            yield return null;
        }
       
        // Ensure we reach the exact end position
        currentBobber.transform.position = end;
    }
   
    // Clean up everything
    public void ResetFishing()
    {
        if (fishingSequenceCoroutine != null)
            StopCoroutine(fishingSequenceCoroutine);
           
        if (bobbingCoroutine != null)
            StopCoroutine(bobbingCoroutine);
           
        if (lineUpdateCoroutine != null)
            StopCoroutine(lineUpdateCoroutine);




        isFishing = false;
       
        if (currentBobber != null)
            Destroy(currentBobber);
           
        if (followBobber != null)
            followBobber.SetActive(true);




        if (fishingLine != null)
            fishingLine.enabled = false;
    }
   
    // Utility method for randomly selecting a fish
    public int CatchFish()
    {
        return Random.Range(0, fishes.Count-1);
    }

    // void ShowVictoryPopup()
    // {
    //     // compute spawn position just above the boat
    //     Vector3 spawnPos = boat.transform.position + Vector3.up * 2f;
        
    //     // instantiate and orient so it faces the camera
    //     var popup = Instantiate(victoryPopupPrefab, spawnPos, Quaternion.identity);
    //     popup.transform.LookAt(Camera.main.transform);
    //     popup.transform.Rotate(0,180,0); // because LookAt faces its back toward camera
        
    //     Destroy(popup, 4f);
    // }

    void ShowVictoryPopup()
    {
        victoryPopup.SetActive(true);
        Invoke(nameof(HideVictoryPopup), 4f);  // hide after 4 seconds
    }

    void HideVictoryPopup()
    {
        victoryPopup.SetActive(false);
    }
}






