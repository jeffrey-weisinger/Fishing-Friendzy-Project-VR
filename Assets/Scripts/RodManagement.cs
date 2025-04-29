// This script uses code from ChatGPT.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FishingRodManager : MonoBehaviour
{
    public Transform rodHolderAnchor;
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    public Rigidbody rb;

    private bool isHeld = false;

    void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        // Grab/drop events
        grabInteractable.selectEntered.AddListener(OnPickup);
        grabInteractable.selectExited.AddListener(OnDrop);
    }

    void OnDestroy()
    {
        // Unsubscribe
        grabInteractable.selectEntered.RemoveListener(OnPickup);
        grabInteractable.selectExited.RemoveListener(OnDrop);
    }

    void OnPickup(SelectEnterEventArgs args)
    {
        Debug.Log("Rod Picked Up");
        isHeld = true;
        // Ensure rb is not kinematic while held
        rb.isKinematic = false;
    }

    void OnDrop(SelectExitEventArgs args)
    {
        Debug.Log("Rod Dropped");
        isHeld = false;

        // --- Return to Holder Logic ---
        // Option A: Re-parent and snap back
        transform.SetParent(rodHolderAnchor); // Parent back to the holder
        transform.localPosition = Vector3.zero; // Reset local position
        transform.localRotation = Quaternion.identity; // Reset local rotation
        rb.isKinematic = true; // Make kinematic again so it doesn't fall
        rb.velocity = Vector3.zero; // Stop any residual movement
        rb.angularVelocity = Vector3.zero;

    }

    // Fishing action placeholder
    void Update()
    {
        if (isHeld)
        {

        }
    }

    void PerformFishingAction()
    {
        Debug.Log("Fishing Action Triggered!");
    }
}