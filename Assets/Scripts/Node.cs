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
	
	public GameObject nodeSpherePrefab;
	public GameObject ns;

	public void ShowNode()
	{
		//GameObject np = nodeSphere;
		ns = (GameObject) GameObject.Instantiate(nodeSpherePrefab, position, Quaternion.identity);
		foreach(Node n in Successors)
		{
			//Debug.DrawLine(position, n.position, Color.white, 10000000, true);
		}
		//Debug.Log (position + " vs. " + ns.transform.position);
	}
	
	public void UpdatePosition()
	{
		if(this.position == ns.transform.position) return;
		
		
		Debug.Log (id+" old position: " + this.position + " to new: " + ns.transform.position);
		this.position = ns.transform.position;
		foreach(Node n in Successors)
		{
			Debug.DrawLine(position, n.position, Color.red, 10000000, true);
		}
	}
	
	public void UpdateSuccessors(List<int> deletedNodesIDs, List<Node> Nodes)
	{
		foreach(int id in deletedNodesIDs) 
		{
			for(int i=0; i<successorIDs.Length; i++) 
			{
				if(successorIDs[i] != "deleted") {
					if(System.Convert.ToInt32(successorIDs[i]) == id) {
						successorIDs[i] = "deleted";
					}
				}
			}
		}

		Successors.Clear();
		foreach(string currentID in successorIDs) {
			if(currentID != "deleted") {
				Node nodeToAdd = Nodes.Find(delegate(Node nodeToFind)
            	{
                	return nodeToFind.id == System.Convert.ToInt32(currentID);
            	});
				Successors.Add(nodeToAdd);
			}
		}
		successorIDs = new string[Successors.Count];
		for(int j=0; j<Successors.Count; j++) {
			successorIDs[j] = Successors[j].id.ToString();
			//Debug.Log (j+": "+successorIDs[j]);
		}
	}
	
	public float f;
	public int parentID = -1;
	
	public float currentDistanceToNode;
	
	public bool Equals(Node other)
	{
		return (this.id == other.id);
	}
	
	public void Reset()
	{
		this.parentID = -1;
	}
}
