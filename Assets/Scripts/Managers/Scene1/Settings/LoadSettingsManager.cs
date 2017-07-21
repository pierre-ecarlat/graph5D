using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection.Emit;
using System.Linq;
using System;

public class LoadSettingsManager : MonoBehaviour {

	// Tab
	private TabManager settingsTabManager;

	// Settings for the data
	public bool beginByMagnitude;
	public bool interaction3D;

	// GUI caracteristics
	private Text beginByMagnitudeToggle;
	private Text interaction3DToggle;


	// Start, get components and active the first tab
	public void Initialize () {
		InitializeVariables ();
		OnDefault ();
	}

	// Initialize variables
	public void InitializeVariables () {
		settingsTabManager = gameObject.GetComponent <TabManager>();
		settingsTabManager.Initialize ();

		Transform tab1 = settingsTabManager.GetTab (1);
		Transform tab2 = settingsTabManager.GetTab (2);

		foreach (Transform element in tab1) {
			if (element.name == "BeginByMagnitude_checkbox")
				beginByMagnitudeToggle = element.GetComponentsInChildren <Text> () [0];
		}

		foreach (Transform element in tab2) {
			if (element.name == "3DInteraction_checkbox")
				interaction3DToggle = element.GetComponentsInChildren <Text> () [0];
		}
	}

	// Reset all default settings
	public void OnDefault () {
		beginByMagnitudeToggle.text = "";
		interaction3DToggle.text = "x";
		OnApply ();
	}

	// Set to a given set of settings
	public void SetTo (PermanentObjectsManager pom) {
		beginByMagnitudeToggle.text = pom.beginByMagnitude ? "x" : "";
		interaction3DToggle.text = pom.interaction3D ? "x" : "";
		OnApply ();
	}

	// Apply the given settings
	public void OnApply () {
		beginByMagnitude = (beginByMagnitudeToggle.text == "x");
		interaction3D = (interaction3DToggle.text == "x");
	}

	// Update the checkboxes
	public void OnClickCheckbox (Text target_text) {
		if (target_text.text == "") target_text.text = "x";
		else target_text.text = "";
	}
}
