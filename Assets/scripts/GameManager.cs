using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField]
    private const float secondsPerHour = 30f;   // how long each hour lasts in real time
    private float hourTimer = 0f;
    private int currentHour = 9;
    private const int endHour = 17;             // 5PM

    [Header("Day Settings")]
    private int currentDay = 1;
    public const int maxDays = 5;               // optional limit

    [Header("UI References")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dayText;
    public GameObject gameOverUI;         // optional UI overlay for death screen

    [Header("Day Transition")]
    public Image dayStartTransitionBackground;
    public List<Sprite> dayStartTransitionBackgrounds;

    public AudioSource dayTransitionAudioSource;
    public AudioClip gameWinSound;
    private bool shiftActive = true;

    public SubtitlePlayer subtitlePlayer;
    public AudioClip voiceClip;
    public SubtitleData subtitleFile;

    public int getCurrentDay() => currentDay;
    public int getCurrentHour() => currentHour;

    public static Action ReturnToDesk;

    public GameObject dayEndAnimator;

    public GameObject gameWin;

    public static bool gameOver;

    void Start()
    {
        gameOver = false;
        currentDay = SaveSystem.LoadDay();
        UpdateUI();
        InitializeDay(currentDay);
    }

    void Update()
    {
        if (!shiftActive) return;

        hourTimer += Time.deltaTime;

        if (hourTimer >= secondsPerHour)
        {
            hourTimer = 0f;
            currentHour++;

            if (currentHour >= endHour)
            {
                currentHour = endHour;
                EndShift();
            }
            else
            {
                UpdateUI();
            }
        }
    }

    void InitializeDay(int day)
    {
        GameManager.ReturnToDesk?.Invoke();
        if (day == 1)
        {
            subtitlePlayer.PlayWithSubtitles(voiceClip, subtitleFile);
        }

        if (day >= 2)
        {
            GameObject leftAttackPos = GameObject.Find("LeftAttackPosition");

            if (leftAttackPos)
            {
                Transform steveCubicleTransform = leftAttackPos.transform.Find("SteveCubicle");
                if (steveCubicleTransform)
                {
                    Room room = steveCubicleTransform.GetComponent<Room>();
                    if (room)
                    {
                        room.occupiedRoomImages[0] = room.occupiedRoomImages[1];
                    }
                }
            }

        }
        FindFirstObjectByType<ProductivitySystem>(FindObjectsInactive.Include)?.ResetProductivity();

        UpdateUI();
        StartCoroutine(BeginDayStartTransition(4));
    }

    void EndShift()
    {
        Time.timeScale = 0f;
        AI[] animatronics = FindObjectsByType<AI>(FindObjectsSortMode.None);
        foreach (AI animatronic in animatronics) {
            animatronic.Reset();
        }
        AudioController.PauseAudio?.Invoke();
        dayTransitionAudioSource.PlayOneShot(gameWinSound);
        dayEndAnimator.SetActive(true);
        dayEndAnimator.GetComponent<Animator>().SetTrigger("DayEndTrigger");
        shiftActive = false;
        NextDay();
        SaveSystem.SaveDay(currentDay);
    }


    public void NextDay()
    {
        currentDay++;
        if (currentDay > maxDays)
        {
            gameOver = true;

            // Game complete or loop back
            currentDay = 1;
            StartCoroutine(ShowWinScreen(10));
            return;
        }

        // Reset shift
        currentHour = 9;
        hourTimer = 0f;
        shiftActive = true;
        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        InitializeDay(currentDay);
    }

    void UpdateUI()
    {
        // Convert currentHour (in 24-hour style counting) to 12-hour display
        int displayHour = currentHour;
        string ampm = "AM";

        if (displayHour >= 12)
        {
            ampm = "PM";
            if (displayHour > 12)
                displayHour -= 12;
        }

        if (timeText != null)
            timeText.text = $"{displayHour} {ampm}";

        if (dayText != null)
            dayText.text = $"DAY {currentDay}";
    }

    /// <summary>
    /// Handles game over state (called by animatronic scripts).
    /// </summary>
    public void TriggerGameOver(string cause)
    {
        gameOver = true;

        shiftActive = false;

        Debug.Log($"GAME OVER — Cause: {cause}");

        if (gameOverUI != null)
            gameOverUI.SetActive(true);
        gameOver = false;
        SceneManager.LoadScene("Title");
        
    }

    public IEnumerator BeginDayStartTransition(int duration) {
        if (currentDay > maxDays)
        {
            yield break;
        }
        dayStartTransitionBackground.sprite = dayStartTransitionBackgrounds[currentDay - 1];
        dayStartTransitionBackground.color = new Color(1, 1, 1, 1);
        float currentTime = 0;
        while (currentTime < duration) {
            dayStartTransitionBackground.color = new Color(1, 1, 1, (duration - currentTime) / duration);
            currentTime += Time.deltaTime;
            yield return null;
        }
        dayStartTransitionBackground.color = new Color(1, 1, 1, 0);

    }

    public IEnumerator ShowWinScreen(int duration)
    {
        gameWin.SetActive(true);
        yield return new WaitForSecondsRealtime(duration);
        TriggerGameOver("YOU SURVIVED CORPORATE HELL");
    }
    
}
