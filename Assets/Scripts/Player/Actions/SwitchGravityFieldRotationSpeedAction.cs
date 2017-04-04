using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;

[Serializable]
public class StateAndAnimation {
	[Range(-4.0f, 4.0f)]
	public float angularVelocity;
	public AnimationClip animationClip;
}

public class SwitchGravityFieldRotationSpeedAction : BaseAction {

	private int currentIndex;
	public bool withAcceleration = true;
	public float accelerationDuration = 5f;
	public StateAndAnimation[] states;
	public Animator animator;

	void Awake()
	{
		Assert.IsNotNull(states);
		Assert.IsTrue(states.Length > 1, "Provide at least 2 states");
	}

	new void Start() {
		base.Start();
		StateAndAnimation currentState = states[currentIndex];
		RotationField.Instance.AngularVelocity = currentState.angularVelocity;
	}

	public override void Execute() {
		currentIndex = (currentIndex+1)%states.Length;
		StateAndAnimation currentState = states[currentIndex];

		animator.Play(currentState.animationClip.name);
		if (withAcceleration)
			RotationField.Instance.AccelerateToWithDuration(currentState.angularVelocity, accelerationDuration);
		else
			RotationField.Instance.AngularVelocity = currentState.angularVelocity;
	}

}
