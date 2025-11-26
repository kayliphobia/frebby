using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JumpscareAnimation : MonoBehaviour
{
    public float duration = 0.2f; // how fast the animation is

    public float holdDuration = 1f;

    public Room Jumpscare;

    public float finalScale = 1f;

    private RectTransform rectTransform;
    private Image image;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();


        if (rectTransform == null || image == null)
        {
            Debug.LogError("JumpscareAnimation requires an Image component on the same GameObject.");
            enabled = false;
            return;
        }

        rectTransform.localScale = Vector3.zero; // start hidden
    }

    public void Play()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleIn());
    }

    IEnumerator ScaleIn()
    {
        rectTransform.localScale = Vector3.zero;
        image.sprite = Jumpscare.GetCurrentImage();

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalized = elapsed / duration;

            // Smooth scale
            rectTransform.localScale = Vector3.Lerp(Vector3.zero, finalScale * Vector3.one, normalized);

            yield return null;
        }
        yield return new WaitForSeconds(holdDuration);
        rectTransform.localScale = finalScale * Vector3.one;
    }
}
