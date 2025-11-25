using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DayEndResume : MonoBehaviour
{
    public void Resume()
    {   
        gameObject.SetActive(false);
        if (GameManager.gameOver)
        {
            return;
        }
        Time.timeScale = 1f;
        AudioController.ResumeAudio?.Invoke();
        Debug.Log("everything resumed");
    }
}
