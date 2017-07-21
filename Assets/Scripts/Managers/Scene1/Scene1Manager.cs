using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System;
using System.Globalization;

public class Scene1Manager : MonoBehaviour {

	// Permanent objects between scenes
	private PermanentObjectsManager pom;

	// Cameras manager
	//private StereoCamerasManager scm;

	// GUI Elements
	private Transform drawingArea;
	private Transform background;
	private Transform mainPanel;
	private Transform computeSettingsPanel;
	private Transform loadSettingsPanel;
	private Transform loadingBarPanel;

	// Managers
	private TabManager mainTabManager;
	private ComputeSettingsManager computeSettingsManager;
	private LoadSettingsManager loadSettingsManager;
	private LoadingBarManager loadingBarManager;

	// Tab 1 - Formula
	private Transform tab1;
	private Dropdown formula_dropdown;
	private Transform dropdownComputeFooter;
	private Transform visualizationComputeFooter;
	private Text formula_text;

	// Tab 2 - Load
	private Transform tab2;
	private Text fileName_text;

	// State flags
	private bool state_onSettings;
	private bool state_onLoadingBar;
	private bool state_onVisualization;


	void Start() {
		// If initialization is incomplete
		if (GameObject.Find ("PermanentObjects") == null) { 
			SceneManager.LoadScene ("sc0_pom");
			return;
		}

		pom = GameObject.Find ("PermanentObjects").GetComponent <PermanentObjectsManager> ();
		drawingArea = GameObject.Find ("DrawingCanvas").GetComponentsInChildren <Transform>()[2];

		foreach (Transform child in drawingArea) {
			if (child.name == "Background") background = child;
			if (child.name == "MainPanel") mainPanel = child;
			if (child.name == "ComputeSettingsPanel") computeSettingsPanel = child;
			if (child.name == "LoadSettingsPanel") loadSettingsPanel = child;
			if (child.name == "LoadingBarPanel") loadingBarPanel = child;
		}
		mainTabManager = mainPanel.GetComponent <TabManager>();
		computeSettingsManager = computeSettingsPanel.GetComponent <ComputeSettingsManager>();
		loadSettingsManager = loadSettingsPanel.GetComponent <LoadSettingsManager>();
		loadingBarManager = loadingBarPanel.GetComponent <LoadingBarManager>();

		mainTabManager.Initialize ();
		computeSettingsManager.Initialize ();
		loadSettingsManager.Initialize ();
		loadingBarManager.Initialize ();

		tab1 = mainTabManager.GetTab (1);
		tab2 = mainTabManager.GetTab (2);

		foreach (Transform child in tab1) {
			if (child.name == "Formula_dropdown") 
				formula_dropdown = child.GetComponent <Dropdown> ();
			if (child.name == "FooterButtonsSection") 
				dropdownComputeFooter = child;
			if (child.name == "FooterVisualizationSection") { 
				visualizationComputeFooter = child;
				foreach (Transform child2 in visualizationComputeFooter) {
					if (child2.name == "DisplayArea") 
						formula_text = child2.GetComponentsInChildren <Text> () [0];
				}
			}
		}
		foreach (Transform child in tab2) {
			if (child.name == "FileName") 
				fileName_text = child.GetComponentsInChildren <Text> () [0];
		}

		mainPanel.gameObject.SetActive (true);
		computeSettingsPanel.gameObject.SetActive (false);
		loadSettingsPanel.gameObject.SetActive (false);
		loadingBarPanel.gameObject.SetActive (false);
		dropdownComputeFooter.gameObject.SetActive (true);
		visualizationComputeFooter.gameObject.SetActive (false);

		state_onSettings = false;
		state_onLoadingBar = false;
		state_onVisualization = false;

		foreach (string name in Enum.GetNames(typeof(Formula.Function))) {
			if (name != "None") {
				formula_dropdown.options.Add (new Dropdown.OptionData () { text = name });
			}
		}

		if (pom.names.Length != 0) {
			computeSettingsManager.SetTo (pom);
			formula_dropdown.value = ((int)pom.function == 0) ? 1 : 0;
			formula_dropdown.value = (int)pom.function;
			loadSettingsManager.SetTo (pom);
		} else {
			formula_dropdown.value = 1;
			formula_dropdown.value = 0;
		}
		OnUpdateFormulaText ();
	}


	// Open the settings panel
	public void OnOpenSettingsPanel (Transform panel) {
		if (!actionOnGoing()) {
			state_onSettings = true;
			mainPanel.gameObject.SetActive (false);
			panel.gameObject.SetActive (true);
		}
	}

	// Close the settings panel (should be called for cancel or apply)
	public void OnCloseSettingsPanel (Transform panel) {
		if (state_onSettings) {
			state_onSettings = false;
			panel.gameObject.SetActive (false);
			mainPanel.gameObject.SetActive (true);
		}
	}

	// Returns true if a panel is opened
	private bool actionOnGoing() {
		return (state_onSettings || state_onLoadingBar);
	}




	// Tab1 - Formula
	// Compute the graph from the function
	public void OnCompute () {
		if (!actionOnGoing()) {
			pom.names = computeSettingsManager.names;
			pom.mins = computeSettingsManager.mins;
			pom.maxs = computeSettingsManager.maxs;
			pom.resolutions = computeSettingsManager.resolutions;
			pom.data = new Data (pom.names, pom.mins, pom.maxs, pom.resolutions);
			pom.interaction3D = computeSettingsManager.interaction3D;
			pom.function = (Formula.Function)formula_dropdown.value;
			pom.data.UpdateCharacteristics (pom.function);

			mainPanel.gameObject.SetActive (false);
			background.gameObject.SetActive (false);
			SceneManager.LoadScene ("sc2_display");
		}
	}

	// Open the visualization panel
	public void OnVisualizeFormula () {
		if (!state_onVisualization) {
			state_onVisualization = true;
			dropdownComputeFooter.gameObject.SetActive (false);
			visualizationComputeFooter.gameObject.SetActive (true);
		}
	}

	// Close the visualization panel
	public void OnCloseVisualization () {
		if (state_onVisualization) {
			state_onVisualization = false;
			dropdownComputeFooter.gameObject.SetActive (true);
			visualizationComputeFooter.gameObject.SetActive (false);
		}
	}

	// Update the formula text
	public void OnUpdateFormulaText () {
		formula_text.text = Formula.GetFormulaText (formula_dropdown.value);
		if (((Formula.Function)formula_dropdown.value) == Formula.Function.BellState)
			computeSettingsManager.OnDefaultBell ();
	}



	// Tab2 - Load
	// Compute the graph from the file
	public void OnLoad () {
		if (!actionOnGoing()) {
			mainPanel.gameObject.SetActive (false);
			background.gameObject.SetActive (false);
			StoreSingleFile (fileName_text.text);
		}
	}

	// Open the load file window
	public void OnBrowse () {
		if (!actionOnGoing()) {
			var extensions = new [] { new ExtensionFilter ("Text files", "txt") };
			var paths = StandaloneFileBrowser.OpenFilePanel ("Select the .txt file with the data.", 
															 "Assets/Resources/Data/", extensions, false);
			if (paths.Length > 0) fileName_text.text = paths [0];
		}
	}

	// Stop the laoding process
	public void OnCancelLoadingBar () {
		StopAllCoroutines ();
		loadingBarManager.ReInitialize ();
		state_onLoadingBar = false;
		loadingBarPanel.gameObject.SetActive (false);
		background.gameObject.SetActive (true);
		mainPanel.gameObject.SetActive (true);
		pom.data.data.Clear ();
	}

	// Read a single file and store it in data
	private void StoreSingleFile (string path) {
		StartCoroutine (Read (path));
	}

	// Start a coroutine for reading the file
	private IEnumerator Read(string path) {
		bool displayBar = false;
		int nbLinesDone = 0;

		using (StreamReader sr = new StreamReader (path)) {
			string[] names = new string [4];
			decimal[] mins = new decimal [4];
			decimal[] maxs = new decimal [4];
			decimal[] steps = new decimal[4];

			for (int i = 0; i < 4; i++) {
				string caractsLine = sr.ReadLine ();
				string[] caractsLineSplitted = caractsLine.Substring (1, caractsLine.Length - 2)
														  .Split (new[] { ", " }, System.StringSplitOptions.None);
				for (int j = 0; j < caractsLineSplitted.Length; j++) {
					switch (i) {
					case 0: names [j] = caractsLineSplitted [j]; break;
					case 1: mins  [j] = decimal.Parse(caractsLineSplitted [j]); break;
					case 2: maxs  [j] = decimal.Parse(caractsLineSplitted [j]); break;
					case 3: steps [j] = decimal.Parse(caractsLineSplitted [j]); break;
					}
				}
			}
			pom.data = new Data (names, mins, maxs, steps);
			displayBar = (pom.data.getTotalNbOfPoints () > 10000);
			int displayStep = pom.data.getTotalNbOfPoints ()/100;

			if (displayBar) {
				loadingBarPanel.gameObject.SetActive (true);
				state_onLoadingBar = true;
			}

			string line = "";
			while ((line = sr.ReadLine ()) != null) {
				if (line.Length == 0 || line[0] != '{') continue;
				string[] splittedLines = line.Substring (1, line.Length - 2)
											 .Split(new[] {", " }, System.StringSplitOptions.None);
				float magnitude;
				if (loadSettingsManager.beginByMagnitude) {
					magnitude = float.Parse(splittedLines[0]);
					splittedLines = splittedLines.Skip (1).ToArray ();
				} else {
					magnitude = float.Parse(splittedLines[splittedLines.Length - 1]);
				}

				Coordinate coordMain = new Coordinate (decimal.Parse(splittedLines[0]), decimal.Parse(splittedLines[1]));
				Coordinate coordAdd = new Coordinate (decimal.Parse(splittedLines[2]), decimal.Parse(splittedLines[3]));
				if (!pom.data.data.ContainsKey (coordAdd))
					pom.data.data.Add (coordAdd, new PointsScatter ());
				pom.data.data[coordAdd].points.Add (coordMain, magnitude);

				nbLinesDone++;
				if (displayBar && nbLinesDone > displayStep) {
					loadingBarManager.UpdateBar (nbLinesDone / displayStep);
					nbLinesDone = 0;
					yield return null;
				}
			}
		}

		pom.interaction3D = loadSettingsManager.interaction3D;
		pom.function = Formula.Function.None;
		pom.beginByMagnitude = loadSettingsManager.beginByMagnitude;
		pom.data.UpdateCharacteristics (pom.function);

		if (displayBar) {
			loadingBarPanel.gameObject.SetActive (false);
			state_onLoadingBar = false;
		}

		mainPanel.gameObject.SetActive (false);
		background.gameObject.SetActive (false);
		SceneManager.LoadScene ("sc2_display");
	}
}
