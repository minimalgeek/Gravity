using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

public class SawController : GLMonoBehaviour {

	private GameObject player;
	public GameObject explosionPrefab;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag(TagsAndLayers.PLAYER);
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject == player) {
			Instantiate(explosionPrefab, player.transform.position, player.transform.rotation);
			Destroy(player);
		}
	}
}
