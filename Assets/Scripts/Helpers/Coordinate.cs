using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinate {

	// Parameters
	public decimal c1, c2;

	public Coordinate (decimal c1, decimal c2) {
		this.c1 = c1;
		this.c2 = c2;
	}

	public Coordinate (float c1, float c2) {
		this.c1 = (decimal)c1;
		this.c2 = (decimal)c2;
	}

	// Need to override the hashcode to use it as dictionary key
	// /!\ Can be more than 10000..
	public override int GetHashCode() {
		return (int)(this.c1*10000) ^ (int)(this.c2*10000);
	}
	public override bool Equals(object obj) {
		return Equals (obj as Coordinate);
	}
	public bool Equals(Coordinate c) {
		return c != null && c.c1 == this.c1 && c.c2 == this.c2;
	}

	// Indexer for the class
	public decimal this[int index] {
		get {
			if (index == 0) return this.c1;
			if (index == 1) return this.c2;
			return 0m;
		}
		set {
			if (index == 0) this.c1 = value;
			if (index == 1) this.c2 = value;
		}
	}
}
