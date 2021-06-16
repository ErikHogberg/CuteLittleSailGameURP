using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour {

	public List<GameObject> SelectorObjects;


	public void UnhideSelectorObject(int index) {
		if (index >= SelectorObjects.Count) {
            Debug.LogWarning("index outside of selector list bounds");
			return;
		}

		for (int i = 0; i < SelectorObjects.Count; i++) {
			SelectorObjects[i]?.SetActive(i == index);
		}
	}

	public void ChangeScene(string sceneName) {
		SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
	}

}
