using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentObjectsManager : MonoBehaviour {

	// Settings panel
	public ComputeSettingsManager computeSettingsManager;
	public LoadSettingsManager loadSettingsManager;

	// Data with the pre-initialized settings
	public Data data;

	// The selected type for the data
	public Formula.Function function;

	// Settings for the computing data
	public string[] names;
	public decimal[] mins, maxs;
	public int[] resolutions;

	// Settings for the load option
	public bool beginByMagnitude;

	// Settings for the load option
	public bool interaction3D;
}
