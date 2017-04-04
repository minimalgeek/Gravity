using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Assertions;
using Gamelogic.Extensions;

public class ItemPickupAction : BaseAction {

	public float throwSpeed = 12f;

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

		if (pickedUpByPlayer) {
			SetPickedUp(true);
			this.transform.SetParent(holdingPoint);
			transform.DOLocalMove(Vector2.zero, 1f).OnComplete(() => executionEnabled = true);
		} else {
			SetPickedUp(false);
			this.transform.SetParent(null);
			rb.velocity = (Vector2)characterController.transform.up * throwSpeed + characterController.velocity;
		}
	}

	private void SetPickedUp(bool picked) {
		physicsCollider.isTrigger = picked;
		rb.simulated = !picked;
		if (toCenterRotator)
			toCenterRotator.enabled = !picked;
	}

}
