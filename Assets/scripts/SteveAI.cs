using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Data;

public class SteveAI : AI
{
    // [Header("Settings")]
    // [SerializeField] private float baseMoveDelay = 5f;
    // [SerializeField] private float officeStayTime = 4f;      // time before leaving or attacking
    // [SerializeField] private float attackWarningTime = 2f;   // how long player sees attack pose before Steve moves in

    protected override void Start()
    {
        base.Start();
        currentState = State.Hallway;
        SetState(currentState);
        // Define the behaviour for each state transition
        stateMapping = new Dictionary<State, System.Action>
        {
            {State.Hallway, () =>
                {
                    SetState(State.AttackPosition);
                }
            },
            {State.AttackPosition, () =>
                {
                    StartCoroutine(MoveToOffice());
                }
            },
            {State.Office, () =>
                {

                }
            },
            {State.Resetting, () =>
                {
                    SetState(State.Hallway);
                }
            },
        };
    }

    private IEnumerator MoveToOffice()
    {
        // Player sees Steve preparing to move (camera + office sprite)
        SetState(State.AttackPosition);
        yield return new WaitForSeconds(attackWarningTime);

        // Move to Office after warning
        SetState(State.Office);
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
            SetState(State.Resetting);
        }
    }

    // private void SetState(SteveState newState)
    // {
    //     currentState = newState;

    //     switch (newState)
    //     {
    //         case SteveState.Hallway:
    //             if (cameraSprite != null)
    //                 cameraSprite.sprite = hallwaySprite;
    //             if (officeRenderer != null)
    //                 officeRenderer.enabled = false;
    //             break;

    //         case SteveState.AttackPosition:
    //             // if (cameraSprite != null)
    //             //     cameraSprite.sprite = attackSprite;
    //             if (officeRenderer != null)
    //             {
    //                 officeRenderer.enabled = true;
    //                 officeRenderer.sprite = attackSprite;
    //             }
    //             break;

    //         case SteveState.Office:
    //             if (cameraSprite != null)
    //                 cameraSprite.sprite = null;
    //             if (officeRenderer != null)
    //             {
    //                 officeRenderer.enabled = true;
    //                 officeRenderer.sprite = officeSpriteImage;
    //             }
    //             break;

    //         case SteveState.Resetting:
    //             if (cameraSprite != null)
    //                 cameraSprite.sprite = null;
    //             if (officeRenderer != null)
    //                 officeRenderer.enabled = false;
    //             break;
    //     }
    // }
}
