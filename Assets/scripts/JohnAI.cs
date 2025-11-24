using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class JohnAI : AI
{    
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
                AILevel = animatronicManager.GetAILevel(1, Animatronic.John);
            }
        }
        base.Update();
    }
}
