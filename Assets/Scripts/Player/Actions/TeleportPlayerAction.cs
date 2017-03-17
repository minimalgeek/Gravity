using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using DG.Tweening;

public class TeleportPlayerAction : BaseAction {

	private GameObject player;
	public TeleportPlayerAction destination;

	void Awake() {
		Assert.IsNotNull(destination);
	}

	new void Start () {
		base.Start();
		player = GameObject.FindGameObjectWithTag(TagsAndLayers.PLAYER);
	}

	public override void Execute() {
		StartCoroutine(Teleport());
	}

	private IEnumerator Teleport() {
		player.transform.DOScale(0.1f, 0.5f);
		player.transform.DOMove(transform.position, 0.5f);
		yield return new WaitForSeconds(0.5f);
		player.transform.position = destination.transform.position;
		player.transform.DOScale(1f, 0.5f);
	}
	
}
