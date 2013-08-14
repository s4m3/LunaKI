using UnityEngine;
using System.Collections;

public class SizeOverTime : MonoBehaviour {
    public AnimationCurve curve;
    public float TargetSize;
    public float TargetTime;
    public bool KillOnComplete;

    float startTime;
	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

        float TimePercent = (Time.time - startTime) / TargetTime;
        Vector3 newScale = Vector3.one * curve.Evaluate(TimePercent) * TargetSize;
        transform.localScale = newScale;
        if (KillOnComplete && Time.time > TargetTime + startTime) Destroy(gameObject);
	}

    public void SetNormal(Vector3 normal)
    {

    }
}
