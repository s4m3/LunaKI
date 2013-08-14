using UnityEngine;
using System.Collections;

public class LookToCamera : MonoBehaviour {

    public Transform Target;
    public Vector3 Offset;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Target != null)
        {
            gameObject.transform.LookAt(Target, -Target.up);
            if (Offset != Vector3.zero) gameObject.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(Offset);
        }
	}
}
