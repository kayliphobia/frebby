using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FemaleAI : AI
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
