using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formula {

	// All the implemented functions
	public enum Function { Sine5D, Ripple5D, BellState, None };

	// Returns the magnitude according to the function and the coordinates
	public static float GetFormula(Function f, decimal[] c) {
		string functionName = Function.GetName(f.GetType(), f); 
		return (float)typeof(Formula).GetMethod (functionName).Invoke (null, new[] {c});
	}

	// Returns the text of the function
	public static string GetFormulaText(int number) {
		string functionName = Function.GetName(((Function)number).GetType(), (Function)number); 
		return (string)typeof(Formula).GetMethod (functionName + "Text").Invoke (null, null);
	}

	// Returns the text of the function
	public static string GetFormulaDefaults(int number) {
		string functionName = Function.GetName(((Function)number).GetType(), (Function)number); 
		return (string)typeof(Formula).GetMethod (functionName + "Defaults").Invoke (null, null);
	}



	// Computes a combination of sinus and cosinus functions
	public static float Sine5D(decimal[] c) {
		return ( 0.25f * Mathf.Sin(4 * Mathf.PI * (float)c[0] + 4 * (float)c[2]) * Mathf.Sin(2 * Mathf.PI * (float)c[1] + (float)c[2]) +
			0.10f * Mathf.Cos(3 * Mathf.PI * (float)c[0] + 5 * (float)c[2]) * Mathf.Cos(5 * Mathf.PI * (float)c[1] + 3 * (float)c[2]) +
			0.15f * Mathf.Sin(Mathf.PI * (float)c[0] + 0.6f * (float)c[2])
		) * ((float)c[3]*0.50f + 1);
	}

	// Computes a ripple with an amplificator
	public static float Ripple5D (decimal[] c) {
		float x = (float)c [0] - 0.5f;
		float y = (float)c [1] - 0.5f;
		float squareRadius = (float)(x*x + y*y);
		return Mathf.Sin(15f * Mathf.PI * squareRadius - 2f * (float)c[2]) / (2f + 100f * squareRadius) * ((float)c[3]*0.50f + 1);
	}

	// Computes the bell state
	public static float BellState (decimal[] c) {
		return (1 / 4f) * (1 +
			3 * Mathf.Cos (2 * (float)c [0]) * Mathf.Cos (2 * (float)c [1]) +
			3 * Mathf.Cos (2 * ((float)c [2] + (float)c [3])) +
			Mathf.Sin (2 * (float)c [0]) * Mathf.Sin (2 * (float)c [1]));
	}


	public static string Sine5DText() {
		return "y = (0.25 * sin(4*PI*x1 + 4*x3) * sin(2*PI*x2 + x3) + 0.10f * cos(3*PI*x1 + 5*x3) * cos(5*PI*x2 + 3*x3) + 0.15f * sin(PI*x1 + 0.6*x3)) * (x4*0.5 + 1)";
	}
	public static string Ripple5DText() {
		return "y = sin(15*PI * ((x1 - 0.5)² + (x2 - 0.5)²) - 2*x3) / (2 + 100 * ((x1 - 0.5)² + (x2 - 0.5)²)) * (0.5*x4 + 1)";
	}
	public static string BellStateText() {
		return "y = 0.25 * (1 + 3 * cos(2*x1) * cos(2*x2) + 3 * cos(2*x3 + x4) + sin(2*x1) * sin(2*x2))";
	}
}
