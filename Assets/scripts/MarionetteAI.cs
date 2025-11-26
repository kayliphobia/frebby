using UnityEngine;
using System.Collections;
using System.Data;

public class MarionetteAI : AI
{
    [Header("Marionette Settings")]
    // [SerializeField] private Sprite marionetteSprite;       // Sprite to show on camera during attack
    [SerializeField] private float baseTravelTime = 6f;     // Base time to reach the office


    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// Call this when the productivity timer depletes to start Marionette's attack.
    /// </summary>
    public void OnProductivityDepleted()
    {
        if (isAttacking) return;  // Prevent multiple attacks
        StartCoroutine(AttackRoutine());
    }

    /// <summary>
    /// Handles Marionette's attack sequence.
    /// </summary>
    protected override IEnumerator AttackRoutine()
    {
        // Play attack sound
        isAttacking = true;
        if (animatronicAudio != null && attackSound != null)
            animatronicAudio.PlayOneShot(attackSound);

        // // Show Marionette sprite on the camera
        // if (cameraSprite != null && marionetteSprite != null)
        //     cameraSprite.sprite = marionetteSprite;

        // Calculate time to reach the office (faster with higher AI level)
        float travelTime = Mathf.Clamp(baseTravelTime - AILevel * 0.5f, 2f, baseTravelTime);
        yield return new WaitForSeconds(travelTime);

        currentRoom.Leave(this);
        currentRoom = currentRoom.GetParentRoom();
        currentRoom.Enter(this);
        yield return StartCoroutine(TriggerJumpscare());
    }

    // Override update functionality to stop the movement logic from happening
    private new void Update()
    {
        
    }
}
