// using UnityEngine;


// public class RightHandInputRouter : MonoBehaviour
// {
//     public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor interactor;

//     public void OnGrab()
//     {
//         Debug.Log("A button pressed!");

//         // Use the first selected interactable, if any
//         var interactable = interactor.interactablesSelected.Count > 0
//             ? interactor.interactablesSelected[0]
//             : null;

//         if (interactable != null)
//         {
//             interactor.interactionManager.SelectEnter(
//                 (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)interactor,
//                 (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)interactable
//             );
//         }
//     }
// }

// using UnityEngine;


// public class RightHandInputRouter : MonoBehaviour
// {
//     public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor interactor;

//     public void OnGrab()
//     {
//         Debug.Log("A button pressed!");

//         // Check what the ray is HOVERING OVER
//         var hoveredInteractable = interactor.interactablesHovered.Count > 0
//             ? interactor.interactablesHovered[0]
//             : null;

//         if (hoveredInteractable != null && hoveredInteractable is UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable selectableInteractable)
//         { 
//             // Cast to the correct interface type
//             interactor.interactionManager.SelectEnter(interactor, selectableInteractable);
//         }
//     }
// }


using UnityEngine;


public class RightHandInputRouter : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor interactor;
    private Quaternion originalRotation;
    private GameObject currentlyGrabbedObject;
    private bool justGrabbed = false;

    public void OnGrab()
    {
        Debug.Log("A button pressed!");

        // Check what the ray is HOVERING OVER
        var hoveredInteractable = interactor.interactablesHovered.Count > 0
            ? interactor.interactablesHovered[0]
            : null;
  
        if (hoveredInteractable != null && hoveredInteractable is UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable selectableInteractable)
        { 
            // Store the original rotation before grabbing
            GameObject interactableObject = (hoveredInteractable as Component)?.gameObject;
            if (interactableObject != null)
            {
                originalRotation = interactableObject.transform.rotation;
                currentlyGrabbedObject = interactableObject;
                justGrabbed = true;
            }

            // Cast to the correct interface type
            interactor.interactionManager.SelectEnter(interactor, selectableInteractable);
        }
    }

    void Update()
    {
        // If we just grabbed something and have a reference to it
        if (justGrabbed && currentlyGrabbedObject != null)
        {
            // Wait one frame for the grab to complete
            justGrabbed = false;
        }
        // Counter-rotate the object if it's still selected
        else if (currentlyGrabbedObject != null && interactor.hasSelection)
        {
            // Check if our object is still selected
            bool isOurObjectSelected = false;
            foreach (var selected in interactor.interactablesSelected)
            {
                GameObject selectedObject = (selected as Component)?.gameObject;
                if (selectedObject == currentlyGrabbedObject)
                {
                    isOurObjectSelected = true;
                    break;
                }
            }

            if (isOurObjectSelected)
            {
                // Apply the original rotation to counter the 180-degree flip
                currentlyGrabbedObject.transform.rotation = originalRotation;
            }
            else
            {
                // Clear the reference if it's no longer selected
                currentlyGrabbedObject = null;
            }
        }
    }
}
