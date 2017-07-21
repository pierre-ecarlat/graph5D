using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class State {

	// States cycle
	private LinkedList<int[]> cycle;
	public LinkedListNode<int[]> current;

	// The dimensions of the state (in the right order)
	public Dimension[] dimensions;

	public State(Dimension[] dimensions, int cycleIndex = 0) {
		cycle = new LinkedList<int[]> ();
		cycle.AddLast (new int[] { 0, 1, 2, 3 });
		cycle.AddLast (new int[] { 0, 2, 1, 3 });
		cycle.AddLast (new int[] { 0, 3, 1, 2 });
		cycle.AddLast (new int[] { 2, 3, 1, 0 });
		cycle.AddLast (new int[] { 2, 1, 3, 0 });
		cycle.AddLast (new int[] { 3, 1, 2, 0 });
		current = GetCycleNodeAtIndex(cycleIndex);
		
		this.dimensions = dimensions;
	}

	// Go to the next element of the cycle
	public void goToNext () {
		int[] prevOrders = current.Value;
		current = current.Next ?? current.List.First;
		dimensions = UpdateDimensions (current);
	}

	// Go to the previous element of the cycle
	public void goToPrevious () {
		int[] prevOrders = current.Value;
		current = current.Previous ?? current.List.Last;
		dimensions = UpdateDimensions (current);
	}

	// Go to a given state 
	public void goToGivenState (int idx1, int idx2) {
		int[] newOrders = (int[])current.Value.Clone();
		newOrders [idx1] = current.Value [idx2];

		current = GetCycleFromMainDims (newOrders);
		dimensions = UpdateDimensions (current);
	}

	// Set to minimal resolution
	public void SetMinResolutions() {
		this.dimensions [0].SetToMinResolution ();
		this.dimensions [1].SetToMinResolution ();
	}

	// Update the state's dimension
	public void UpMainDimsResolutions() {
		dimensions [0].UpResolution ();
		dimensions [1].UpResolution ();
	}

	// Update the state's dimension
	public void DownMainDimsResolutions() {
		dimensions [0].DownResolution ();
		dimensions [1].DownResolution ();
	}

	// Get the orders of the current cycle
	public decimal[] Regularize (decimal[] v) {
		return new decimal[] { v[Array.IndexOf(current.Value, 0)], v[Array.IndexOf(current.Value, 1)], 
							   v[Array.IndexOf(current.Value, 2)], v[Array.IndexOf(current.Value, 3)] };
	}

	// Get current dims
	public Coordinate GetCurrentDims() {
		return new Coordinate (dimensions[2].value, dimensions[3].value);
	}

	// Returns the dimensions in the right order according to the given state
	public Dimension[] UpdateDimensions (LinkedListNode<int[]> node) {
		Dimension[] dims = new Dimension[4];
		for (int i = 0; i < dimensions.Length; i++)
			for (int j = 0; j < dimensions.Length; j++)
				if (dimensions [j].index == node.Value[i]) 
					dims [i] = dimensions[j];
		return dims;
	}

	// Duplicate
	public State Duplicate() {
		Dimension[] _d = new Dimension[dimensions.Length];
		for (int i = 0; i < dimensions.Length; i++)
			_d[i] = dimensions[i].Duplicate ();
		return new State (_d, GetIndexFromNode(current));
	}


	// FINDERS

	// Get the node of the cycle at a given index
	private LinkedListNode<int[]> GetCycleNodeAtIndex (int idx) {
		LinkedListNode<int[]> lln = cycle.First;
		for (int i = 0; i < idx; i++) lln = lln.Next;
		return lln;
	}

	// Get the index of the cycle containing a given node
	private int GetIndexFromNode (LinkedListNode<int[]> node) {
		LinkedListNode<int[]> lln = cycle.First;
		for (int i = 0; i < cycle.Count; i++) {
			if (lln.Value.SequenceEqual(node.Value)) return i;
			lln = lln.Next;
		}
		return -1;
	}

	// Returns the index of the cycle containing the main dimensions (any order)
	private LinkedListNode<int[]> GetCycleFromMainDims (int[] mainDims) {
		LinkedListNode<int[]> lln = cycle.First;
		for (int i = 0; i < cycle.Count; i++) {
			if ((lln.Value [0] == mainDims [0] && lln.Value [1] == mainDims [1]) ||
			    (lln.Value [1] == mainDims [0] && lln.Value [0] == mainDims [1])) 
				return lln;
			lln = lln.Next;
		}
		return null;
	}
}



