using UnityEngine;
using System.Collections;

public class RayCastTest1 : MonoBehaviour {
	public LayerMask mask;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	 RaycastHit hit;
		Debug.DrawRay(transform.position, transform.up, Color.green);
		if(Physics.Raycast(transform.position, transform.up, out hit, Mathf.Infinity, mask)){
			print("hitsomething "+hit.collider.gameObject.name);
			Debug.DrawLine(transform.position, hit.point, Color.red);			
		}
	}
}
