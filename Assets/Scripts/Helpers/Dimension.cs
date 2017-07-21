using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Dimension {

	// The values of the resolution
	public int minResolution = 20;
	public int maxResolution = 100;
	public int stepResolution = 10;

	// Parameters
	public int index;
	public string name;
	public decimal minVal, maxVal;
	public decimal value;

	// Resolutions
	private LinkedList<Resolution> resolutions;
	private LinkedListNode<Resolution> currentResolution;
	
	public Dimension (int index, string name, decimal minVal, decimal maxVal, int resolution) {
		this.index = index;
		this.name = name;
		this.minVal = minVal;
		this.maxVal = maxVal;
		this.value = minVal;

		this.resolutions = ComputeResolutions (minVal, maxVal, resolution);
		this.currentResolution = FindCurrentResolution (resolution);
	}


	// Getters
	public int GetRes () { return currentResolution.Value.resolution; }
	public decimal GetStep () { return currentResolution.Value.step; }


	// Update the resolution (to min, to next, to prev)
	public void SetToMinResolution () { currentResolution = resolutions.First; }
	public void UpResolution () { 
		if (currentResolution.Next != null) currentResolution = currentResolution.Next;
	}
	public void DownResolution () {
		if (currentResolution.Previous != null) currentResolution = currentResolution.Previous;
	}

	// Compute the list of resolutions
	private LinkedList<Resolution> ComputeResolutions(decimal min, decimal max, int resolution) {
		LinkedList<Resolution> tmp_resolutions = new LinkedList<Resolution> ();

		if (resolution >= this.minResolution)
			resolution = this.minResolution + (resolution % this.minResolution);
		tmp_resolutions.AddLast (new Resolution (resolution, (decimal)((max - min) / resolution)));

		while (true) {
			Resolution lastRes = tmp_resolutions.Last();
			resolution = lastRes.resolution + this.stepResolution;
			if (resolution > this.maxResolution)
				return tmp_resolutions;
			tmp_resolutions.AddLast (new Resolution (resolution, (decimal)((max - min) / resolution)));
		}
	}

	// Returns the node corresponding to a given resolution
	private LinkedListNode<Resolution> FindCurrentResolution (int resolution) {
		for (var lln = resolutions.First; lln != null; lln = lln.Next) {
			if (lln.Value.resolution == resolution)
				return lln;
		}
		return null;
	}

	// Duplicate
	public Dimension Duplicate() {
		Dimension dim = new Dimension (index, name, minVal, maxVal, currentResolution.Value.resolution);
		dim.value = value;
		return dim;
	}
}
