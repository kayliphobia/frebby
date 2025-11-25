using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DayEndResume : MonoBehaviour
{
    public void Resume()
    {
        Time.timeScale = 1f;
        AudioController.ResumeAudio?.Invoke();
        gameObject.SetActive(false);
        Debug.Log("everything resumed");
    }
}
