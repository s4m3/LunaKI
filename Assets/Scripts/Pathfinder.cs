using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinder : MonoBehaviour {
	
	public GameObject waypointCreaterPrefab;
	public GameObject Planet;
	private int maxAmountOfSuccessors = 6;
	
	private static Pathfinder pathfinderInstance;
	
	public GameObject TestSphere;
	
	private WaypointCreater wpCreator;
	public List<Waypoint> waypoints;
	public List<Node> Nodes;
	
	public Vector3[] nodePositions;
	public KDTree kdTree;
	
	public List<Node> path;
	public int pathID;
	
	FindPathJob findPathJob;
	public bool isFindingPath = false;
	
	public List<int> deletedNodesIDs;
	public GameObject text;
	public GameObject nodeSphere;
	
	public bool turnOnGUI = false;
	
	public static Pathfinder Instance
    {
        get
        {
            return pathfinderInstance != null ? pathfinderInstance : GetInstance();
        }
    }
	
	private static Pathfinder GetInstance()
    {
	
		pathfinderInstance = (Pathfinder) GameObject.FindObjectOfType(typeof(Pathfinder));
       	return pathfinderInstance;
    }
	
	public void Init () {
		pathID = 0;
		//are there nodes already stored?
		LoadNodesFromFileAndConvert();
		if(Nodes.Count == 0)
		{
			Debug.Log ("No nodes found: Creating new nodes!");
			GameObject obj = (GameObject) GameObject.Instantiate(waypointCreaterPrefab);
			wpCreator = obj.GetComponent<WaypointCreater>();
			wpCreator.planet = Planet;
			wpCreator.CreateWayPoints();
			this.waypoints = wpCreator.Waypoints;
			CreateNodesFromWaypoints();
		}
		
		nodePositions = GetPositionsForKDTree();
		kdTree = KDTree.MakeFromPoints(nodePositions);
		
	}
	
	Vector3[] GetPositionsForKDTree() {
		Vector3[] positions = new Vector3[Nodes.Count];
		for(int i=0; i<Nodes.Count; i++) {
			positions[i] = Nodes[i].position;
		}
		return positions;
	}
	
	void Update () {
		if (findPathJob != null)
	    {
	        if (findPathJob.Update())
	        {
				//print path nodes as text
				if(AGGame.Instance.DebugMode)
				{
					for(int i=0; i<path.Count; i++)
					{
						Node n = path[i];
						GameObject obj = (GameObject) GameObject.Instantiate(text);
						TextMesh tm = obj.GetComponent<TextMesh>();
						tm.text = i.ToString();
						Instantiate(obj, n.position, Quaternion.identity);
						
					}
				}
	            findPathJob = null;
	        }
	    }
	}
	
	public Node FindNodeToPositionWithKDTree(Vector3 target) 
	{
		int num = kdTree.FindNearest(target);
		return Nodes[num];
	}
	
	public Node FindNodeToPosition(Vector3 position)
	{
		float lowest = float.MaxValue;
		float current = float.MaxValue;
		int id = -1;
		foreach(Node n in Nodes) {
			current = Vector3.Distance(n.position, position);
			if( current < lowest) {
				lowest = current;
				id = n.id;
				if(lowest < 0.8f) return n;
			}
		}
		
		Node nodeToFind = Nodes.Find(
					delegate(Node nd)
	            	{
	                	return nd.id == id;
	            	});
		return nodeToFind;
	}
	
	public void CalculateNewPath(Vector3 start, Vector3 target)
	{
		Node a = FindNodeToPositionWithKDTree(start);
		Node b = FindNodeToPositionWithKDTree(target);

		StartPathfind(a, b);
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
			node.nodeSpherePrefab = nodeSphere;
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
				if(succString != "") {
					nodeToFind = Nodes.Find(
						delegate(Node nd)
		            	{
		                	return nd.id == System.Convert.ToInt32(succString);
		            	});
					n.Successors.Add(nodeToFind);
				}
			}
			//n.ShowNode();
		}
		//DrawSuccessors(Nodes);
	}
	///////////DEBUG///////////
	private void DrawLines(List<Node> nodes)
	{
		for(int l=0; l<nodes.Count-1; l++)
		{
			Debug.DrawLine(nodes[l].position, nodes[l+1].position, Color.red, 10000000, true);
		}
	}
	
	private void DrawSuccessors(List<Node> nodes)
	{
		foreach(Node node in nodes)
		{
			foreach(Node successor in node.Successors)
			{
				Debug.DrawLine(node.position, successor.position, Color.yellow, 10000000, true);
			}
		}
	}
	//+++++++++++++++++++++++++++
	
	private float euclideanDist(Node point1, Node point2)
	{
		return Vector3.Distance(point1.position, point2.position);
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
		CalculateSuccessors();
	}
	
	private void CalculateSuccessors() {
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
	
	public void StartPathfind(Node startNode, Node endNode)
	{
		if(isFindingPath) return;
		isFindingPath = true;
		findPathJob = new FindPathJob();
		findPathJob.startNode = startNode;
		findPathJob.endNode = endNode;
 
    	findPathJob.Start();
	}

	//NOT IN USE//
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
	

	
	///////////GUI//////////////////	FOR DEBUG PURPOSES ONLY
	void UpdateAllNodes()
	{
		deletedNodesIDs = new List<int>();
		foreach(Node nd in Nodes) 
		{
			if(nd.ns) {
				NodeSphereVars nsv = nd.ns.GetComponent<NodeSphereVars>();
				if(nsv.delete) 
				{
					print ("node" + nd.id + "will be deleted");
					deletedNodesIDs.Add(nd.id);
				}
			}

			
		}
		foreach(int delID in deletedNodesIDs) {
			
			Node nodeToDelete = Nodes.Find(delegate(Node nodeToFind)
            	{
                	return nodeToFind.id == delID;
            	});
			Destroy(nodeToDelete.ns);
			Nodes.RemoveAll((Node i) => i.id == delID);
			Destroy(nodeToDelete);
			
		}
		foreach(Node n in Nodes)
		{
			n.UpdatePosition();
			n.UpdateSuccessors(deletedNodesIDs, Nodes);
		}
	}
	
	void SaveAllNodes()
	{
		Persist.serializeNodesToFile(Nodes, "newNodesDeleted2.dat");
	}
	
	void OnGUI()
	{
		if(!turnOnGUI) return;
		
		if(GUI.Button (new Rect(0, 0, 120, 40), "update all nodes"))
		{
			UpdateAllNodes();
		}
		
		if(GUI.Button (new Rect(0, 150, 120, 40), "recalc successors"))
		{
			CalculateSuccessors();
		}
		
		if(GUI.Button (new Rect(0, 300, 120, 50), "save new nodes"))
		{
			SaveAllNodes();
		}
	}
}
