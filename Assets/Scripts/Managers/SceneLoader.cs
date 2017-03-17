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

	public void RestartCurrentSceneWithDelayAndShake() {
		StartCoroutine(Restart());
	}

	private IEnumerator Restart() {
		CameraShake shake = Camera.main.GetComponent<CameraShake>();
		if (shake) {
			shake.ShakeCamera(50f, 1f);
		}

		yield return new WaitForSeconds(2f);
		RestartCurrentScene();
	}

}