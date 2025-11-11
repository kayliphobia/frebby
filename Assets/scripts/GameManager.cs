using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField]
    private const float secondsPerHour = 50f;   // how long each hour lasts in real time
    private float hourTimer = 0f;
    private int currentHour = 9;
    private const int endHour = 17;             // 5PM

    [Header("Day Settings")]
    private int currentDay = 1;
    public const int maxDays = 5;               // optional limit

    [Header("UI References")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dayText;
    public GameObject shiftCompleteUI;
    public GameObject gameOverUI;         // optional UI overlay for death screen

    private bool shiftActive = true;
    private bool isGameOver = false;

    public int getCurrentDay() => currentDay;

    void Start()
    {
        UpdateUI();
        InitializeDay(currentDay);
    }

    void Update()
    {
        if (!shiftActive || isGameOver) return;

        hourTimer += Time.deltaTime;

        if (hourTimer >= secondsPerHour)
        {
            hourTimer = 0f;
            currentHour++;

            if (currentHour > endHour)
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
        // Basic example of AI scaling per day
        // for (int i = 0; i < animatronicDifficulty.Length; i++)
        // {
        //     animatronicDifficulty[i] = Mathf.Clamp(day * baseDifficultyPerDay[i], 1, 20);
        // }

        UpdateUI();
    }

    void EndShift()
    {
        shiftActive = false;
        if (shiftCompleteUI != null)
            shiftCompleteUI.SetActive(true);
    }

    public void NextDay()
    {
        currentDay++;
        if (currentDay > maxDays)
        {
            // Game complete or loop back
            currentDay = 1;
        }

        // Reset shift
        currentHour = 9;
        hourTimer = 0f;
        shiftActive = true;
        isGameOver = false;
        if (shiftCompleteUI != null)
            shiftCompleteUI.SetActive(false);
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
        if (isGameOver) return;

        isGameOver = true;
        shiftActive = false;

        Debug.Log($"GAME OVER — Cause: {cause}");

        // Optional freeze and show game over UI
        Time.timeScale = 0f;
        if (gameOverUI != null)
            gameOverUI.SetActive(true);
    }
}
