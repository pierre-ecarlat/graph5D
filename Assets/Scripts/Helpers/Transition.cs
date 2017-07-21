using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition {

	// Enumerates the types of transition
	public enum TransitionType { None, UpdateState, RotateOver };

	// Handler
	private Dictionary<TransitionType, Rotation> transitions;
	private TransitionType transitionType;

	// Counter for the frames
	public int currentFrame;

	public Transition () {
		transitions = new Dictionary<TransitionType, Rotation> ();
		transitions.Add (TransitionType.None, null);
		transitions.Add (TransitionType.UpdateState, new Rotation (90, 60, true));
		transitions.Add (TransitionType.RotateOver,  new Rotation (45, 10, false));

		transitionType = TransitionType.None;
		currentFrame = 0;
	}

	// Setters
	public void SetToUpdateState () { transitionType = TransitionType.UpdateState; }
	public void SetToRotateOver ()  { transitionType = TransitionType.RotateOver; }
	public void SetToPrev () { transitions [transitionType].SetToPrev (); }
	public void SetToNext () { transitions [transitionType].SetToNext (); }
	public void SetToNone () { transitions [transitionType].SetToNone (); }

	// Getters
	public bool isRotating () { return transitions[transitionType].IsNone () == false; }
	public int isNegative () { return transitions[transitionType].IsPrev () ? -1 : 1; }
	public bool isADrawingTransition () { return transitions[transitionType].DoNeedToDraw (); }
	public int GetNbOfFrames () { return transitions[transitionType].GetNbOfFrames (); }
	public int GetAngleRotation () { return transitions[transitionType].GetAngleRotation (); }

	// Bools
	public bool onTransition ()  { return transitionType != TransitionType.None; }
	public bool onUpdateState () { return transitionType == TransitionType.UpdateState; }
	public bool onRotateOver ()  { return transitionType == TransitionType.RotateOver; }


	// Returns true if the rotation is over
	public bool RotationIsOver () {
		if (currentFrame >= GetNbOfFrames ()) return true;
		else 								  return false;
	}

	// Update the frames counter
	public void GoToNextFrame () { currentFrame++; }

	// Reset the transition
	public void Reset () {
		transitions[transitionType].SetToNone ();
		transitionType = TransitionType.None;
		currentFrame = 0;
	}
}
