using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Extensions;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader> {

	void Start() {
		DontDestroyOnLoad(this);
	}
	
	public void RestartCurrentScene() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}