using UnityEngine;

public class ComputerClick : MonoBehaviour
{
    [Header("References")]
    public CameraMenuManager cameraMenuManager;
    private SpriteRenderer sr;

    [Header("Glow Settings")]
    [SerializeField] private float normalAlpha = 0.5f;
    [SerializeField] private float glowAlpha = 1f;
    [SerializeField] private float fadeSpeed = 5f;

    private bool isHovering = false;

    private void Awake()
    {
        // Get sprite on the same object
        sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            SetAlpha(normalAlpha);
    }

    void OnMouseDown()
    {
        if (cameraMenuManager != null)
            cameraMenuManager.OpenCamera();
    }

    private void OnMouseEnter()
    {
        isHovering = true;
    }

    private void OnMouseExit()
    {
        isHovering = false;
    }

    private void Update()
    {
        if (sr == null) return;

        float target = isHovering ? glowAlpha : normalAlpha;
        float newAlpha = Mathf.Lerp(sr.color.a, target, Time.deltaTime * fadeSpeed);
        SetAlpha(newAlpha);
    }

    private void SetAlpha(float a)
    {
        Color c = sr.color;
        c.a = a;
        sr.color = c;
    }
}
