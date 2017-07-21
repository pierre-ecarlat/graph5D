using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsScatter {

	// Points
	public Dictionary<Coordinate, float> points;

	public PointsScatter () {
		this.points = new Dictionary<Coordinate, float> ();
	}

	// Returns the number of points in the cloud
	public int nbPoints () { return points.Count; }
}
