using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SliderManager : MonoBehaviour {

	// Scene manager
	Scene2Manager sceneManager;

	// Main sections
	private Transform headerSection;
	private Text name_text;
	private Text value_text;
	private Transform sliderUI;
	private Slider slider;

	// Dimension for this slider
	private Dimension dim;

	public void Initialize(Dimension d) {
		sceneManager = GameObject.Find ("DrawingCanvas").GetComponent<Scene2Manager> ();

		foreach (Transform t in transform) {
			if (t.name == "Header") headerSection = t;
			if (t.name == "SliderUI") sliderUI = t;
		}
		foreach (Transform t in headerSection) {
			if (t.name == "Name") name_text = t.GetComponentsInChildren <Text> ()[0];
			if (t.name == "Value") value_text = t.GetComponentsInChildren <Text> ()[0];
		}
		foreach (Transform t in sliderUI) {
			if (t.name == "Slider") slider = t.GetComponent<Slider> ();
		}

		dim = d;
		name_text.text = dim.name;
		value_text.text = dim.minVal.ToString ();
		slider.maxValue = dim.GetRes ();
	}

	// Update the values of dimensions
	public void UpdateDim (Dimension d) {
		dim = d;
		name_text.text = dim.name;
		slider.maxValue = dim.GetRes ();
	}

	// Update the values of dimensions
	public void UpdateValue (decimal val) {
		slider.value = (int)((val - dim.minVal) / (dim.maxVal - dim.minVal) * dim.GetRes ());
		OnSliderChange ();
	}


	// EVENT

	// Handle the slider
	public void OnSliderChange() {
		decimal val = ((decimal)slider.value / dim.GetRes () * (dim.maxVal - dim.minVal)) + dim.minVal;
		value_text.text = String.Format("{0:0.00}", val);
		string name = gameObject.name;
		sceneManager.CatchSliderChange (Int32.Parse(name.Substring(3, name.Length-3)), val);
	}
}
