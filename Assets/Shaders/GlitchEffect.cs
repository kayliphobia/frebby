using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GlitchEffect : MonoBehaviour
{
    public Image glitchOverlay;
    public float glitchDuration = 0.15f;
    public float fadeInSpeed = 0.1f;
    public float fadeOutSpeed = 0.15f;

    public static Action GlitchEvent;


    public void Start()
    {
        GlitchEvent += TriggerGlitch;
    }

    public void TriggerGlitch()
    {
        StartCoroutine(GlitchRoutine());
    }

    IEnumerator GlitchRoutine()
    {
        // Fade In
        float t = 0.5f;
        Color c = glitchOverlay.color;

        while (c.a < 1f)
        {
            t += Time.deltaTime / fadeInSpeed;
            c.a = Mathf.Lerp(0f, 1f, t);
            glitchOverlay.color = c;
            yield return null;
        }

        // Hold it for the duration
        yield return new WaitForSeconds(glitchDuration);

        // Fade Out
        t = 0.2f;

        while (c.a > 0.2f)
        {
            t += Time.deltaTime / fadeOutSpeed;
            c.a = Mathf.Lerp(1f, 0f, t);
            glitchOverlay.color = c;
            yield return null;
        }
    }

    void OnDestroy()
    {
        GlitchEvent -= TriggerGlitch;
    }
}
