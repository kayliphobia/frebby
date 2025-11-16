using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class JohnAI : AI
{

    protected override void Update()
    {
        if (gameManager.getCurrentDay() < 2)
        {
            AILevel = 0;
        }
        base.Update();
    }
}
