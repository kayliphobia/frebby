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
    public float maxValue = 2500;

    [Header("Debug")]
    public float currentValue;
    public bool windowActive = false;

    private GameManager gameManager;
    public float baseDepletionRate = 1f;   // Night 1 baseline
    public float dayMultiplier = 0.3f;     // +30% per additional day

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip sfx;
    public MarionetteAI marionette;

    // ==========================
    // PARAGRAPH TYPING SYSTEM
    // ==========================

    [TextArea(3, 10)]
    public string[] fakeParagraphs = {
        "Compiling system logs and reconstructing fragmented task data...",
        "Analyzing workstation processes and simulating productivity output...",
        "Decrypting cached work buffers and validating computational integrity...",
        "Processing archived reports and integrating cross-referenced metadata..."
    };

    private string currentParagraph = "";
    private int charIndex = 0;

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
                // Add productivity
                currentValue = Mathf.Min(maxValue, currentValue + 10f);
                productivityBar.value = currentValue;

                // TYPE ONE LETTER FROM CURRENT PARAGRAPH
                if (charIndex < currentParagraph.Length)
                {
                    fakeTextOutput.text += currentParagraph[charIndex];
                    charIndex++;
                }
                else
                {
                    // If the paragraph is finished, load a new one
                    currentParagraph = fakeParagraphs[Random.Range(0, fakeParagraphs.Length)];
                    fakeTextOutput.text = "";
                    charIndex = 0;

                    // Type the first letter instantly
                    fakeTextOutput.text += currentParagraph[charIndex];
                    charIndex++;
                }
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
        if (day == 1 && gameManager.getCurrentHour() < 12)
        {
            return;
        }

        float scaledDepletion = baseDepletionRate * (1f + (day - 1) * dayMultiplier);

        currentValue -= scaledDepletion;
        currentValue = Mathf.Max(0, currentValue);

        if (windowActive && productivityBar != null)
            productivityBar.value = currentValue;

        if (currentValue == 50)
        {
            sfxSource.PlayOneShot(sfx);
        }

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

        if (!active)
        {
            fakeTextOutput.text = "";
            return;
        }

        // When the window opens, start a new paragraph
        currentParagraph = fakeParagraphs[Random.Range(0, fakeParagraphs.Length)];
        charIndex = 0;
        fakeTextOutput.text = "";
    }


    public void ResetProductivity()
    {
        currentValue = maxValue;
    }
}
