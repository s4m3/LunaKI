using UnityEngine;
using System.Collections;

public class AniTest : MonoBehaviour {
	public AnimationClip Idle;
	public AnimationClip Walk;
	public AnimationClip WalkLeft;
	public AnimationClip WalkRight;
	
	public AnimationClip Dash;
	
	public float Percentage = 1.0f;
	public float WalkPercentage = 0.0f; // -1 Left, 0 Center, +1 Right
	public float Speed = 1.0f;
	
	public float DashWeight = 0.0f;
	
	private AnimationState idle;
	private AnimationState walk;
	private AnimationState walkLeft;
	private AnimationState walkRight;
	
	private AnimationState dash;
	// Use this for initialization
	void Start ()
	{
		idle = animation [Idle.name];
		walk = animation [Walk.name];
		walkLeft = animation [WalkLeft.name];
		walkRight = animation [WalkRight.name];
		dash = animation [Dash.name];
		
		idle.wrapMode = WrapMode.Loop;
		walk.wrapMode = WrapMode.Loop;
		walkLeft.wrapMode = WrapMode.Loop;
		walkRight.wrapMode = WrapMode.Loop;
		
		dash.wrapMode = WrapMode.Loop;
		
		var perc = calcPerc (Percentage, WalkPercentage);
		idle.weight = 1;// - Percentage;
		walkLeft.weight = perc [0];
		walk.weight = perc [1];
		walkRight.weight = perc [2];
		
		dash.weight = DashWeight;
		
		idle.enabled = true;
		walk.enabled = true;
		walkLeft.enabled = true;
		walkRight.enabled = true;
		
		dash.enabled = true;
		
		idle.normalizedSpeed = Speed;
		walk.normalizedSpeed = Speed;
		walkLeft.normalizedSpeed = Speed;
		walkRight.normalizedSpeed = Speed;
		
		idle.layer = 1;
		walk.layer = 2;
		walkLeft.layer = 2;
		walkRight.layer = 2;
		dash.layer = 3;
	}
	
	float[] calcPerc (float percentage, float walkPercentage)
	{
		float[] retVal = new float[3];
		
		retVal [2] = Mathf.Max (0, walkPercentage)*percentage;
		retVal [0] = (-1.0f)*Mathf.Min (0, walkPercentage) * percentage;
		retVal [1] = (1.0f - Mathf.Abs (walkPercentage))*percentage;
		
		return retVal;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//idle.weight = 1 - Percentage;
//		walk.weight = Percentage;
		
		var perc = calcPerc (Percentage, WalkPercentage);
//		Print.Log (perc [0] + " " + perc [1] + " " + perc [2]);
//		idle.weight = 1;// - Percentage;
		walkLeft.weight = perc [0];
		walk.weight = perc [1];
		walkRight.weight = perc [2];
		
		dash.weight = DashWeight;
		
		
		idle.normalizedSpeed = Speed;
		walk.normalizedSpeed = Speed;
		walkLeft.normalizedSpeed = Speed;
		walkRight.normalizedSpeed = Speed;
		
//		Print.Log (idle.time + " " + walk.time);
	}
}
