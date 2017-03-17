using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using System;
using DG.Tweening;

public enum KeyDirection
{
    KeyUp, KeyDown
}

public class BaseAction : GLMonoBehaviour, IAction
{

    public KeyDirection keyDirectionToTrigger = KeyDirection.KeyUp;

	protected bool executionEnabled = false;

    protected GameObject player;

	protected void Start() {
		BoxCollider2D collider = this.GetOrAddComponent<BoxCollider2D>();
		collider.isTrigger = true;

        player = GameObject.FindGameObjectWithTag(TagsAndLayers.PLAYER);
	}

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
