using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FindPathJob : ThreadedJob
{	
	public Node startNode;
	public Node endNode;
	
	private List<Node> path;
 
    protected override void ThreadFunction()
    {
        path = FindPath();
    }
    protected override void OnFinished()
    {
        // This is executed by the Unity main thread when the job is finished
        Pathfinder.Instance.path = path;
		Pathfinder.Instance.pathID++;
		Pathfinder.Instance.isFindingPath = false;
    }
	
	public List<Node> FindPath(){
		List<Node> openList = new List<Node>();
		List<Node> closedList = new List<Node>();
		Node currentNode;
		startNode.Cost = 0;
		startNode.parentID = -1;
		currentNode = startNode;
		openList.Add(currentNode);
		double lowestCost;
		int loop=0;
		while(currentNode.id != endNode.id && loop < 100){
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
		Debug.Log("loops needed: " +loop);
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
	
	private float euclideanDist(Node point1, Node point2)
	{
		return Vector3.Distance(point1.position, point2.position);
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
	
	private List<Node> getPath(Node lastNode, List<Node> closedList) {
		List<Node> path = new List<Node>();
		path.Add(lastNode);
		//print ("parentid:"+lastNode.parentID);
		while(lastNode.parentID != -1){
			lastNode = closedList.Find(delegate(Node nd)
            	{
                	return nd.id == lastNode.parentID;
            	});
			//print ("lastnode id:" + lastNode.id);
			path.Add(lastNode);
		}
		path.Reverse();
		return path;
	}
}
