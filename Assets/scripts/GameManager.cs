using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float secondsPerHour = 50f;   // how long each hour lasts in real time
    private float hourTimer = 0f;
    private int currentHour = 9;
    private int endHour = 17;             // 5PM

    [Header("Day Settings")]
    public int currentDay = 1;
    public int maxDays = 5;               // optional limit

    [Header("UI References")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dayText;
    public GameObject shiftCompleteUI;
    public GameObject gameOverUI;         // optional UI overlay for death screen

    [Header("AI Difficulty")]
    public int[] animatronicDifficulty;   // difficulty per animatronic
    public int[] baseDifficultyPerDay;    // overall AI scaling per day

    private bool shiftActive = true;
    private bool isGameOver = false;

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
        for (int i = 0; i < animatronicDifficulty.Length; i++)
        {
            animatronicDifficulty[i] = Mathf.Clamp(day * baseDifficultyPerDay[i], 1, 20);
        }

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

    // --- NEW FUNCTIONS BELOW ---

    /// <summary>
    /// Returns the AI level for a given animatronic name.
    /// Animatronic names correspond to index in animatronicDifficulty.
    /// </summary>
    public int GetAILevel(string animatronicName)
    {
        // Example naming convention: 0 = Steve, 1 = Marionette, etc.
        animatronicName = animatronicName.ToLower();

        int index = 0;
        switch (animatronicName)
        {
            case "steve": index = 0; break;
            case "marionette": index = 1; break;
            // add more animatronics here
            default: index = 0; break;
        }

        if (index >= 0 && index < animatronicDifficulty.Length)
            return animatronicDifficulty[index];
        else
            return 0;
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
