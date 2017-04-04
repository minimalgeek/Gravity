using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gamelogic.Extensions;
using UnityEngine.SceneManagement;

public class LevelLoader : Singleton<LevelLoader> {

	public GameObject buttonPrefab;
	public GameObject panel;

	void Start () {
	}

	public void StartLevel(string levelName) {
		SceneManager.LoadScene(levelName);
	}
}
