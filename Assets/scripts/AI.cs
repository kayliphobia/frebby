using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected PlayerHidingSystem playerHiding;
    [SerializeField] protected GameManager gameManager;
    [SerializeField] protected AnimatronicManager animatronicManager;

    [Header("Settings")]
    [SerializeField] protected float baseMoveDelay = 3f;
    [SerializeField] protected float officeStayTime = 2f;
    [SerializeField] protected float attackWarningTime = 1f;
    [SerializeField] protected float baseAttackLingerTime = 1f;
    [SerializeField] protected Animatronic aiName;
    [SerializeField] protected float graceTimer = 5f;

    [Header("Room System")]
    public Room startRoom;
    [SerializeField] protected Room currentRoom;

    public int AILevel;
    protected float moveTimer;
    protected bool isActive = true;

    public Animatronic GetAIName() => aiName;

    public AudioSource animatronicAudio;

    public float footStepVolumePercentage = 57.5f;
    public AudioClip footstepSound;

    public float attackSoundVolumePercentage = 100f;
    public AudioClip attackSound;

    public float warningSoundVolumePercentage = 100f;
    public AudioClip warningSound;

    public float firedSoundVolumePercentage = 100f;
    public AudioClip firedSound;

    public bool isAttacking;

    protected virtual void Start()
    {
        Debug.Log($"{aiName} has been created!");
        Reset();

        moveTimer = baseMoveDelay;

        if (currentRoom != null)
            currentRoom.Enter(this);

        isAttacking = false;
    }

    protected virtual void Update()
    {
        if (!isActive) return;
        if (isAttacking) return;

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
            AdvanceRoom();
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

            if (currentRoom.roomName.Contains("AttackPosition"))
            {
                animatronicAudio.volume = warningSoundVolumePercentage / 100;
                animatronicAudio.PlayOneShot(warningSound);
            }
                

            if (currentRoom.roomName.Contains("Office"))
                StartCoroutine(AttackRoutine());
        }
    }

    protected virtual IEnumerator AttackRoutine()
    {
        animatronicAudio.volume = attackSoundVolumePercentage / 100;
        animatronicAudio.PlayOneShot(attackSound);
        if (currentRoom == null || playerHiding == null) yield break;

        isAttacking = true;

        float currentWaitTime = Mathf.Clamp(
            attackWarningTime - (attackWarningTime - 1.5f) * AILevel / 20f,
            1f,
            float.MaxValue
        );

        yield return new WaitForSeconds(currentWaitTime);

        float timer = 0;
        float attackLingerTime = baseAttackLingerTime + 6 * (AILevel / 20);
        while (timer < attackLingerTime)
        {
            if (!playerHiding.IsHiding())
            {
                // >>> Call the new coroutine version
                currentRoom.Leave(this);
                currentRoom = currentRoom.GetParentRoom();
                currentRoom.Enter(this);
                yield return StartCoroutine(TriggerJumpscare());
            }
            timer += Time.deltaTime;
            yield return null;
        }
        // retreat
        Room retreatRoom = ((Office)currentRoom).GetWeightedConnectedRoom();
        if (retreatRoom != null)
        {
            currentRoom.Leave(this);
            currentRoom = retreatRoom;
            animatronicAudio.volume = footStepVolumePercentage / 100;
            animatronicAudio.PlayOneShot(footstepSound);
            currentRoom.Enter(this);
            moveTimer = graceTimer;
        }
        isAttacking = false;
    }

    // ================================
    //       UPDATED JUMPSCARE CODE
    // ================================
    protected IEnumerator TriggerJumpscare()
    {
        GameManager.gameOver = true;

        AudioController.PauseAudio?.Invoke();

        Debug.Log("Jumpscare triggered!");

        // 1. Immediately return to desk
        GameManager.ReturnToDesk.Invoke();

        // 2. Wait 1 frame so the desk loads
        yield return null;

        animatronicAudio.volume = firedSoundVolumePercentage / 100;
        animatronicAudio.PlayOneShot(firedSound);
        // 3. Play the jumpscare animation on desk
        JumpscareAnimation jumpscareAnimation = FindFirstObjectByType<JumpscareAnimation>(FindObjectsInactive.Include);

        if (jumpscareAnimation != null)
        {
            jumpscareAnimation.Play();
            yield return new WaitForSeconds(jumpscareAnimation.duration + jumpscareAnimation.holdDuration);
        }

        // 4. Now trigger game over
        if (gameManager != null)
            gameManager.TriggerGameOver($"{aiName} entered the office");
    }

    public void Reset()
    {
        Debug.Log($"reset {aiName}");

        if (currentRoom)
        {
            currentRoom.Leave(this);
        }

        currentRoom = startRoom;
        currentRoom.Enter(this);

        Debug.Log($"{aiName} was reset to {currentRoom}");

        if (animatronicManager != null)
            AILevel = animatronicManager.GetAILevel(gameManager.getCurrentDay(), aiName);

        moveTimer = graceTimer;
    }
}
