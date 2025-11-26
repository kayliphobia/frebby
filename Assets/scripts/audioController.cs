using UnityEngine;
using System.Collections;
using System;

public class AudioController : MonoBehaviour
{
    [Header("Ambience")]
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioClip ambienceLoop;

    [Header("Random One-Shots")]
    [SerializeField] private AudioSource sfxSource;   // must NOT be looping
    [SerializeField] private AudioClip[] randomClips;

    [Header("Timing")]
    [SerializeField] private float minDelay = 5f;
    [SerializeField] private float maxDelay = 12f;

    [Header("plush")]
    [SerializeField] private AudioSource plushSource;
    [SerializeField] private AudioClip plushSound;

    public static Action PauseAudio; 
    public static Action ResumeAudio;

    private void Start()
    {
        // Start ambience
        if (ambienceSource && ambienceLoop)
        {
            ambienceSource.loop = true;
            ambienceSource.clip = ambienceLoop;
            ambienceSource.Play();
        }

        // Start random SFX loop
        if (randomClips.Length > 0)
            StartCoroutine(PlayRandomSounds());
        PauseAudio += PauseAllSounds;
        ResumeAudio += ResumeAllSounds;
    }

    private IEnumerator PlayRandomSounds()
    {
        while (true)
        {
            // Wait a random amount of time
            float delay = UnityEngine.Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            // Play a random clip
            AudioClip clip = randomClips[UnityEngine.Random.Range(0, randomClips.Length)];
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PauseAllSounds()
    {
        AudioSource[] sources = GameObject.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource src in sources)
            src.Pause();
    }


    public void ResumeAllSounds()
    {
        AudioSource[] sources = GameObject.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource src in sources)
            src.UnPause();
    }

    public void plushTrapSound()
    {
        plushSource.PlayOneShot(plushSound);
    }

    public void OnDestroy(){
        PauseAudio -= PauseAllSounds;
        ResumeAudio -= ResumeAllSounds;
    }

}
