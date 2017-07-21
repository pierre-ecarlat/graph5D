using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorConverter {

	// Get the percentage given the min and max possible values
	public static float getPercentage (float val, float minVal, float maxVal) {
		return ((val - minVal) * 360) / (maxVal - minVal);
	}

	// Convert a HSV color to RGB
	public static Color HSVToRGB (float h, float s, float v) {
		h /= 60f;
		float c = v * s;
		float m = v - c;
		float X = c * (1 - Mathf.Abs ((h % 2) - 1));

		if 		(h <= 1) { return new Color (c+m, X+m, 0+m); }
		else if (h <= 2) { return new Color (X+m, c+m, 0+m); }
		else if (h <= 3) { return new Color (0+m, c+m, X+m); }
		else if (h <= 4) { return new Color (0+m, X+m, c+m); }
		else if (h <= 5) { return new Color (X+m, 0+m, c+m); }
		else			 { return new Color (c+m, 0+m, X+m); }
	}
}
