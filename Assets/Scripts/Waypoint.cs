using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waypoint : ScriptableObject {
	public int id;
	public float radius;
	public float inclination;
	public float azimuth;
	
	public Vector3 originalPoint;
	
	public List<Waypoint> Successors;
	
	public float f;
}
