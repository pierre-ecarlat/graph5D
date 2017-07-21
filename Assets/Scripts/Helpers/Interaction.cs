using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceNavigatorDriver;
using UnityEngine.UI;

public class Interaction {

	// Enumerates the interaction types: Mouse, Mouse3D, etc
	public enum InteractionType { Mouse, Mouse3D };

	// Enumerates all the possible actions
	public enum Action { None,
						 UpdateStateToLeft, UpdateStateToRight, 
						 UpdateStateToLeftStatic, UpdateStateToRightStatic, 
						 RotateOverToUp, RotateOverToDown,
						 OnMovingCamera, OnStopMovingCamera };

	// Handler
	private InteractionType type;

	public float prevAngle = 0f;				// Save the last angle for 3D mouse velocity
	public float sensibility3D = 0.5f;			// Sensibility of the 3D mouse
	public float sensibility2D = 0.1f;			// Sensibility of the 2D mouse
	public int sensibilityWheel = 20;			// Sensibility of the wheel for the 2D mouse
	public float max3DValue = 0.014f;			// Value from where we detect the end of the 3D mouse rotation
	public float velocity3DThresh = 0.002125f;	// The threshold from when the velocity is enough to change state


	public Interaction(InteractionType type) {
		this.type = type;
	}

	// Returns the current action
	public Action GetAction () {
		if 		(Input.GetKeyDown (KeyCode.LeftArrow)) 	return Action.UpdateStateToLeft;
		else if (Input.GetKeyDown (KeyCode.RightArrow)) return Action.UpdateStateToRight;
		else if (Input.GetKeyDown (KeyCode.UpArrow)) 	return Action.RotateOverToUp;
		else if (Input.GetKeyDown (KeyCode.DownArrow)) 	return Action.RotateOverToDown;
		else if (Input.GetMouseButtonDown(2)) 			return Action.OnMovingCamera;
		else if (Input.GetMouseButtonUp(2)) 			return Action.OnStopMovingCamera;
		else if (type == InteractionType.Mouse3D) {
			float angleH = SpaceNavigator.Rotation.y;
			float diff = Mathf.Abs (angleH - prevAngle);
			if (angleH > max3DValue && diff > velocity3DThresh) {
				GameObject.Find ("Test").GetComponent<Text> ().text = diff.ToString();
				return Action.UpdateStateToRightStatic;
			} else if (angleH < -max3DValue && diff > velocity3DThresh) { 
				GameObject.Find ("Test").GetComponent<Text> ().text = diff.ToString();
				return Action.UpdateStateToLeftStatic;
			} else {
				prevAngle = angleH;
			}
		} 
		return Action.None;
	}

	// 3D Mouse rotation
	public Quaternion GetRotation() {
		if (type == InteractionType.Mouse3D) {
			return Quaternion.LerpUnclamped(Quaternion.identity, SpaceNavigator.Rotation, sensibility3D);
			//return SpaceNavigator.Rotation;
		}
		return new Quaternion(0,0,0,0);
	}

	// 3D Mouse translation
	public Vector3 GetTranslation() {
		if (type == InteractionType.Mouse3D) {
			Vector3 t = SpaceNavigator.Translation;
			return new Vector3(t.x * sensibility3D, t.y * sensibility3D, t.z * sensibility3D);
			//return SpaceNavigator.Translation;
		}
		return new Vector3(0,0,0);
	}

	// Returns the mouse position
	public float[] GetMouseAxis () {
		float v = Input.GetAxis ("Mouse X") * sensibility2D * (-1);
		float h = Input.GetAxis ("Mouse Y") * sensibility2D * (-1);
		return new float[] {v, h};
	}

	// Returns the wheel acceleration
	public float GetMouseWheel () {
		return Input.GetAxis ("Mouse ScrollWheel") * sensibilityWheel; 
	}
}
