using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popUp : MonoBehaviour
{
    public GameObject popupPanel;
    public float displayDuration = 8f; // how many seconds to show
    public float fadeDuration = 1f; // how long it takes to fade out

    private CanvasGroup canvasGroup;

    void Start()
    {
        // Add a CanvasGroup if not already there
        canvasGroup = popupPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = popupPanel.AddComponent<CanvasGroup>();
        }

        popupPanel.SetActive(true);
        canvasGroup.alpha = 1f; // fully visible

        // Start the hide process
        Invoke(nameof(StartFadeOut), displayDuration);
    }

    void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    System.Collections.IEnumerator FadeOut()
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        popupPanel.SetActive(false); // fully hide when done
    }

    // Optional manual show/hide if you still want them!
    public void ShowPopup()
    {
        popupPanel.SetActive(true);
        canvasGroup.alpha = 1f;
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false);
        canvasGroup.alpha = 0f;

        // script written with ChatGPT
    }
}
