using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabMenuManager : MonoBehaviour {

	// Tabs
	public List<GameObject> tabs;

	// General
	public float tabButton_height = 28f;

	// GUI
	private Transform ui;


	// Use this for initialization
	void Start () {
		foreach (Transform child in this.transform) {
			if (child.name == "UI") ui = child;
		}
		//ui.
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
