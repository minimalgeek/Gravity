using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SwitchGravityFieldRotationSpeedAction : BaseAction {

	private int currentIndex;
	public float[] states;

	void Awake()
	{
		Assert.IsNotNull(states);
		Assert.IsTrue(states.Length > 1, "Provide at least 2 states");
	}
	
	void Start()
	{
		
	}

	public override void Execute() {

	}

}
