using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Data;

public class MarionetteAI : AI
{
    [SerializeField] private Sprite marionetteSprite;
    [SerializeField] private float baseTravelTime = 6f;

    private bool attacking = false;
    protected override void Start()
    {
        base.Start();
        currentState = State.Hallway;
        SetState(currentState);
    }

    public void OnProductivityDepleted()
    {
        if (attacking) return;
        attacking = true;
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        if (cameraSprite != null)
            cameraSprite.sprite = marionetteSprite;
        float timeToOffice = Mathf.Clamp(baseTravelTime - AILevel * 0.5f, 2f, 6f);
        yield return new WaitForSeconds(timeToOffice);

        TriggerJumpscare();
    }

    new void Update()
    {
        
    }
}