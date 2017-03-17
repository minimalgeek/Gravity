using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using System;

public class BaseAction : GLMonoBehaviour, IAction {
    
    void Update () {
		if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl)) {
			Execute();
		}
	}

	public virtual void Execute()
    {
    }
}
