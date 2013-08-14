using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinder : MonoBehaviour {
	
	public GameObject waypointCreaterPrefab;
	public GameObject Planet;
	private int maxAmountOfSuccessors = 10;
	
	public GameObject TestSphere;
	
	private WaypointCreater wpCreator;
	public List<Waypoint> waypoints;
	public List<Node> Nodes;
	

	public void Init () {
		//are there nodes already stored?
		//TODO: as coroutine... oder ganzes init als coroutine
		LoadNodesFromFileAndConvert();
		if(Nodes.Count == 0)
		{
			print ("here");
			GameObject obj = (GameObject) GameObject.Instantiate(waypointCreaterPrefab);
			wpCreator = obj.GetComponent<WaypointCreater>();
			wpCreator.planet = Planet;
			wpCreator.CreateWayPoints();
			this.waypoints = wpCreator.Waypoints;
			CreateNodesFromWaypoints();
		}
	
		float time = Time.realtimeSinceStartup;
		StartCoroutine(StartPathfind(Nodes[0], Nodes[4823]));
		print (Time.realtimeSinceStartup - time);
	}
	
	void Update () {
	
	}
	
	private void LoadNodesFromFileAndConvert()
	{
		List<string> stringList = Persist.deserializeNodesFromFile("nodes.dat");
		string[] arr;
		string[] posArr;
		Node node;
		foreach(string current in stringList)
		{
			node = new Node();
			arr = current.Split('_');
			posArr = arr[1].Split(';');
			
			node.id = System.Convert.ToInt32(arr[0]);
			node.position = new Vector3( (float) System.Convert.ToDouble(posArr[0]), 
											(float) System.Convert.ToDouble(posArr[1]), 
											(float) System.Convert.ToDouble(posArr[2]));
			
			node.successorIDs = arr[2].Split('*');
			node.Cost = double.MaxValue;
			node.DistanceToGoal = double.MaxValue;
			
			Nodes.Add(node);
		}
		
		//after creating all nodes, insert successors
		foreach(Node n in Nodes)
		{
			Node nodeToFind;
			n.Successors = new List<Node>();
			foreach(string succString in n.successorIDs)
			{
				nodeToFind = Nodes.Find(
					delegate(Node nd)
	            	{
	                	return nd.id == System.Convert.ToInt32(succString);
	            	});
				n.Successors.Add(nodeToFind);
			}
		}
	}
	
	private void DrawLines(List<Node> nodes)
	{
		for(int l=0; l<nodes.Count-1; l++)
		{
			Debug.DrawLine(nodes[l].position, nodes[l+1].position, Color.red, 10000000, true);
		}
	}
	
	private void CreateNodesFromWaypoints()
	{
		Nodes = new List<Node>();
		foreach(Waypoint wp in waypoints)
		{
			Node node = new Node();
			node.id = wp.id;
			node.position = wp.originalPoint;
			node.Cost = double.MaxValue;
			node.DistanceToGoal = double.MaxValue;
			Nodes.Add(node);
		}
		Node currentNode;
		List<Node> closestSuccessors = new List<Node>();
		for(int i=0; i<Nodes.Count; i++)
		{
			Nodes[i].Successors = new List<Node>();
			for(int j = 0; j<Nodes.Count; j++)
			{
				if(i != j)
				{
					currentNode = Nodes[j];
					currentNode.currentDistanceToNode = euclideanDist(Nodes[i], currentNode);
					closestSuccessors.Add(currentNode);
				}
			}
			closestSuccessors.Sort(delegate(Node a, Node b) {
								return a.currentDistanceToNode.CompareTo(b.currentDistanceToNode);
								});
			Nodes[i].Successors.AddRange(closestSuccessors.GetRange(0, maxAmountOfSuccessors));
			closestSuccessors.Clear();
			//print (Nodes[i].Successors.Count);
			//printList(Nodes[i].Successors);
		}
	}
	
	private void printList(List<Node> liste)
	{
		print ("listcount: " + liste.Count);
		foreach(Node node in liste)
		{
			print ("id: " + node.id +" with parent: "+ node.parentID);
		}
	}
	
	public IEnumerator StartPathfind(Node startNode, Node endNode)
	{
		List<Node> path = FindPath(startNode, endNode);
		foreach(Node adf in path)
		{
			Debug.Log(adf.id);
		}
		DrawLines(path);
//		Instantiate(TestSphere, Nodes[0].position, transform.rotation);
//		Instantiate(TestSphere, Nodes[60].position, transform.rotation);
		yield return null;
	}
	
	public List<Node> FindPath(Node startNode, Node endNode){
		List<Node> openList = new List<Node>();
		List<Node> closedList = new List<Node>();
		Node currentNode;
		startNode.Cost = 0;
		currentNode = startNode;
		openList.Add(currentNode);
		double lowestCost;
		int loop=0;
		while(currentNode.id != endNode.id && loop < 10000){
			loop++;
			expand (currentNode, ref openList, closedList, endNode);
			
			closedList.Add(currentNode);
			
			lowestCost = double.MaxValue;
			Node nodeWithLowestCost = null;
			if(openList.Count <= 0)
				break;
			
			foreach(Node wn in openList) {
				if(wn.DistanceToGoal < lowestCost){
					nodeWithLowestCost = wn;
					lowestCost = wn.DistanceToGoal;
				}
			}
			currentNode = openList.Find(
				delegate(Node nd)
            	{
                	return nd.id == nodeWithLowestCost.id;
            	});
		}
		closedList.Add(currentNode);
		return getPath(currentNode, closedList);
	}

	private void expand(Node expandedNode, ref List<Node> openList, List<Node> closedList, Node endNode){
		foreach(Node successor in expandedNode.Successors)
		{
			if(listContainsNode(closedList, successor))
				continue;
			
			double totalCost = expandedNode.Cost + euclideanDist(expandedNode, successor);
			
			if(listContainsNode(openList, successor) && totalCost >= successor.Cost)
				continue;
			
			successor.Cost = totalCost;
			successor.DistanceToGoal = euclideanDist(successor, endNode);
			successor.parentID = expandedNode.id;
			//if(listContainsNode(openList, successor))
				//update cost?
			openList.Add(successor);
		}
	}
	
	private bool listContainsNode(List<Node> list, Node node)
	{
		foreach(Node listNode in list)
		{
			if(listNode.id == node.id)
				return true;
		}
		return false;
	}
	
	private float haversineDist(Waypoint point1, Waypoint point2)
	{
		float dLat = Mathf.Deg2Rad * (point2.inclination - point1.inclination);
		float dLon = Mathf.Deg2Rad * (point2.azimuth - point1.azimuth);
		
		float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) + 
					Mathf.Cos(Mathf.Deg2Rad * point1.inclination) * Mathf.Cos(Mathf.Deg2Rad * point2.inclination) * 
					Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
		float c = 2 * Mathf.Asin(Mathf.Min(1, Mathf.Sqrt(a)));
		float d = point1.radius * c;
		
		return d;
	}
	
	private float euclideanDist(Node point1, Node point2)
	{
		return Vector3.Distance(point1.position, point2.position);
	}
	
	private List<Node> getPath(Node lastNode, List<Node> closedList) {
		List<Node> path = new List<Node>();
		path.Add(lastNode);
		while(lastNode.parentID != -1){
			lastNode = closedList.Find(delegate(Node nd)
            	{
                	return nd.id == lastNode.parentID;
            	});
			path.Add(lastNode);
		}
		path.Reverse();
		return path;
	}
}
