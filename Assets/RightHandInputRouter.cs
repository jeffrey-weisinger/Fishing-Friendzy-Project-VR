using UnityEngine;


public class RightHandInputRouter : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor interactor;

    public void OnGrab()
    {
        Debug.Log("A button pressed!");

        // Use the first selected interactable, if any
        var interactable = interactor.interactablesSelected.Count > 0
            ? interactor.interactablesSelected[0]
            : null;

        if (interactable != null)
        {
            interactor.interactionManager.SelectEnter(
                (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)interactor,
                (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)interactable
            );
        }
    }
}
