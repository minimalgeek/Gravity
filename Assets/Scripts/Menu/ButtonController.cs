using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    private Button myButton;
	public string sceneNameToStart;

    void Awake()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(() => { SceneManager.LoadScene(sceneNameToStart); });
    }

}
