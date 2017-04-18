using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using System;

public class MouseBaseAction : BaseAction
{
    new protected void Update()
    {
        if (!executionEnabled)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            PressExecute();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ReleaseExecute();
        }
    }

    public override void Execute() { }
}
