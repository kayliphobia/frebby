using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SteveAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHidingSystem playerHiding;
    [SerializeField] private Image cameraSprite;             // UI sprite for camera feed
    [SerializeField] private SpriteRenderer officeRenderer;  // world-space image for office view
    [SerializeField] private GameManager gameManager;

    [Header("Sprites")]
    [SerializeField] private Sprite hallwaySprite;           // camera sprite when in hallway
    [SerializeField] private Sprite attackSprite;            // sprite for attack position
    [SerializeField] private Sprite officeSpriteImage;       // sprite shown when in office

    [Header("Settings")]
    [SerializeField] private float baseMoveDelay = 5f;
    [SerializeField] private float officeStayTime = 4f;      // time before leaving or attacking
    [SerializeField] private float attackWarningTime = 2f;   // how long player sees attack pose before Steve moves in

    private enum SteveState { Hallway, AttackPosition, Office, Resetting }
    private SteveState currentState = SteveState.Hallway;

    private float moveTimer;
    private int AILevel;
    private bool isActive = true;

    void Start()
    {
        if (gameManager != null)
            AILevel = gameManager.GetAILevel("Steve");

        SetState(SteveState.Hallway);
        moveTimer = baseMoveDelay;
    }

    void Update()
    {
        if (!isActive) return;

        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            AttemptMovement();
            moveTimer = baseMoveDelay;
        }
    }

    private void AttemptMovement()
    {
        int randomRoll = Random.Range(1, 21);
        if (AILevel >= randomRoll)
        {
            AdvanceState();
        }
    }

    private void AdvanceState()
    {
        switch (currentState)
        {
            case SteveState.Hallway:
                SetState(SteveState.AttackPosition);
                break;

            case SteveState.AttackPosition:
                StartCoroutine(MoveToOffice());
                break;

            case SteveState.Office:
                // handled in coroutine
                break;

            case SteveState.Resetting:
                SetState(SteveState.Hallway);
                break;
        }
    }

    private IEnumerator MoveToOffice()
    {
        // Player sees Steve preparing to move (camera + office sprite)
        SetState(SteveState.AttackPosition);
        yield return new WaitForSeconds(attackWarningTime);

        // Move to Office after warning
        SetState(SteveState.Office);
        yield return new WaitForSeconds(officeStayTime);

        // Check again after being in office for a while
        if (playerHiding != null && !playerHiding.IsHiding())
        {
            // Player failed to hide in time -> jumpscare
            TriggerJumpscare();
        }
        else
        {
            // Player hid successfully -> Steve retreats
            SetState(SteveState.Hallway);
        }
    }

    private void TriggerJumpscare()
    {
        Debug.Log("Steve jumpscare triggered!");
        if (gameManager != null)
            gameManager.TriggerGameOver("Steve entered the office");
        else
            Debug.LogWarning("No GameManager connected — manually handle game over.");
    }

    private void SetState(SteveState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case SteveState.Hallway:
                if (cameraSprite != null)
                    cameraSprite.sprite = hallwaySprite;
                if (officeRenderer != null)
                    officeRenderer.enabled = false;
                break;

            case SteveState.AttackPosition:
                if (cameraSprite != null)
                    cameraSprite.sprite = attackSprite;
                if (officeRenderer != null)
                {
                    officeRenderer.enabled = true;
                    officeRenderer.sprite = attackSprite;
                }
                break;

            case SteveState.Office:
                if (cameraSprite != null)
                    cameraSprite.sprite = null;
                if (officeRenderer != null)
                {
                    officeRenderer.enabled = true;
                    officeRenderer.sprite = officeSpriteImage;
                }
                break;

            case SteveState.Resetting:
                if (cameraSprite != null)
                    cameraSprite.sprite = null;
                if (officeRenderer != null)
                    officeRenderer.enabled = false;
                break;
        }
    }
}
