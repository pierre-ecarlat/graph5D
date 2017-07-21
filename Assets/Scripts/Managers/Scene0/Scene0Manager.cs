using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene0Manager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SceneManager.LoadScene ("sc1_menu");
	}

	// Don't destroy the gameobject between scenes
	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}
}
