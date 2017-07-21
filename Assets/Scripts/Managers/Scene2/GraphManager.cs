using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Assertions;

public class GraphManager : MonoBehaviour {

	// Graphic display
	private Transform graph;
	private Transform grid;

	// Managers
	private GridManager gridManager;

	// The data handler
	private Data dataHandler;
	private Dictionary<Coordinate, PointsScatter> data;
	private Formula.Function function;

	// All the points of the graph
	private ParticleSystem.Particle[] points;
	public PointsScatter pointsValues;
	public PointsScatter[] transitionalPointsValues;



	public void Initialize (Data dataHandler, Formula.Function function) {
		foreach (Transform t in gameObject.transform) {
			if (t.name == "Graph") graph = t;
			if (t.name == "Grid") grid = t;
		}

		gridManager = grid.GetComponent<GridManager> ();

		this.dataHandler = dataHandler;
		this.data = this.dataHandler.data;
		this.function = function;

		UpdateGrapherRatio ();

		gridManager.Initialize (dataHandler);
	}


	// Update the values of the graph to fit with the data
	public void DrawPoints (PointsScatter values) {
		points = new ParticleSystem.Particle[values.nbPoints()];
		int idx = 0;
		foreach(KeyValuePair<Coordinate, float> p in values.points) {
			points [idx].position = new Vector3 ((float)p.Key[0], p.Value, (float)p.Key[1]);
			float h = ColorConverter.getPercentage (p.Value, dataHandler.minMagnitude, dataHandler.maxMagnitude);
			points[idx].startColor = ColorConverter.HSVToRGB(h, 0.5f, 0.5f);
			points[idx].startSize = 0.1f;
			idx++;
		}
		graph.GetComponent<ParticleSystem> ().SetParticles(points, points.Length);

		pointsValues = values;
	}

	// Get the data between two dimensions (excludes the current state, includes the next one)
	public void GetTransitionalData (State prev, State next, int nbOfFramesH) {
		// First and last points
		PointsScatter firstPoints = pointsValues;
		PointsScatter lastPoints = createPointsFrom(next);

		// Normalize the first or last points depending on the resolutions
		preparePoints (ref firstPoints, prev, ref lastPoints, next);

		// Transitional points
		transitionalPointsValues = new PointsScatter[nbOfFramesH];

		// Get the steps for the position and magnitude of each point
		int idx = 0;
		decimal[,] posSteps = new decimal[firstPoints.nbPoints(), 2];
		float[] magnSteps = new float[firstPoints.nbPoints()];
		Assert.AreEqual(firstPoints.nbPoints(), lastPoints.nbPoints(), "/!\\ Fatal: Dimension doesn't match.");
		foreach(KeyValuePair<Coordinate, float> p in firstPoints.points) {
			posSteps [idx, 0] = (lastPoints.points.Keys.ElementAt(idx)[0] - p.Key[0]) / nbOfFramesH;
			posSteps [idx, 1] = (lastPoints.points.Keys.ElementAt(idx)[1] - p.Key[1]) / nbOfFramesH;
			magnSteps[idx] = (lastPoints.points.Values.ElementAt(idx) - p.Value) / (float)nbOfFramesH;
			idx++;
		}
		
		for (int f = 0; f < nbOfFramesH - 1; f++) {
			PointsScatter nextPoints = new PointsScatter ();

			idx = 0;
			foreach(KeyValuePair<Coordinate, float> p in firstPoints.points) {
				nextPoints.points.Add (new Coordinate(p.Key[0] + posSteps[idx, 0], p.Key[1] + posSteps[idx, 1]),
									   p.Value + magnSteps[idx]);
				idx++;
			}
			firstPoints = nextPoints;
			transitionalPointsValues [f] = nextPoints;
		}
	}

	// Rotate the graph horizontally for a given angle
	public void RotateBy (Vector3 graphPos, float angle) {
		transform.RotateAround (graphPos, Vector3.up, angle);
	}

	// Rotate the graph vertically for a given angle
	public void RotateOverBy (Vector3 graphPos, float angle) {
		transform.RotateAround (graphPos, Vector3.right, angle);
	}

	// Move the graph with the 3D mouse
	public void MoveGraph (Quaternion rotation, Vector3 translation) {
		Vector3 trans = Vector3.Scale(translation, new Vector3 (0.5f, 0.5f, 0.5f));
		transform.position = transform.position + trans;

		Vector3 graphPos = dataHandler.getGraphPosition ();
		float angleH = rotation.y * 250;
		float angleV = - rotation.z * 250;
		RotateBy (graphPos, angleH);
		RotateOverBy (graphPos, angleV);
	}

	// Update the names in the grid UI
	public void UpdateGridNames(State state) {
		gridManager.UpdateNames (state);
	}


	// Update the size of the graph
	private void UpdateGrapherRatio () {
		float minDimAxis = dataHandler.getMinDimAxis ();
		float maxDimAxis = dataHandler.getMaxDimAxis ();

		float ratio = 1f / (maxDimAxis - minDimAxis);
		Transform particles = graph.GetComponent<ParticleSystem> ().transform;
		graph.localScale = new Vector3(ratio, ratio, ratio);
		particles.GetComponent<ParticleSystemRenderer> ().minParticleSize = 0.1f * (maxDimAxis - minDimAxis);
		grid.position = new Vector3(grid.position[0] + minDimAxis * ratio, 0f, grid.position[1] + minDimAxis * ratio);
	}

	// Merge or divide points to make them match the new shape (admit that one of the coordinate remains the same)
	private void preparePoints(ref PointsScatter prevPoints, State prev,
							   ref PointsScatter nextPoints, State next) {
		// Study the resolutions
		Dimension[] prevDims = new Dimension[] { prev.dimensions[0], prev.dimensions[1] };
		Dimension[] nextDims = new Dimension[] { next.dimensions[0], next.dimensions[1] };
		int[] diffDims = new int[] { nextDims[0].GetRes () - prevDims[0].GetRes (),
									 nextDims[1].GetRes () - prevDims[1].GetRes () };

		// Same resolutions, no need to normalize
		if (diffDims[0] == 0 && diffDims[1] == 0)
			return;

		// Normalize
		if (diffDims[0] > 0 || diffDims[1] > 0) 
			normalizePointsShapeFromTo (ref prevPoints, prevDims, nextDims);
		else 
			normalizePointsShapeFromTo (ref nextPoints, nextDims, prevDims);
	}

	// Normalize the shape of the given points from state prev to next
	private void normalizePointsShapeFromTo(ref PointsScatter points, 
											Dimension[] prev, Dimension[] next) {
		PointsScatter newValues = new PointsScatter ();
		int[] diffDims = new int[] { next[0].GetRes () - prev[0].GetRes (),
									 next[1].GetRes () - prev[1].GetRes () };

		for (int x = - (diffDims[0] / 2); x <= prev[0].GetRes () + (diffDims[0] / 2); x++) {
			float posX = getPosFrom (prev[0], next[0], diffDims[0], x);
			float coordX = getCoordFrom(posX, prev[0]);

			for (int z = - (diffDims[1] / 2); z <= prev[1].GetRes () + (diffDims[1] / 2); z++) {
				float posZ = getPosFrom (prev[1], next[1], diffDims[1], z);
				float coordZ = getCoordFrom(posZ, prev[1]);
				float magnitude = points.points [new Coordinate (coordX, coordZ)];
				newValues.points.Add (new Coordinate (posX + (float)prev[0].minVal, posZ + (float)prev[1].minVal), magnitude);
			}
		}
		points = newValues;
	}

	// Create the points giving a state
	public PointsScatter createPointsFrom(State s) { 
		PointsScatter pts = new PointsScatter ();
		for (int x = 0; x <= s.dimensions[0].GetRes (); x++) {
			for (int z = 0; z <= s.dimensions[1].GetRes (); z++) {
				float _x = Convert.ToSingle (x * s.dimensions[0].GetStep ());
				float _z = Convert.ToSingle (z * s.dimensions[1].GetStep ());
				pts.points.Add (new Coordinate(_x + (float)s.dimensions[0].minVal, _z + (float)s.dimensions[1].minVal), 
								GetM (s, new Coordinate(_x, _z), s.GetCurrentDims()));
			}
		}
		return pts;
	}

	//
	private float getPosFrom (Dimension prev, Dimension next, int diff, int value) {
		float pos = (float)(prev.GetStep ());
		if (diff < 0) { pos *= prev.GetRes () / next.GetRes (); }
		return scalePos (pos, ((diff < 0) ? (value + (diff / 2)) : value), prev, next);
	}

	// 
	private float getCoordFrom (float pos, Dimension prev) {
		float coord = Mathf.Round(((pos) / (float)(prev.maxVal - prev.minVal)) * prev.GetRes ()) / (float)prev.GetRes ();
		coord = coord * (float)(prev.maxVal - prev.minVal) + (float)prev.minVal;
		return Mathf.Clamp ((float)coord, (float)prev.minVal, (float)prev.maxVal);
	}

	// 
	private float scalePos (float pos, int ratio, Dimension prev, Dimension next) {
		if (ratio >= 0 && ratio <= prev.GetRes ()) return pos * ratio;
		else if (ratio < 0)                        return - ratio / 10000f;
		else                                       return (float)(prev.maxVal - prev.minVal) + (ratio - prev.GetRes ()) / 10000f;
	}

	// Find Magnitude from data
	private float GetM (State state, Coordinate mainDim, Coordinate addDim) {
		decimal[] coords = state.Regularize (new decimal[] { mainDim[0], mainDim[1], addDim[0], addDim[1] });
		if (function == Formula.Function.None) return GetFromData (coords);
		else 								   return Formula.GetFormula (function, coords);
	}

	// Get the magnitude from the loaded data
	private float GetFromData(decimal[] c) {
		if (data.ContainsKey (new Coordinate(c[2], c[3])) && 
			data[new Coordinate(c[2], c[3])].points.ContainsKey (new Coordinate(c[0], c[1])))
			return data[new Coordinate(c[2], c[3])].points[new Coordinate(c[0], c[1])];
		//Debug.Log (mainDim [0] + " " + mainDim [1] + " " + addDim [0] + " " + addDim [1]);
		return 0f;
	}
}
