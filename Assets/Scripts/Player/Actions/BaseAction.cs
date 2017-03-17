using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using System;

public enum KeyDirection
{
    KeyUp, KeyDown
}

public class BaseAction : GLMonoBehaviour, IAction
{

    public KeyDirection keyDirectionToTrigger = KeyDirection.KeyUp;

	private bool executionEnabled = false;

    void Update()
    {
		if (!executionEnabled) {
			return;
		}

        if (keyDirectionToTrigger == KeyDirection.KeyUp)
        {
            if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
            {
                Execute();
            }
        }
        else
        {
			if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                Execute();
            }
        }
    }

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == TagsAndLayers.PLAYER) {
			executionEnabled = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == TagsAndLayers.PLAYER) {
			executionEnabled = false;
		}
	}

    public virtual void Execute()
    {
    }
}
