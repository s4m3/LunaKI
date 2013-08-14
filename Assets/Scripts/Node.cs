using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : Object {

	public int id;
	public float radius;
	public float inclination;
	public float azimuth;
	public double Cost;
	public double DistanceToGoal;
	
	public Vector3 position;
	
	public List<Node> Successors;
	
	public string[] successorIDs;

	
	
	public float f;
	public int parentID = -1;
	
	public float currentDistanceToNode;
	
	public bool Equals(Node other)
	{
		return (this.id == other.id);
	}
}
