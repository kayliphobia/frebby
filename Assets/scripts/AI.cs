using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected PlayerHidingSystem playerHiding;
    // [SerializeField] protected Image cameraSprite;            
    // [SerializeField] protected SpriteRenderer officeRenderer; 
    [SerializeField] protected GameManager gameManager;
    [SerializeField] protected AnimatronicManager animatronicManager;

    // [Header("Sprites")]
    // [SerializeField] protected Sprite hallwaySprite;
    // [SerializeField] protected Sprite attackSprite;
    // [SerializeField] protected Sprite officeSpriteImage;

    [Header("Settings")]
    [SerializeField] protected float baseMoveDelay = 3f;
    [SerializeField] protected float officeStayTime = 2f;
    [SerializeField] protected float attackWarningTime = 1f;
    [SerializeField] protected Animatronic aiName;

    [Header("Room System")]
    [SerializeField] protected Room currentRoom;

    public int AILevel;
    protected float moveTimer;
    protected bool isActive = true;

    public Animatronic GetAIName() => aiName;

    public AudioSource animatronicAudio;
    public AudioClip footstepSound;
    public AudioClip attackSound;

    protected virtual void Start()
    {
        Debug.Log($"{aiName} has been created!");
        if (animatronicManager != null)
            AILevel = animatronicManager.GetAILevel(gameManager.getCurrentDay(), aiName);

        moveTimer = baseMoveDelay;

        if (currentRoom != null)
            currentRoom.Enter(this);

        // UpdateRoomVisuals();
    }

    protected virtual void Update()
    {
        // Debug.Log("updating");
        if (!isActive) return;

        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            Debug.Log("Trying to move");
            AttemptMovement();
            moveTimer = baseMoveDelay;
        }
    }

    protected void AttemptMovement()
    {
        int randomRoll = Random.Range(1, 21);
        if (AILevel >= randomRoll)
        {
            AdvanceRoom();
        } else
        {
            Debug.Log("Move failed");
            
        }
    }

    protected void AdvanceRoom()
    {
        if (currentRoom == null) return;

        Room nextRoom = currentRoom.GetWeightedConnectedRoom();
        if (nextRoom != null)
        {
            currentRoom.Leave(this);
            currentRoom = nextRoom;
            currentRoom.Enter(this);

            // UpdateRoomVisuals();

            if (currentRoom.roomName.Contains("Office"))
                StartCoroutine(AttackRoutine());
        }
        else
        {
            Debug.Log($"{aiName} could not move, waiting in {currentRoom.roomName}");
        }
    }

    protected virtual IEnumerator AttackRoutine()
    {
        if (currentRoom == null || playerHiding == null) yield break;

        // if (officeRenderer != null)
        // {
        //     officeRenderer.enabled = true;
        //     officeRenderer.sprite = attackSprite;
        // }

        yield return new WaitForSeconds(attackWarningTime);

        if (!playerHiding.IsHiding())
            TriggerJumpscare();
        else
        {
            // Retreat to previous or connected room if available
            Room retreatRoom = ((Office) currentRoom).GetWeightedConnectedRoom();
            if (retreatRoom != null)
            {
                currentRoom.Leave(this);
                currentRoom = retreatRoom;
                animatronicAudio.PlayOneShot(footstepSound);
                currentRoom.Enter(this);
                // UpdateRoomVisuals();
            }
        }
    }

    protected void TriggerJumpscare()
    {
        Debug.Log("Jumpscare triggered!");
        if (gameManager != null)
            gameManager.TriggerGameOver($"{aiName} entered the office");
    }

    // protected void UpdateRoomVisuals()
    // {
    //     if (currentRoom.roomName.Contains("Hallway"))
    //     {
    //         if (cameraSprite != null) cameraSprite.sprite = hallwaySprite;
    //         if (officeRenderer != null) officeRenderer.enabled = false;
    //     }
    //     else if (currentRoom.roomName.Contains("AttackPosition"))
    //     {
    //         if (officeRenderer != null)
    //         {
    //             officeRenderer.enabled = true;
    //             officeRenderer.sprite = attackSprite;
    //         }
    //     }
    //     else if (currentRoom.roomName.Contains("Office"))
    //     {
    //         if (officeRenderer != null)
    //         {
    //             officeRenderer.enabled = true;
    //             officeRenderer.sprite = officeSpriteImage;
    //         }
    //     }
    // }
}
