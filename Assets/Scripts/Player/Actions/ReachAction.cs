using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using System;
using DG.Tweening;
public class ReachAction : GLMonoBehaviour, IAction {

	protected bool executionEnabled = false;

    protected GameObject player;
    protected PolarCharacterController characterController;

	protected void Start() {
		player = GameObject.FindGameObjectWithTag(TagsAndLayers.PLAYER);
        characterController = PolarCharacterController.Instance;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == TagsAndLayers.PLAYER) {
			Execute();
		}
	}

    public virtual void Execute()
    {
    }
}
