using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHidingSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button hideButton;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Image vignetteImage; // fade + breathing

    [SerializeField] private Image hideImage; // fade + breathing

    [Header("Settings")]
    [SerializeField] private float cameraDownOffset = -1.2f;
    [SerializeField] private float transitionDuration = 0.8f;
    [SerializeField] private float breathingSpeed = 1.5f;
    [SerializeField] private float breathingIntensity = 0.1f;

    private bool isHiding = false;
    private Vector3 originalCameraPos;
    private Coroutine transitionRoutine;
    private Color baseColor;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        originalCameraPos = cameraTransform.localPosition;

        if (vignetteImage != null)
        {
            baseColor = vignetteImage.color;
            vignetteImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        }

        hideButton.onClick.AddListener(ToggleHide);
    }

    void Update()
    {
        // breathing vignette pulse while hiding
        if (isHiding && vignetteImage != null)
        {
            float alphaOffset = Mathf.Sin(Time.time * breathingSpeed) * breathingIntensity;
            Color c = vignetteImage.color;
            c.a = Mathf.Clamp01(0.6f + alphaOffset);
            vignetteImage.color = c;
        }
    }

    private void ToggleHide()
    {
        isHiding = !isHiding;

        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(HideTransition(isHiding));
    }

    private IEnumerator HideTransition(bool hide)
    {
        Vector3 startPos = cameraTransform.localPosition;
        Vector3 endPos = hide ? originalCameraPos + new Vector3(0, cameraDownOffset, 0) : originalCameraPos;

        float startAlpha = vignetteImage != null ? vignetteImage.color.a : 0f;
        float endAlpha = hide ? 0.8f : 0f;

        float time = 0f;
        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = time / transitionDuration;

            // camera move
            cameraTransform.localPosition = Vector3.Lerp(startPos, endPos, t);


            if (hideImage != null)
                if (t <= transitionDuration)
                {
                    hideImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, Mathf.Lerp(0f, 4f, t * 2f));
                }
                else
                {
                    hideImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, Mathf.Lerp(2f, 0f, t - transitionDuration));
                }
                
            // fade vignette smoothly during transition
            if (vignetteImage != null)
                vignetteImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, Mathf.Lerp(startAlpha, endAlpha, t));


            yield return null;
        }


        // ensure vignette ends at proper alpha
        if (vignetteImage != null)
            vignetteImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, endAlpha);
            
        if (hideImage != null)
            hideImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

    }
}
