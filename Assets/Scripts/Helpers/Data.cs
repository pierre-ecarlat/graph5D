using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Data {

	// Dimensions
	public State state;

	// Parameters about the data
	public Dictionary<Coordinate, PointsScatter> data;
	public float minMagnitude, maxMagnitude;


	public Data (string[] names, decimal[] minVals, decimal[] maxVals, decimal[] steps) {
		int[] resolutions = new int[steps.Length];
		for (int i = 0; i < steps.Length; i++)
			resolutions[i] = (int)((maxVals[i] - minVals[i]) / steps[i]);
		Initialize (names, minVals, maxVals, resolutions);
	}

	public Data (string[] names, decimal[] minVals, decimal[] maxVals, int[] resolutions) {
		Initialize (names, minVals, maxVals, resolutions);
	}

	private void Initialize (string[] names, decimal[] minVals, decimal[] maxVals, int[] resolutions) {
		Dimension[] dims = new Dimension[names.Length];
		for (int i = 0; i < names.Length; i++) 
			dims [i] = new Dimension (i, names[i], minVals[i], maxVals[i], resolutions[i]);

		this.data = new Dictionary<Coordinate, PointsScatter> ();

		this.minMagnitude = float.PositiveInfinity;
		this.maxMagnitude = float.NegativeInfinity;

		this.state = new State (dims);
	}


	// Computes the min and max magnitudes
	public void UpdateCharacteristics (Formula.Function function) {
		if (function == Formula.Function.None) {
			foreach (Coordinate d in data.Keys) {
				foreach (Coordinate c in data[d].points.Keys) {
					if (this.minMagnitude > this.data [d].points [c]) this.minMagnitude = this.data [d].points [c];
					if (this.maxMagnitude < this.data [d].points [c]) this.maxMagnitude = this.data [d].points [c];
				}
			}
		} else {
			this.minMagnitude = -0.5f;
			this.maxMagnitude = 0.5f;
		}
	}

	// Returns the total number of points of the data
	public int getTotalNbOfPoints () {
		Dimension[] d = state.dimensions;
		return (d[0].GetRes () + 1) * (d[1].GetRes () + 1) * (d[2].GetRes () + 1) * (d[3].GetRes () + 1);
	}

	// Returns the position of the graph
	public Vector3 getGraphPosition () {
		// The graph has been scaled to [0;1] previously
		float ratio = 1f / (getMaxDimAxis () - getMinDimAxis ());
		return new Vector3 (0.5f + getMinDimAxis () * ratio, 0f, 0.5f + getMinDimAxis () * ratio);
	}

	// Update the value of a given additional dimension
	public void UpdateDimXBy(int x_idx, decimal val) {
		state.dimensions[x_idx].value = val;
	}

	// Returns the min value over the axis
	public float getMinDimAxis () {
		Dimension[] dims = state.dimensions;
		decimal min = dims [0].minVal;
		for (int i = 1; i < state.dimensions.Length; i++)
			if (min > dims [i].minVal) min = dims [i].minVal;
		return Convert.ToSingle (min);
	}

	// Returns the max value over the axis
	public float getMaxDimAxis () {
		Dimension[] dims = state.dimensions;
		decimal max = dims [0].maxVal;
		for (int i = 1; i < state.dimensions.Length; i++)
			if (max < dims [i].maxVal) max = dims [i].maxVal;
		return Convert.ToSingle (max);
	}
}
