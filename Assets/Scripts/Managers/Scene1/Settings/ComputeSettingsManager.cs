using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection.Emit;
using System.Linq;
using System;

public class ComputeSettingsManager : MonoBehaviour {

	// Tab
	private TabManager settingsTabManager;

	// Settings for the data
	public string[] names;
	public decimal[] mins, maxs;
	public int[] resolutions;
	public bool interaction3D;

	// GUI caracteristics
	private InputField[] namesDimsIF;
	private InputField[] minDimsIF, maxDimsIF, resolutionDimsIF;
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

		List<Transform> dim_name_elmts = new List<Transform>();
		List<Transform> dim_min_elmts = new List<Transform>();
		List<Transform> dim_max_elmts = new List<Transform>();
		List<Transform> dim_resolution_elmts = new List<Transform>();

		foreach (Transform element in tab1) {
			if (element.name == "DimensionNames_tabler") {
				foreach (Transform sub_element in element) 
					if (sub_element.name.StartsWith ("Dim")) dim_name_elmts.Add (sub_element);
			}
			if (element.name == "DimensionMins_tabler") {
				foreach (Transform sub_element in element) 
					if (sub_element.name.StartsWith ("Dim")) dim_min_elmts.Add (sub_element);
			}
			if (element.name == "DimensionMaxs_tabler") {
				foreach (Transform sub_element in element) 
					if (sub_element.name.StartsWith ("Dim")) dim_max_elmts.Add (sub_element);
			}
			if (element.name == "DimensionResolutions_tabler") {
				foreach (Transform sub_element in element) 
					if (sub_element.name.StartsWith ("Dim")) dim_resolution_elmts.Add (sub_element);
			}
		}

		foreach (Transform element in tab2) {
			if (element.name == "3DInteraction_checkbox")
				interaction3DToggle = element.GetComponentsInChildren <Text> () [0];
		}

		int nbDims = dim_name_elmts.Count;
		names = new string[nbDims];
		mins  = new decimal[nbDims];
		maxs  = new decimal[nbDims];
		resolutions = new int[nbDims];

		namesDimsIF = new InputField[nbDims];
		minDimsIF   = new InputField[nbDims];
		maxDimsIF   = new InputField[nbDims];
		resolutionDimsIF  = new InputField[nbDims];
		for (var i = 0; i < nbDims; i++) {
			namesDimsIF [i] = dim_name_elmts [i].GetComponent<InputField>();
			minDimsIF [i] = dim_min_elmts [i].GetComponent<InputField>();
			maxDimsIF [i] = dim_max_elmts [i].GetComponent<InputField>();
			resolutionDimsIF [i] = dim_resolution_elmts [i].GetComponent<InputField>();
		}
	}

	// Reset all default settings
	public void OnDefault () {
		namesDimsIF[0].text = "theta1"; namesDimsIF[1].text = "theta2";
		namesDimsIF[2].text = "phi1"; 	namesDimsIF[3].text = "phi2";

		minDimsIF[0].text = "0.0"; maxDimsIF[0].text = "1.0"; resolutionDimsIF[0].text = "20";
		minDimsIF[1].text = "0.0"; maxDimsIF[1].text = "1.0"; resolutionDimsIF[1].text = "20";
		minDimsIF[2].text = "0.0"; maxDimsIF[2].text = "1.0"; resolutionDimsIF[2].text = "20";
		minDimsIF[3].text = "0.0"; maxDimsIF[3].text = "1.0"; resolutionDimsIF[3].text = "20";

		interaction3DToggle.text = "x";

		OnApply ();
	}

	// Reset all default settings
	public void OnDefaultBell () {
		namesDimsIF[0].text = "theta1"; namesDimsIF[1].text = "theta2";
		namesDimsIF[2].text = "phi1"; 	namesDimsIF[3].text = "phi2";

		minDimsIF[0].text = "0.0"; maxDimsIF[0].text = "3.0"; resolutionDimsIF[0].text = "20";
		minDimsIF[1].text = "0.0"; maxDimsIF[1].text = "3.0"; resolutionDimsIF[1].text = "20";
		minDimsIF[2].text = "0.0"; maxDimsIF[2].text = "6.0"; resolutionDimsIF[2].text = "40";
		minDimsIF[3].text = "0.0"; maxDimsIF[3].text = "6.0"; resolutionDimsIF[3].text = "40";

		interaction3DToggle.text = "x";

		OnApply ();
	}

	// Set to a given set of settings
	public void SetTo (PermanentObjectsManager pom) {
		for (int i = 0; i < 4; i++) {
			namesDimsIF [i].text = pom.names [i].ToString();
			minDimsIF [i].text = pom.mins [i].ToString();
			maxDimsIF [i].text = pom.maxs [i].ToString();
			resolutionDimsIF[i].text = pom.resolutions[i].ToString();
			namesDimsIF [i].ActivateInputField ();
			minDimsIF [i].ActivateInputField ();
			maxDimsIF [i].ActivateInputField ();
			resolutionDimsIF [i].ActivateInputField ();
		}
		interaction3DToggle.text = pom.interaction3D ? "x" : "";

		OnApply ();
	}

	// Apply the given settings
	public void OnApply () {
		if (!CheckFormat ())
			return;
		
		for (int i = 0; i < namesDimsIF.Length; i++) { 
			names[i]   = namesDimsIF[i].text;
			mins[i]  = decimal.Parse(minDimsIF[i].text);
			maxs[i]  = decimal.Parse(maxDimsIF[i].text);
			resolutions[i] = Int32.Parse(resolutionDimsIF[i].text);
		}
		interaction3D = (interaction3DToggle.text == "x");
		bool changed = true;
		while (changed) {
			changed = false;
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 4; j++) {
					if (i == j) continue;
					if (names [j] == names [i]) {
						names [j] = names [j] + "_";
						changed = true;
					}
				}
			}
		}
	}

	private bool CheckFormat () {
		for (int i = 0; i < namesDimsIF.Length; i++) { 
			if (decimal.Parse(minDimsIF[i].text) >= decimal.Parse(maxDimsIF[i].text))
				return false;
			if (Int32.Parse(resolutionDimsIF[i].text) <= 0)
				return false;
		}
		return true;
	}

	// Update the checkboxes
	public void OnClickCheckbox (Text target_text) {
		if (target_text.text == "") target_text.text = "x";
		else target_text.text = "";
	}
}
