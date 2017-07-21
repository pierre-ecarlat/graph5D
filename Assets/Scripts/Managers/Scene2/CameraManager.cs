using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	private Data dataHandler;
	private Camera thisCamera;

	public void Initialize (Data d) {
		dataHandler = d;
		thisCamera = GetComponent<Camera> ();
		Vector3 positionFromGraph = new Vector3 (0.25f, 1.25f, -1.5f);
		Vector3 rotationFromGraph = new Vector3 (45f, 0f, 0f);
		transform.position = positionFromGraph + getRelativeCenterScreen();
		transform.Rotate (rotationFromGraph);
	}

	// Rotate the camera vertically for a given angle
	public void RotateOverBy (Vector3 graphPos, float angle) {
		transform.RotateAround (graphPos, Vector3.right, angle);
	}

	// Moves the camera with the wheel button
	public void UpdateCameraPosition (float [] mousePos) {
		transform.Translate (mousePos[0], mousePos[1], 0f);
	}

	// Update the camera position with mouse wheel
	public void UpdateZoom (float wheel) {
		if (wheel != 0f) {
			float newFOV = thisCamera.fieldOfView - wheel;
			thisCamera.fieldOfView = Mathf.Clamp(newFOV, 10f, 75f);
		}
	}

	// Get the relative center of the screen
	private Vector3 getRelativeCenterScreen() {
		return getCenterScreen () + dataHandler.getGraphPosition();
	}

	// Get the center of the screen
	private Vector3 getCenterScreen() {
		return thisCamera.ScreenToWorldPoint (new Vector3 (Screen.width/2f - 175f, 
														   Screen.height/2f, 
														   thisCamera.nearClipPlane));
	}
}
