using UnityEngine;
using System.Collections;

public class AGSphere : MonoBehaviour {
    Vector3 RotationAxis;
    public float RotationSpeed;

	// Use this for initialization
	void Start () {
        RotationAxis = transform.up;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.rotation = Quaternion.AngleAxis(RotationSpeed * Time.fixedDeltaTime, RotationAxis) * transform.rotation;
	}
}
