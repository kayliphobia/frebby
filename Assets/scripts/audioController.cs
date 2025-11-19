using UnityEngine;
using System.Collections;

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
    }

    private IEnumerator PlayRandomSounds()
    {
        while (true)
        {
            // Wait a random amount of time
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            // Play a random clip
            AudioClip clip = randomClips[Random.Range(0, randomClips.Length)];
            sfxSource.PlayOneShot(clip);
        }
    }
}
