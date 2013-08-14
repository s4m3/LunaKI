using UnityEngine;
using System.Collections;

public class RotateFoo : MonoBehaviour {
	public Quaternion quat;
	public Vector3 FromTo1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		
	//transform.rotation = Quaternion.FromToRotation (Vector3.up, transform.forward);
	
		//print(Quaternion.Dot(transform.rotation, Quaternion.identity));
		//Vector3 dir = ( transform.forward * 3);
		
	//	Debug.DrawRay(transform.position, dir, Color.red);		
		Debug.DrawRay(transform.position, FromTo1, Color.green);
		Debug.DrawRay(transform.position, Quaternion.FromToRotation(FromTo1, transform.forward ) * new Vector3(1,0,0), Color.blue);
	
		
		Quaternion q = Quaternion.AngleAxis(Time.deltaTime * 10f, transform.up);
		
		transform.rotation = q * transform.rotation;
	}
}
