using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

	// Main sections
	private TextMesh grid_axis1, grid_axis2;
	private TextMesh grid_axis1_minVal, grid_axis2_minVal;
	private TextMesh grid_axis1_maxVal, grid_axis2_maxVal;

	public void Initialize(Data dataHandler) {
		foreach (Transform t in gameObject.transform) {
			if (t.gameObject.name == "Dim1Text") grid_axis1 = t.GetComponent<TextMesh> ();
			if (t.gameObject.name == "Dim2Text") grid_axis2 = t.GetComponent<TextMesh> ();
			if (t.gameObject.name == "Dim1MinVal") grid_axis1_minVal = t.GetComponent<TextMesh> ();
			if (t.gameObject.name == "Dim2MinVal") grid_axis2_minVal = t.GetComponent<TextMesh> ();
			if (t.gameObject.name == "Dim1MaxVal") grid_axis1_maxVal = t.GetComponent<TextMesh> ();
			if (t.gameObject.name == "Dim2MaxVal") grid_axis2_maxVal = t.GetComponent<TextMesh> ();
		}

		UpdateNames (dataHandler.state);
		UpdateValues (dataHandler);
	}

	// Update the names of the two main dimensions of the grid
	public void UpdateNames (State state) {
		grid_axis1.text = state.dimensions[0].name;
		grid_axis2.text = state.dimensions[1].name;
	}

	// Update the values of the grid (min and max)
	private void UpdateValues (Data dataHandler) {
		float minDimAxis = dataHandler.getMinDimAxis ();
		float maxDimAxis = dataHandler.getMaxDimAxis ();

		grid_axis1_minVal.text = minDimAxis.ToString();
		grid_axis2_minVal.text = minDimAxis.ToString();
		grid_axis1_maxVal.text = maxDimAxis.ToString();
		grid_axis2_maxVal.text = maxDimAxis.ToString();
	}
}
