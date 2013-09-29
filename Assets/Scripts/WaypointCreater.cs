using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointCreater : MonoBehaviour {
	
	public GameObject planet;
	public GameObject testObject;
	public GameObject testObjectHit;
	public List<Waypoint> Waypoints;


	void Start () {
	}
	
	
	public void CreateWayPoints()
	{
		Waypoints = new List<Waypoint>();
		List<Vector3> liste = CalculateWaypoints();
		int id = 0;
		foreach(Vector3 point in liste)
		{
			float radius = Mathf.Sqrt(point.x * point.x + point.y * point.y + point.z * point.z);
			
			float inclination = Mathf.Acos(point.z / radius);
			float azimuth = Mathf.Atan(point.y / point.x);
			Waypoint wp = ScriptableObject.CreateInstance<Waypoint>();
			wp.id = id;
			wp.radius = radius;
			wp.inclination = inclination;
			wp.azimuth = azimuth;
			wp.originalPoint = point;
			Waypoints.Add(wp);
			id++;
		}
		
	}
	
	private List<Vector3> CalculateWaypoints()
	{
		if(!planet) return null;
		if(!testObject) return null;
		
		List<Vector3> waypoints = new List<Vector3>();
		MeshFilter meshFilter = (MeshFilter) planet.GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;
		Vector3[] normals = mesh.normals;
		Vector3[] vertices = mesh.vertices;
		Vector3 currentpos = Vector3.zero;
		int hitCount = 0;
		RaycastHit hit;
		for(int i=0; i<normals.Length; i++)
		{
			currentpos = vertices[i] + normals[i];
			if(!Physics.SphereCast(vertices[i], 1f, currentpos, out hit, 200.0f)){
				waypoints.Add(currentpos);
			} else {
				hitCount++;
			}
		}
		return waypoints;
	}
	
	private float euclideanDist(Waypoint point1, Waypoint point2)
	{
		return Vector3.Distance(point1.originalPoint, point2.originalPoint);
	}
}
