using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHidingSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button hideButton;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Image vignetteImage;
    [SerializeField] private Image hideImage;

    [Header("Settings")]
    [SerializeField] private float cameraDownOffset = -1.2f;
    [SerializeField] private float transitionDuration = 0.8f;
    [SerializeField] private float breathingSpeed = 1.5f;
    [SerializeField] private float breathingIntensity = 0.1f;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource sfxSource;       // one-shot sfx (already used)
    [SerializeField] private AudioClip hidingsfx;

    [SerializeField] private AudioSource heartbeatSource; // NEW – looping heartbeat source
    [SerializeField] private AudioClip heartbeatClip;     // NEW – heartbeat clip

    private bool isHiding = false;
    private Vector3 originalCameraPos;
    private Coroutine transitionRoutine;
    private Coroutine heartbeatRoutine;
    private Color baseColor;

    public bool IsHiding() => isHiding;

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

        if (heartbeatSource != null)
        {
            heartbeatSource.clip = heartbeatClip;
            heartbeatSource.loop = true;
            heartbeatSource.volume = 0f;
        }

        hideButton.onClick.AddListener(ToggleHide);
    }

    void Update()
    {
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

        // heartbeat fade logic
        if (heartbeatRoutine != null)
            StopCoroutine(heartbeatRoutine);

        heartbeatRoutine = StartCoroutine(FadeHeartbeat(isHiding));
    }

    private IEnumerator HideTransition(bool hide)
    {
        Vector3 startPos = cameraTransform.localPosition;
        Vector3 endPos = hide ? originalCameraPos + new Vector3(0, cameraDownOffset, 0) : originalCameraPos;

        float startAlpha = vignetteImage != null ? vignetteImage.color.a : 0f;
        float endAlpha = hide ? 0.8f : 0f;

        float time = 0f;

        sfxSource.PlayOneShot(hidingsfx);

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = time / transitionDuration;

            cameraTransform.localPosition = Vector3.Lerp(startPos, endPos, t);

            if (hideImage != null)
            {
                if (t <= transitionDuration)
                {
                    hideImage.color = new Color(baseColor.r, baseColor.g, baseColor.b,
                        Mathf.Lerp(0f, 4f, t * 2f));
                }
                else
                {
                    hideImage.color = new Color(baseColor.r, baseColor.g, baseColor.b,
                        Mathf.Lerp(2f, 0f, t - transitionDuration));
                }
            }

            if (vignetteImage != null)
                vignetteImage.color = new Color(baseColor.r, baseColor.g, baseColor.b,
                    Mathf.Lerp(startAlpha, endAlpha, t));

            yield return null;
        }

        if (vignetteImage != null)
            vignetteImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, endAlpha);

        if (hideImage != null)
            hideImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
    }

    // --- NEW: heartbeat fade coroutine ---
    private IEnumerator FadeHeartbeat(bool fadeIn)
    {
        if (heartbeatSource == null || heartbeatClip == null)
            yield break;

        if (fadeIn)
        {
            if (!heartbeatSource.isPlaying)
                heartbeatSource.Play();
        }

        float duration = 0.6f;
        float time = 0f;
        float start = heartbeatSource.volume;
        float end = fadeIn ? 0.6f : 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            heartbeatSource.volume = Mathf.Lerp(start, end, time / duration);
            yield return null;
        }

        heartbeatSource.volume = end;

        if (!fadeIn)
            heartbeatSource.Stop();
    }
}
