using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;

public class ObjectiveCollector : Singleton<ObjectiveCollector> {

	private List<CollectableController> collectedItems = new List<CollectableController>();

	void Start () {
		DontDestroyOnLoad(this);
	}

	public void AddCollectedItem(CollectableController item) {
		if (GetNextValidOrderValue() == item.orderValue) {
			collectedItems.Add(item);
		} else {
			SceneLoader.Instance.RestartCurrentScene();
		}
	}

	private int GetNextValidOrderValue() {
		if (collectedItems.Count == 0) {
			return 1;
		} else {
			return collectedItems[collectedItems.Count - 1].orderValue + 1;
		}
	}
}
