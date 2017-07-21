using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System;
using UnityEngine.Assertions;

public class Scene2Manager : MonoBehaviour {

	// Permanent objects between scenes
	private PermanentObjectsManager pom;

	// Cameras manager
	//private StereoCamerasManager scm;
	private CameraManager cameraManager;

	// Data
	private Data dataHandler;

	// GUI Elements
	private Transform grapher;
	private Transform drawingCanvas;
	private Transform drawingArea;
	private Transform background, content;
	private Transform parameters, graphics, navigation;

	// Managers
	private GraphManager grapherManager;
	private SliderManager dim1SliderManager, dim2SliderManager;
	private SliderSBSManager resolutionSSBSManager;
	private Text activateRotationToggle, fullGraphicsToggle;
	private Dropdown dim1StateDropdown, dim2StateDropdown;

	// Handle the events
	private Interaction interaction;
	private Transition transition;

	// Settings bools
	private bool activateRotation;
	private bool fullGraphics;
	private bool currentlyMovingCamera;
	private bool staticCall;


	// Start function
	void Start() {
		// If initialization is incomplete
		if (GameObject.Find ("PermanentObjects") == null) { 
			SceneManager.LoadScene ("sc0_pom");
			return;
		}

		pom = GameObject.Find ("PermanentObjects").GetComponent <PermanentObjectsManager> ();
		cameraManager = GameObject.Find ("Camera").GetComponent <CameraManager> ();
		grapher = GameObject.Find ("Grapher").transform;
		drawingCanvas = GameObject.Find ("DrawingCanvas").GetComponentsInChildren <Transform>()[1];
		drawingArea = drawingCanvas.GetComponentsInChildren <Transform>()[1];

		foreach (Transform child in drawingArea) {
			if (child.name == "Background") 
				background = child;
			if (child.name == "Content") { 
				content = child;
				foreach (Transform subchild in child) {
					if (subchild.name == "Parameters") parameters = subchild;
					if (subchild.name == "Graphics") graphics = subchild;
					if (subchild.name == "Navigation") navigation = subchild;
				}
			}
		}

		grapherManager = grapher.gameObject.GetComponent<GraphManager> ();
		foreach (Transform child in parameters) {
			if (child.name == "Dim1") dim1SliderManager = child.GetComponent<SliderManager> ();
			if (child.name == "Dim2") dim2SliderManager = child.GetComponent<SliderManager> ();
		}
		foreach (Transform child in graphics) {
			if (child.name == "Resolution") resolutionSSBSManager = child.GetComponent<SliderSBSManager> ();
			if (child.name == "ExtraGraphics") { 
				foreach (Transform subchild in child) {
					if (subchild.name == "ActivateRotation") 
						activateRotationToggle = subchild.GetComponentsInChildren <Transform> () [1]
														 .GetComponentsInChildren <Text> () [0];
					if (subchild.name == "FullGraphics") 
						fullGraphicsToggle = subchild.GetComponentsInChildren <Transform> () [1]
													 .GetComponentsInChildren <Text> () [0];
				}
			}
		}
		foreach (Transform child in navigation) {
			if (child.name == "Dim1") dim1StateDropdown = child.GetComponent<Dropdown> ();
			if (child.name == "Dim2") dim2StateDropdown = child.GetComponent<Dropdown> ();
		}

		dataHandler = pom.data;
		Dimension[] dims = dataHandler.state.dimensions;
		grapherManager.Initialize (dataHandler, pom.function);

		cameraManager.Initialize (dataHandler);
		dim1SliderManager.Initialize (dims[2]);
		dim2SliderManager.Initialize (dims[3]);
		resolutionSSBSManager.Initialize (pom.function != Formula.Function.None);
		resolutionSSBSManager.UpdateName (dims[0].GetRes () + " x " + dims[1].GetRes ());

		interaction = new Interaction (Interaction.InteractionType.Mouse);
		if (pom.interaction3D)
			interaction = new Interaction (Interaction.InteractionType.Mouse3D);
		transition = new Transition ();

		UpdateCheckboxes ();
		UpdateNavigationDropdowns (dims);
		currentlyMovingCamera = false;
		staticCall = false;

		grapherManager.pointsValues = grapherManager.createPointsFrom(dataHandler.state);
		grapherManager.DrawPoints (grapherManager.pointsValues);

		int[] orders = dataHandler.state.current.Value;
	}

	void Update () {
		// If between two states (transition)
		if (transition.onTransition ()) {
			DrawTransitionalPoints ();		// If drawable, draw new points
			Rotate ();						// Rotate the graph or camera
			transition.GoToNextFrame ();	// Update current frame
			CatchEndTransition ();			// If transition is over, end it
		}

		else {
			// If on moving the camera (regular mode: wheel pressed)
			if (currentlyMovingCamera) {
				cameraManager.UpdateCameraPosition (interaction.GetMouseAxis ());
			} 

			// In any case
			grapherManager.MoveGraph(interaction.GetRotation(), interaction.GetTranslation());
			cameraManager.UpdateZoom (interaction.GetMouseWheel ());

			ListenToChanges ();
		}
	}


	// Get the key press for the rotation
	private void ListenToChanges () {
		State prev = dataHandler.state.Duplicate ();

		switch (interaction.GetAction ()) {
		case Interaction.Action.UpdateStateToLeft:
			transition.SetToUpdateState ();
			transition.SetToPrev ();
			dataHandler.state.goToPrevious ();
			CreateTransitions (prev, dataHandler.state);
			break;
		case Interaction.Action.UpdateStateToLeftStatic:
			staticCall = true;
			transition.SetToUpdateState ();
			transition.SetToPrev ();
			dataHandler.state.goToPrevious ();
			CreateTransitions (prev, dataHandler.state);
			break;
		case Interaction.Action.UpdateStateToRight:
			transition.SetToUpdateState ();
			transition.SetToNext ();
			dataHandler.state.goToNext ();
			CreateTransitions (prev, dataHandler.state);
			break;
		case Interaction.Action.UpdateStateToRightStatic:
			staticCall = true;
			transition.SetToUpdateState ();
			transition.SetToNext ();
			dataHandler.state.goToNext ();
			CreateTransitions (prev, dataHandler.state);
			break;
		case Interaction.Action.RotateOverToDown:
			transition.SetToRotateOver ();
			transition.SetToPrev ();
			break;
		case Interaction.Action.RotateOverToUp:
			transition.SetToRotateOver ();
			transition.SetToNext ();
			break;
		case Interaction.Action.OnMovingCamera:
			currentlyMovingCamera = true;
			break;
		case Interaction.Action.OnStopMovingCamera:
			currentlyMovingCamera = false;
			break;
		}
	}

	// Call the transition creator in grapher manager
	private void CreateTransitions (State prev, State next) {
		int nbFrames = transition.GetNbOfFrames ();
		State redPrev = prev.Duplicate ();
		State redNext = next.Duplicate ();

		if (!fullGraphics) {
			redPrev.SetMinResolutions ();
			redNext.SetMinResolutions ();
			grapherManager.pointsValues = grapherManager.createPointsFrom (redPrev);
		}

		grapherManager.GetTransitionalData (redPrev, redNext, nbFrames);
		grapherManager.transitionalPointsValues[nbFrames - 1] = grapherManager.createPointsFrom(next);
	}

	// Draw the points
	private void DrawTransitionalPoints () {
		if (transition.isADrawingTransition ())
			grapherManager.DrawPoints (grapherManager.transitionalPointsValues [transition.currentFrame]);
	}

	// Update the current dimensions
	private void Rotate () {
		Vector3 graphPos = dataHandler.getGraphPosition ();
		float angle = transition.GetAngleRotation () * transition.isNegative () / (float)transition.GetNbOfFrames ();
		
		if (transition.onUpdateState () && transition.isRotating () && activateRotation && !staticCall) 
			grapherManager.RotateBy (graphPos, angle);
		else if (transition.onRotateOver ()) {
			grapher.RotateAround (graphPos, Vector3.right, angle);
		}
	}

	// Ends the rotation process
	private void CatchEndTransition () {
		if (transition.RotationIsOver ()) {
			int[] orders = dataHandler.state.current.Value;

			transition.Reset();
			resolutionSSBSManager.SetInteractability (pom.function != Formula.Function.None);
			grapherManager.UpdateGridNames (dataHandler.state);

			Dimension[] dims = dataHandler.state.dimensions;
			UpdateDimsSlider (dim1SliderManager, dims[2]);
			UpdateDimsSlider (dim2SliderManager, dims[3]);
			UpdateNavigationDropdowns (dims);
			resolutionSSBSManager.UpdateName (dims[0].GetRes () + " x " + dims[1].GetRes ());

			dim1StateDropdown.interactable = true;
			dim2StateDropdown.interactable = true;
			staticCall = false;
		}
	}



	// EVENTS

	// Apply given settings
	public void OnBack () {
		SceneManager.LoadScene ("sc1_menu");
	}

	// Catch a sldier change
	public void CatchSliderChange(int dim_idx, decimal val) {
		dataHandler.UpdateDimXBy (2 + (dim_idx - 1), val);
		grapherManager.DrawPoints (grapherManager.createPointsFrom (dataHandler.state));
	}

	// Clicked on the checkboxes
	public void OnClickCheckbox (Text target_text) {
		if (target_text.text == "") target_text.text = "x";
		else target_text.text = "";
		UpdateCheckboxes ();
	}

	// Updated the checkboxes
	public void UpdateCheckboxes () {
		activateRotation = (activateRotationToggle.text == "x");
		fullGraphics = (fullGraphicsToggle.text == "x");
	}

	//
	public void UpdateDimsSlider (SliderManager slider, Dimension dim) {
		slider.UpdateDim(dim);
		slider.UpdateValue(dim.value);
	}

	//
	public void UpdateNavigationDropdowns (Dimension[] dims) {
		UpdateDropdown (dim1StateDropdown, dims);
		UpdateDropdown (dim2StateDropdown, dims);
		dim1StateDropdown.captionText.text = dims[0].name;
		dim2StateDropdown.captionText.text = dims[1].name;
	}

	// 
	private void UpdateDropdown (Dropdown dropdown, Dimension[] dims) {
		dropdown.ClearOptions ();
		dropdown.options.Add (new Dropdown.OptionData () { text = dims [2].name });
		dropdown.options.Add (new Dropdown.OptionData () { text = dims [3].name });
		dropdown.options.Add (new Dropdown.OptionData () { text = "" });
		dropdown.value = 2;
		dropdown.options.RemoveAt (2);
	}

	// Click on -
	public void ClickOnMinus () {
		dataHandler.state.DownMainDimsResolutions ();
		grapherManager.DrawPoints (grapherManager.createPointsFrom (dataHandler.state));
		Dimension[] dims = dataHandler.state.dimensions;
		resolutionSSBSManager.UpdateName (dims[0].GetRes () + " x " + dims[1].GetRes ());
	}

	// Click on +
	public void ClickOnPlus () {
		dataHandler.state.UpMainDimsResolutions ();
		grapherManager.DrawPoints (grapherManager.createPointsFrom (dataHandler.state));
		Dimension[] dims = dataHandler.state.dimensions;
		resolutionSSBSManager.UpdateName (dims[0].GetRes () + " x " + dims[1].GetRes ());
	}

	// Navigation dropdown
	public void ChangeStateViaNavigation (GameObject go_dropdown) {
		int currentValue = go_dropdown.GetComponent<Dropdown> ().value;
		if (currentValue == 2) 
			return;
		
		int idx1 = Int32.Parse (go_dropdown.name.Substring(3)) - 1;
		int idx2 = 2 + currentValue;

		State prev = dataHandler.state.Duplicate ();
		transition.SetToUpdateState ();
		transition.SetToNone ();
		dataHandler.state.goToGivenState (idx1, idx2);

		CreateTransitions (prev, dataHandler.state);

		dim1StateDropdown.interactable = false;
		dim2StateDropdown.interactable = false;
	}
}
