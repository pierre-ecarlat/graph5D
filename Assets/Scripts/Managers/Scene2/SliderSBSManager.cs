using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSBSManager : MonoBehaviour {

	// Main
	private Text name_text;
	private Button minus_button, plus_button;

	// Use this for initialization
	public void Initialize (bool isInteractable) {
		foreach (Transform t in transform) {
			if (t.name == "-_button") minus_button = t.GetComponent<Button> ();
			if (t.name == "Name") name_text = t.GetComponentsInChildren<Text> () [0];
			if (t.name == "+_button") plus_button = t.GetComponent<Button> ();
		}
		SetInteractability (isInteractable);
	}

	// Update the text on the slider step by step
	public void UpdateName(string name) {
		name_text.text = name;
	}

	// Unactivate the sliding buttons
	public void SetInteractability(bool val) {
		minus_button.interactable = val;
		plus_button.interactable = val;
		minus_button.OnDeselect (null);
		plus_button.OnDeselect (null);
	}
}
