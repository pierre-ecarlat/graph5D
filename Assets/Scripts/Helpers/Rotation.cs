using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation {

	// Rotation orientation (prev: left, next: right, none: none)
	private enum Rotate { None, Prev, Next };

	// Parameters
	private Rotate type;
	private int angle;
	private int nbOfFrames;
	private bool needToDraw;

	public Rotation(int angle, int nbOfFrames, bool needToDraw) {
		this.type = Rotate.None;
		this.angle = angle;
		this.nbOfFrames = nbOfFrames;
		this.needToDraw = needToDraw;
	}

	// Getters
	public int GetNbOfFrames () { return this.nbOfFrames; }
	public int GetAngleRotation () { return this.angle; }
	public bool IsPrev () { return type == Rotate.Prev; }
	public bool IsNone () { return type == Rotate.None; }
	public bool DoNeedToDraw () { return this.needToDraw; }

	// Setters
	public void SetToPrev () { type = Rotate.Prev; }
	public void SetToNext () { type = Rotate.Next; }
	public void SetToNone () { type = Rotate.None; }
}
