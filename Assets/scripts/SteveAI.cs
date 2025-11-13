using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SteveAI : AI
{
    // protected override IEnumerator AttackRoutine()
    // {
    //     if (currentRoom == null || playerHiding == null) yield break;

    //     // // Show attack sprite first
    //     // if (officeRenderer != null)
    //     // {
    //     //     officeRenderer.enabled = true;
    //     //     officeRenderer.sprite = attackSprite;
    //     // }

    //     yield return new WaitForSeconds(attackWarningTime);

    //     if (!playerHiding.IsHiding())
    //     {
    //         TriggerJumpscare();
    //     }
    //     else
    //     {
    //         // Retreat to a connected room if available
    //         Room retreatRoom = ((Office) currentRoom).GetWeightedConnectedRoom();
    //         if (retreatRoom != null)
    //         {
    //             currentRoom.Leave(this);
    //             currentRoom = retreatRoom;
    //             currentRoom.Enter(this);
    //             // UpdateRoomVisuals();
    //         }
    //     }
    // }
    protected override void Update()
    {
        if (gameManager.getCurrentDay() == 1)
        {
            if (gameManager.getCurrentHour() < 12)
            {
                AILevel = 0;
            }
            else
            {
                AILevel = animatronicManager.GetAILevel(1, Animatronic.Steve);
            }
        }
        base.Update();
    }
}
