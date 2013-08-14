using UnityEngine;
using System.Collections;

public class AGSun : MonoBehaviour {
	
	public Transform centerOfRotation;
	private Light sunlight;
	public float rotAngle;

	private Vector3 rotationAxis = Vector3.zero;

	void Start () {
		sunlight = this.GetComponent<Light>();
		
		Vector3 normal = transform.position - centerOfRotation.position;
		Vector3.OrthoNormalize(ref normal, ref rotationAxis);
        transform.LookAt(centerOfRotation);
		//rotationAxis = new Vector3(rotationAxis.y, rotationAxis.x, rotationAxis.z);
	}
	
	public void Spin () {
		this.transform.RotateAround(centerOfRotation.position, rotationAxis, rotAngle * Time.deltaTime);
        transform.LookAt(centerOfRotation);
	}
	
	public void setLightIntensity(float intensity)
	{
		this.sunlight.intensity = intensity;
	}
	
}
