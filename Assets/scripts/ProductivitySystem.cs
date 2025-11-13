using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductivitySystem : MonoBehaviour
{
    [Header("UI References")]
    public Slider productivityBar;
    public TextMeshProUGUI fakeTextOutput;

    [Header("Settings")]
    public float depletionRate = 1f; // per tick
    public float tickRate = 0.05f;   // 50 ms
    public float maxValue = 2000f;

    [Header("Debug")]
    public float currentValue;
    public bool windowActive = false;

    private GameManager gameManager;
    public float baseDepletionRate = 1f;   // Night 1 baseline
    public float dayMultiplier = 0.3f;     // +30% per additional day

    public MarionetteAI marionette;


    private string[] fakePhrases = {
        "Compiling report...", "Checking logs...", 
        "Processing data...", "Updating files...", 
        "Running diagnostics..."
    };

    void Start()
    {
        
        gameManager = FindFirstObjectByType<GameManager>();

        currentValue = maxValue;
        productivityBar.maxValue = maxValue;
        productivityBar.value = currentValue;

        InvokeRepeating(nameof(DepleteBar), tickRate, tickRate);
    }


    void Update()
    {
        // Always depleting in the background
        if (windowActive)
        {
            if (Input.anyKeyDown && !IsMouseInput())
            {
                currentValue = Mathf.Min(maxValue, currentValue + 10f);
                productivityBar.value = currentValue;

                string phrase = fakePhrases[Random.Range(0, fakePhrases.Length)];
                fakeTextOutput.text = phrase;
            }
        }

        // Always keep bar visually synced if visible
        if (windowActive && productivityBar != null)
        {
            productivityBar.value = currentValue;
        }
    }

    void DepleteBar()
    {
        int day = gameManager != null ? gameManager.getCurrentDay() : 1;
        float scaledDepletion = baseDepletionRate * (1f + (day - 1) * dayMultiplier);

        currentValue -= scaledDepletion;
        currentValue = Mathf.Max(0, currentValue);

        if (windowActive && productivityBar != null)
            productivityBar.value = currentValue;

        if (currentValue <= 0)
        {
            Debug.Log("Productivity dropped to 0!");
            marionette.OnProductivityDepleted();
        }
    }

    bool IsMouseInput()
    {
        return Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2);
    }

    public void SetWindowActive(bool active)
    {
        windowActive = active;
        if (!active && fakeTextOutput != null)
            fakeTextOutput.text = "";
    }
}
