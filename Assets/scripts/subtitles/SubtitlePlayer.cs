using UnityEngine;
using TMPro;

public class SubtitlePlayer : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public SubtitleData subtitles;
    public TMP_Text subtitleText;

    private int currentIndex = 0;

    private void Start()
    {
        subtitleText.text = "";
    }

    private void Update()
    {
        if (!audioSource.isPlaying || subtitles == null || subtitles.lines.Length == 0)
            return;

        float t = audioSource.time;

        // If next subtitle line should appear
        if (currentIndex < subtitles.lines.Length && t >= subtitles.lines[currentIndex].time)
        {
            subtitleText.text = subtitles.lines[currentIndex].text;
            currentIndex++;
        }
    }

    public void PlayWithSubtitles(AudioClip clip, SubtitleData data)
    {
        subtitles = data;
        audioSource.clip = clip;
        audioSource.Play();
        currentIndex = 0;
        subtitleText.text = "";
    }
}
