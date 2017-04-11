using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Assertions;
using Gamelogic.Extensions;

public class ItemPickupAction : BaseAction {

	public float releaseSpeed = 3f;
	public float throwSpeedUpLight = 15f;
	public float throwSpeedSideLight = 15f;
	public float throwSpeedUp = 30f;
	public float throwSpeedSide = 30f;

	[Tooltip("If the GameObject doesn't have a triggering collider" +
	"(but has one for the physics collision), it generates one, " + 
	"with this radius increment.")]
	public float autoColliderSizeIncrement = 0.2f;
	private bool pickedUpByPlayer;
	private Transform holdingPoint;

	private Collider2D physicsCollider;
	private Collider2D pickupCollider;
	private Rigidbody2D rb;

	private FaceAxis toCenterRotator;
	private float throwSpeedMultiplier;

	new void Start () {
		base.Start();
		holdingPoint = characterController.ItemHoldingTransform;
		toCenterRotator = GetComponent<FaceAxis>();

		rb = GetComponent<Rigidbody2D>();
		physicsCollider = Array.Find(GetComponents<Collider2D>(), x => x.isTrigger == false);

		Assert.IsNotNull(rb);
		Assert.IsNotNull(physicsCollider);

		pickupCollider = Array.Find(GetComponents<Collider2D>(), x => x.isTrigger == true);
		if (!pickupCollider) {
			pickupCollider = this.gameObject.AddComponent<CircleCollider2D>();
			((CircleCollider2D)pickupCollider).radius *= Mathf.Sqrt(2f);// * (1f + autoColliderSizeIncrement);
			((CircleCollider2D)pickupCollider).radius += autoColliderSizeIncrement;
			pickupCollider.offset = physicsCollider.offset;
			pickupCollider.isTrigger = true;
		}
	}

	public override void Execute() {
		pickedUpByPlayer = !pickedUpByPlayer;
		SetPickedUp(pickedUpByPlayer);
		
		if (pickedUpByPlayer) {
			this.transform.SetParent(holdingPoint);
			transform.DOLocalMove(Vector2.zero, 1f).OnComplete(() => executionEnabled = true);
		} else {
			this.transform.SetParent(null);
			rb.velocity = GetThrowSpeedAsVector() + characterController.velocity;
		}
	}

	public override void PressExecute() {
		characterController.canWalk = false;
		StartCoroutine(ThrowSpeedCalculation());
	}

	public override void ReleaseExecute() {
		characterController.canWalk = true;
		StopAllCoroutines();
		throwSpeedMultiplier = 0;
	}

	private IEnumerator ThrowSpeedCalculation() {
		while(true) {
			throwSpeedMultiplier += Time.deltaTime;
			throwSpeedMultiplier = Mathf.Clamp01(throwSpeedMultiplier);
		}
	}

	private void SetPickedUp(bool picked) {
		physicsCollider.isTrigger = picked;
		rb.simulated = !picked;
		if (toCenterRotator)
			toCenterRotator.enabled = !picked;
	}

	private Vector2 GetThrowSpeedAsVector() {
		Vector2 up = characterController.transform.up;
		Vector2 right = characterController.transform.right;
		Vector2 left = right*-1;

		bool leftDir = Input.GetAxisRaw("Horizontal") < 0;
		bool rightDir = Input.GetAxisRaw("Horizontal") > 0;
		bool upDir = Input.GetAxisRaw("Vertical") > 0;

		float sideSpeed = FloatLerp(throwSpeedSideLight, throwSpeedSide, throwSpeedMultiplier);
		float upSpeed = FloatLerp(throwSpeedUpLight, throwSpeedUp, throwSpeedMultiplier);

		if (leftDir && upDir) {
			return up*upSpeed + left*sideSpeed;
		} else if (rightDir && upDir) {
			return up*upSpeed + right*sideSpeed;
		} else if (leftDir) {
			return left*sideSpeed;
		} else if (rightDir) {
			return right*sideSpeed;
		} else if (upDir) {
			return up*upSpeed;
		} else {
			return up*releaseSpeed;
		}
		
	}

	private float FloatLerp(float from, float to, float time) {
		float diff = to-from;
		return from + diff*time;
	}

}
