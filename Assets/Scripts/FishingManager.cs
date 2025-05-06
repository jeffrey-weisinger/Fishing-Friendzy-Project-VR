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

    [Header("Line Renderer Setup")]
    public LineRenderer fishingLine; // Assign your Line Renderer component here in the Inspector
    public Transform rodTip;         // Assign the actual tip of the fishing rod here


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

        // Ensure the Line Renderer is assigned
        if (fishingLine == null)
        {
            Debug.LogError("Fishing Line Renderer not assigned in the Inspector!");
            // Optionally, try to get it if it's on the same GameObject
            // fishingLine = GetComponent<LineRenderer>();
            // if (fishingLine == null) enabled = false; // Disable script if still not found
        }
        else
        {
            fishingLine.enabled = false; // Start with  line hidden
            fishingLine.positionCount = 2; // Rod tip and bobber
        }

        if (rodTip == null)
        {
            Debug.LogError("Rod Tip Transform not assigned in the Inspector!");
            // You might want to disable the script or provide a default if critical
            // enabled = false;
        }
    }

    void Update()
    {
        // If fishing and the bobber exists, update the line's end point
        if (isFishing && currentBobber != null && fishingLine != null && fishingLine.enabled)
        {
            fishingLine.SetPosition(0, rodTip.position); // Start of the line is always the rod tip
            fishingLine.SetPosition(1, currentBobber.transform.position); // End of the line follows the bobber
        }
    }

    
    // // Method called after timer from Cast
    // private IEnumerator ResetFishingAfterDelay()
    // {
    //     float delay = Random.Range(5f, 10f);
    //     yield return new WaitForSeconds(delay);

    //     ResetFishing(true); // Pass true to indicate it came from Cast
    // }
    // private IEnumerator ResetFishingAfterDelay()
    // {
    //     // Wait a random time before fish bites
    //     float delay = Random.Range(5f, 10f);
    //     yield return new WaitForSeconds(delay);
        
    //     // Fish bite animation
    //     if (currentBobber != null)
    //     {
    //         // Store original position
    //         Vector3 originalPosition = currentBobber.transform.position;
            
    //         // Define how far down the bobber should go
    //         Vector3 bitePosition = originalPosition + new Vector3(0, -1.5f, 0);
            
    //         // Quick, dramatic pull down animation
    //         float biteDuration = 0.5f;
    //         float elapsedTime = 0;
            
    //         while (elapsedTime < biteDuration)
    //         {
    //             elapsedTime += Time.deltaTime;
    //             float t = elapsedTime / biteDuration;
                
    //             // Use easeIn curve for more dramatic effect
    //             float easedT = t * t;
    //             currentBobber.transform.position = Vector3.Lerp(originalPosition, bitePosition, easedT);
                
    //             // Update fishing line in real-time
    //             if (fishingLine != null && fishingLine.enabled)
    //             {
    //                 fishingLine.SetPosition(0, rodTip.position);
    //                 fishingLine.SetPosition(1, currentBobber.transform.position);
    //             }
                
    //             yield return null;
    //         }
            
    //         // Short pause with bobber underwater
    //         yield return new WaitForSeconds(0.3f);
            
    //         // Optional: Add a second, smaller tug for extra effect
    //         originalPosition = currentBobber.transform.position;
    //         bitePosition = originalPosition + new Vector3(0, -0.5f, 0);
            
    //         elapsedTime = 0;
    //         biteDuration = 0.2f;
            
    //         while (elapsedTime < biteDuration)
    //         {
    //             elapsedTime += Time.deltaTime;
    //             float t = elapsedTime / biteDuration;
    //             currentBobber.transform.position = Vector3.Lerp(originalPosition, bitePosition, t);
                
    //             if (fishingLine != null && fishingLine.enabled)
    //             {
    //                 fishingLine.SetPosition(0, rodTip.position);
    //                 fishingLine.SetPosition(1, currentBobber.transform.position);
    //             }
                
    //             yield return null;
    //         }
    //     }
        
    //     // Now catch the fish
    //     ResetFishing(true);
    // }
    // private IEnumerator ResetFishingAfterDelay()
    // {
    //     // Initial wait time - adding 2 more seconds as requested
    //     float delay = Random.Range(7f, 12f); // Increased from 5-10 seconds to 7-12 seconds
    //     yield return new WaitForSeconds(delay);
        
    //     // Fish bite animation
    //     if (currentBobber != null)
    //     {
    //         // Store original position
    //         Vector3 originalPosition = currentBobber.transform.position;
            
    //         // Define how far down the bobber should go
    //         Vector3 bitePosition = originalPosition + new Vector3(0, -1.5f, 0);
            
    //         // Quick, dramatic pull down animation
    //         float biteDuration = 0.5f;
    //         float elapsedTime = 0;
            
    //         while (elapsedTime < biteDuration)
    //         {
    //             elapsedTime += Time.deltaTime;
    //             float t = elapsedTime / biteDuration;
                
    //             // Use easeIn curve for more dramatic effect
    //             float easedT = t * t;
    //             currentBobber.transform.position = Vector3.Lerp(originalPosition, bitePosition, easedT);
                
    //             // Update fishing line in real-time
    //             if (fishingLine != null && fishingLine.enabled)
    //             {
    //                 fishingLine.SetPosition(0, rodTip.position);
    //                 fishingLine.SetPosition(1, currentBobber.transform.position);
    //             }
                
    //             yield return null;
    //         }
            
    //         // Short pause with bobber underwater
    //         yield return new WaitForSeconds(0.3f);
            
    //         // Optional: Add a second, smaller tug for extra effect
    //         originalPosition = currentBobber.transform.position;
    //         bitePosition = originalPosition + new Vector3(0, -0.5f, 0);
            
    //         elapsedTime = 0;
    //         biteDuration = 0.2f;
            
    //         while (elapsedTime < biteDuration)
    //         {
    //             elapsedTime += Time.deltaTime;
    //             float t = elapsedTime / biteDuration;
    //             currentBobber.transform.position = Vector3.Lerp(originalPosition, bitePosition, t);
                
    //             if (fishingLine != null && fishingLine.enabled)
    //             {
    //                 fishingLine.SetPosition(0, rodTip.position);
    //                 fishingLine.SetPosition(1, currentBobber.transform.position);
    //             }
                
    //             yield return null;
    //         }
            
    //         // Add a longer 1-second delay after the bite as requested
    //         yield return new WaitForSeconds(1.0f);
    //     }
        
    //     // Now catch the fish
    //     ResetFishing(true);
    // }

    private IEnumerator ResetFishingAfterDelay()
    {
        // Initial wait time
        float delay = Random.Range(7f, 12f);
        yield return new WaitForSeconds(delay);
        
        // Fish bite animation
        if (currentBobber != null)
        {
            // Store original position
            Vector3 originalPosition = currentBobber.transform.position;
            
            // Define how far down the bobber should go
            Vector3 bitePosition = originalPosition + new Vector3(0, -1.5f, 0);
            
            // Slightly longer, smoother pull down animation
            float biteDuration = 0.65f; // Increased from 0.5f for smoother motion
            float elapsedTime = 0;
            
            while (elapsedTime < biteDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / biteDuration);
                
                // Use smooth step function for smoother start and end
                float easedT = t * t * (3f - 2f * t); // Smoothstep function
                currentBobber.transform.position = Vector3.Lerp(originalPosition, bitePosition, easedT);
                
                // Update fishing line in real-time
                if (fishingLine != null && fishingLine.enabled)
                {
                    fishingLine.SetPosition(0, rodTip.position);
                    fishingLine.SetPosition(1, currentBobber.transform.position);
                }
                
                yield return null;
            }
            
            // Short pause with bobber underwater
            yield return new WaitForSeconds(0.3f);
            
            // Second smaller tug
            originalPosition = currentBobber.transform.position;
            bitePosition = originalPosition + new Vector3(0, -0.5f, 0);
            
            elapsedTime = 0;
            biteDuration = 0.2f;
            
            while (elapsedTime < biteDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / biteDuration;
                currentBobber.transform.position = Vector3.Lerp(originalPosition, bitePosition, t);
                
                if (fishingLine != null && fishingLine.enabled)
                {
                    fishingLine.SetPosition(0, rodTip.position);
                    fishingLine.SetPosition(1, currentBobber.transform.position);
                }
                
                yield return null;
            }
            
            // Longer delay after the bite
            yield return new WaitForSeconds(0.3f);
        }
        
        // Now catch the fish
        ResetFishing(true);
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

        // Enable and set up the fishing line
        if (fishingLine != null && rodTip != null)
        {
            fishingLine.enabled = true;
            fishingLine.SetPosition(0, rodTip.position);
            fishingLine.SetPosition(1, currentBobber.transform.position); // Initial position of the bobber
        }
        else
        {
            Debug.LogError("Cannot draw fishing line - LineRenderer or RodTip not set!");
        }
        //set a slerp timer. 
        //call the coroutine function.

        // Define the duration of the slerp
        float lerpDuration = 10.0f;

        // Store the initial rotation of the bobber
        Vector3 start = new Vector3(0, -1f, 0);

        // Define the target rotation (using identity as an example)
        Vector3 end = new Vector3(0, 15f, 0);

        // Start the coroutine for smooth rotation transition
        // StartCoroutine(LerpBobberRotation(currentBobber, start, end, lerpDuration));
        StartCoroutine(BobberBobbingMotion(currentBobber, 0.3f, 2.0f, lerpDuration));

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

        // Disable the fishing line
        if (fishingLine != null)
        {
            fishingLine.enabled = false;
        }
        
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
            if (fishes != null && fishes.Count > 0)
            {
                fishes[CatchFish()].SetActive(true);
            }
            else
            {
                Debug.LogWarning("Fishes list is not set up or empty.");
            }

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

    //slerp function
    // private IEnumerator LerpBobberRotation(GameObject bobber, Vector3 start, Vector3 end, float duration)
    // {
    //     Debug.Log("Slerp");
    //     float startTime = Time.time;
    //     Debug.Log(Time.time - startTime);
    //     Debug.Log(Mathf.Abs(Time.time - startTime) < duration);

    //     while (Mathf.Abs(Time.time - startTime) < duration)
    //     {
    //         Debug.Log("About to Lerp");
    //         float t = (Time.time - startTime) / duration;
    //         bobber.transform.position = Vector3.Lerp(start, end, t);
    //     }
    //     Debug.Log(Time.time - startTime);
    //     Debug.Log(Mathf.Abs(Time.time - startTime) < duration);
    //     return null;

    // }


//working better now.
//     private IEnumerator LerpBobberRotation(GameObject bobber, Vector3 start, Vector3 end, float duration)
// {

//     yield return new WaitForSeconds(3);
//     float startTime = Time.time;
//     float elapsedTime = 0f;
    
//     // Get the initial position of the bobber
//     Vector3 initialPosition = bobber.transform.position;
    
//     while (elapsedTime < duration)
//     {
//         elapsedTime = Time.time - startTime;
//         float t = Mathf.Clamp01(elapsedTime / duration); // Clamp between 0 and 1
        
//         // Apply bobbing motion to the current position
//         Vector3 offset = Vector3.Lerp(start, end, t/1000);
//         bobber.transform.position = initialPosition + offset;
        
//         yield return null; // This is critical - it returns control to Unity for a frame
//     }
    
//     // Ensure we end at the exact final position
//     bobber.transform.position = initialPosition + end;
// }


// private IEnumerator BobberBobbingMotion(GameObject bobber, float amplitude, float frequency, float duration)
// {
//     yield return new WaitForSeconds(3); // Initial delay before bobbing starts
    
//     float startTime = Time.time;
//     float elapsedTime = 0f;
    
//     // Store initial position to apply offsets to
//     Vector3 initialPosition = bobber.transform.position;
    
//     while (elapsedTime < duration)
//     {
//         elapsedTime = Time.time - startTime;
        
//         // Create a sine wave motion for natural bobbing
//         float yOffset = amplitude * Mathf.Sin(frequency * elapsedTime);
        
//         // Apply only the y-axis movement to create bobbing
//         Vector3 newPosition = initialPosition;
//         newPosition.y = initialPosition.y + yOffset;
        
//         bobber.transform.position = newPosition;
        
//         yield return null; // Return control to Unity
//     }
// }

private IEnumerator BobberBobbingMotion(GameObject bobber, float amplitude, float frequency, float duration)
{
    yield return new WaitForSeconds(3); // Initial delay before bobbing starts
    
    float startTime = Time.time;
    float elapsedTime = 0f;
    
    // Store initial position to apply offsets to
    Vector3 initialPosition = bobber.transform.position;
    
    while (elapsedTime < duration)
    {
        elapsedTime = Time.time - startTime;
        
        // Create a sine wave motion for natural bobbing
        float yOffset = amplitude * Mathf.Sin(frequency * elapsedTime);
        
        // Apply only the y-axis movement to create bobbing
        Vector3 newPosition = initialPosition;
        newPosition.y = initialPosition.y + yOffset;
        
        bobber.transform.position = newPosition;
        
        // Update the fishing line here instead of in Update()
        if (fishingLine != null && fishingLine.enabled)
        {
            fishingLine.SetPosition(0, rodTip.position);
            fishingLine.SetPosition(1, bobber.transform.position);
        }
        
        yield return null; // Return control to Unity
    }
}

}
