using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;


public class AI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected PlayerHidingSystem playerHiding;
    [SerializeField] protected Image cameraSprite;             // UI sprite for camera feed
    [SerializeField] protected SpriteRenderer officeRenderer;  // world-space image for office view
    [SerializeField] protected GameManager gameManager;

    [SerializeField] protected AnimatronicManager animatronicManager;

    [Header("Sprites")]
    [SerializeField] protected Sprite hallwaySprite;           // camera sprite when in hallway
    [SerializeField] protected Sprite attackSprite;            // sprite for attack position
    [SerializeField] protected Sprite officeSpriteImage;       // sprite shown when in office

    [Header("Settings")]
    [SerializeField] protected float baseMoveDelay;
    [SerializeField] protected float officeStayTime;      // time before leaving or attacking
    [SerializeField] protected float attackWarningTime;   // how long player sees attack pose before Steve moves in

    [SerializeField] protected Animatronic animatronicName; // name of this animatronic

    protected float moveTimer;
    protected int AILevel;
    protected bool isActive = true;

    protected enum State { Hallway, AttackPosition, Office, Resetting }

    [SerializeField]
    protected State currentState;

    protected Dictionary<State, System.Action> stateMapping;

    public AudioSource animatronicAudio;
    public AudioClip footstepSound;
    protected virtual void Start()
    {
        Debug.Log($"{animatronicName} has been created!");
        if (animatronicManager != null)
            AILevel = animatronicManager.GetAILevel(gameManager.getCurrentDay(), Animatronic.Steve);

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

    protected void AttemptMovement()
    {
        int randomRoll = Random.Range(1, 21);
        if (AILevel >= randomRoll)
        {
            AdvanceState();
        }
    }

    protected void AdvanceState()
    {
        stateMapping[currentState]();
        Debug.Log($"{name} has advanced to state {currentState}");
    }

    protected void TriggerJumpscare()
    {
        Debug.Log("Jumpscare triggered!");
        if (gameManager != null)
            gameManager.TriggerGameOver($"{animatronicName} entered the office");
        else
            Debug.LogWarning("No GameManager connected — manually handle game over.");
    }

    protected void SetState(State newState)
    {
        currentState = newState;

        switch (newState)
        {
            case State.Hallway:
                if (cameraSprite != null)
                    cameraSprite.sprite = hallwaySprite;
                if (officeRenderer != null)
                    officeRenderer.enabled = false;
                break;

            case State.AttackPosition:
                // if (cameraSprite != null)
                //     cameraSprite.sprite = attackSprite;
                if (officeRenderer != null)
                {
                    officeRenderer.enabled = true;
                    officeRenderer.sprite = attackSprite;
                }
                break;

            case State.Office:
                if (cameraSprite != null)
                    cameraSprite.sprite = null;
                if (officeRenderer != null)
                {
                    officeRenderer.enabled = true;
                    officeRenderer.sprite = officeSpriteImage;
                }
                break;

            case State.Resetting:
                if (cameraSprite != null)
                    cameraSprite.sprite = null;
                if (officeRenderer != null)
                    officeRenderer.enabled = false;
                animatronicAudio.PlayOneShot(footstepSound);
                break;
        }
    }
}
