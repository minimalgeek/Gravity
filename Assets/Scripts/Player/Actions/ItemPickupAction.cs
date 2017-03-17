using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Assertions;
using Gamelogic.Extensions;

public class ItemPickupAction : BaseAction {

	public float throwSpeed = 5f;

	private bool pickedUpByPlayer;
	private Transform holdingPoint;

	private Collider2D physicsCollider;
	private Rigidbody2D rb;

	private CharacterController charController;
	private ToCenterRotator toCenterRotator;

	new void Start () {
		base.Start();
		charController = player.GetComponent<CharacterController>();
		holdingPoint = charController.itemHoldingPoint.transform;
		toCenterRotator = GetComponent<ToCenterRotator>();

		rb = GetComponent<Rigidbody2D>();
		physicsCollider = Array.Find(GetComponents<Collider2D>(), x => x.isTrigger == false);

		Assert.IsNotNull(rb);
		Assert.IsNotNull(physicsCollider);
	}
	
	public override void Execute() {
		pickedUpByPlayer = !pickedUpByPlayer;

		if (pickedUpByPlayer) {
			SetPickedUp(true);
			this.transform.SetParent(holdingPoint);
			transform.DOLocalMove(Vector2.zero, 1f).OnComplete(() => executionEnabled = true);
		} else {
			SetPickedUp(false);
			float yForce = charController.GetFacingDirection() == Facing.RIGHT ? throwSpeed : -throwSpeed;
			rb.AddForce(Vector3.zero.WithY(yForce), ForceMode2D.Impulse);
			this.transform.SetParent(null);
		}
	}

	private void SetPickedUp(bool picked) {
		physicsCollider.isTrigger = picked;
		rb.simulated = !picked;
		if (toCenterRotator)
			toCenterRotator.enabled = !picked;
	}

}
