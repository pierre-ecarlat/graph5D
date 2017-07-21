using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StereoCamerasManager : MonoBehaviour {
	public Camera cameraL;
	public Camera cameraR;
	public Camera camera2D;

	public float displacement;

	void Start () {
		if (displacement == null)
			displacement = 0f;
	}

	public void Initialize (float d) {
		displacement = d;
		AdjustCamera ();
	}

	void Update () {
		if (Input.GetKeyDown ("p")) {
			displacement += 0.01f;
		}
		if (Input.GetKeyDown ("o")) {
			displacement -= 0.01f;
		}
		GameObject.Find ("Test").GetComponent<Text> ().text = displacement.ToString();
		AdjustCamera ();
	}

	public void AdjustCamera () {
		cameraL.transform.position = camera2D.transform.position;
		cameraL.transform.rotation = camera2D.transform.rotation;
		cameraR.transform.position = camera2D.transform.position;
		cameraR.transform.rotation = camera2D.transform.rotation;

		//cameraL.transform.RotateAround(new Vector3 (0f, 0f, 0f), Vector3.up,  displacement);
		//cameraR.transform.RotateAround(new Vector3 (0f, 0f, 0f), Vector3.up, -displacement);
		cameraL.transform.position = cameraL.transform.position + (Vector3.left * -displacement);
		cameraR.transform.position = cameraR.transform.position + (Vector3.left * displacement);
	}

	public void MatchAngles (Transform target) {
		target.rotation = camera2D.transform.rotation;
	}

	// Get the center of the screen
	public Vector3 getCenterScreen () {
		return cameraR.ScreenToWorldPoint (new Vector3 (Screen.width/2f - 175f, 
										   Screen.height/2f, 
										   cameraR.nearClipPlane));
	}

	public void SetStereoMode (bool _stereo) {
		if (_stereo) {
			camera2D.gameObject.SetActive (false);
			cameraL.gameObject.SetActive (true);
			cameraR.gameObject.SetActive (true);
		} else {
			camera2D.gameObject.SetActive (true);
			cameraL.gameObject.SetActive (false);
			cameraR.gameObject.SetActive (false);
		}
	}

	// Moves the camera with the wheel button
	public void UpdateCameraPosition (float [] mousePos) {
		foreach (Camera cam in new Camera[] { cameraL, cameraR, camera2D }) {
			cam.transform.Translate (mousePos [0], mousePos [1], 0f);
		}
	}

	// Update the camera position with mouse wheel
	public void UpdateZoom (float wheel) {
		if (wheel != 0f) {
			float newFOV = cameraL.fieldOfView - wheel;
			foreach (Camera cam in new Camera[] { cameraL, cameraR, camera2D }) {
				cam.fieldOfView = Mathf.Clamp(newFOV, 10f, 170f);
			}
			AdjustCamera ();
		}
	}

	public void UpdateStereo (bool _stereo) {
		SetStereoMode (_stereo);
	}

	public void SetCanvasOnRight (Transform target, bool _stereo) {
		if (_stereo) {
			target.GetComponent<RectTransform>().localScale = new Vector3 (0.8f, 0.8f, 0.8f);
			target.GetComponent<RectTransform>().localPosition = new Vector3 (137f, 0f, 0f);
		} else {
			target.GetComponent<RectTransform>().localScale = new Vector3 (1f, 1f, 1f);
			target.GetComponent<RectTransform>().localPosition = new Vector3 (363f, 0f, 0f);
		}
	}
}

