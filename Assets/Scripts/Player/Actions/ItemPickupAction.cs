﻿using System.Collections;
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

	private BoxCollider2D physicsCollider;
	private BoxCollider2D pickupCollider;
	private Rigidbody2D rb;

	private ToCenterRotator toCenterRotator;

	new void Start () {
		base.Start();
		holdingPoint = characterController.itemHoldingPoint.transform;
		toCenterRotator = GetComponent<ToCenterRotator>();

		rb = GetComponent<Rigidbody2D>();
		physicsCollider = Array.Find(GetComponents<BoxCollider2D>(), x => x.isTrigger == false);

		Assert.IsNotNull(rb);
		Assert.IsNotNull(physicsCollider);

		pickupCollider = Array.Find(GetComponents<BoxCollider2D>(), x => x.isTrigger == true);
		if (!pickupCollider) {
			pickupCollider = this.gameObject.AddComponent<BoxCollider2D>();
			pickupCollider.size = physicsCollider.size*2;
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
			float yForce = characterController.GetFacingDirection() == Facing.RIGHT ? -throwSpeed : throwSpeed;
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
