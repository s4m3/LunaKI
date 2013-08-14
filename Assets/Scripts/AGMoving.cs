using UnityEngine;
using System.Collections;

public class AGMoving : AGActor
{
	public float VelocityLookDirectionSpeed;
	public float SurfaceNormalSmoothSpeed = 100f;
	public float VelocityMaximum;


	[HideInInspector]
	public float currentVelocityMaximum;
	[HideInInspector]
	public Vector3 Velocity;
	[HideInInspector]
	public Vector3 TargetVelocity;
	[HideInInspector]
	public Vector3 LookDirection;
	[HideInInspector]
	public Vector3 SmoothSurfaceNormal;


    public virtual Vector3 GetLookDirection()
    {
        return LookDirection;
    }
	protected override void Start ()
	{
		base.Start ();
		LookDirection = transform.forward;
		currentVelocityMaximum = VelocityMaximum;
        nFramesForStateChangeCheck = 1;
	}

	protected override void FixedUpdate ()
	{
        UpdateHeight();
        base.FixedUpdate ();

		Move ();
	}

	public virtual void Move ()
	{
      
		//Update Vectors to Match Surfacenormals	
		SmoothSurfaceNormal = Vector3.Lerp (SmoothSurfaceNormal, LocalGroundNormal, Time.deltaTime * SurfaceNormalSmoothSpeed);

		TargetVelocity = Tools.ProjectVectorOntoPlane (TargetVelocity, SmoothSurfaceNormal);
		Velocity = Tools.ProjectVectorOntoPlane (Velocity, SmoothSurfaceNormal);
		LookDirection = Tools.ProjectVectorOntoPlane (LookDirection, SmoothSurfaceNormal).normalized;

		Debug.DrawRay (transform.position, Velocity, Color.blue);
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.velocity = Velocity;
		
		UpdateLookDirection ();
		UpdateRotation ();
		
	
		//Rotationchange to match Lookdirection    
	}

	protected virtual void UpdateRotation ()
	{

		transform.rotation = Quaternion.LookRotation (LookDirection, SmoothSurfaceNormal);

	}

	public override void UpdateHeight ()
	{
		base.UpdateHeight ();       
	}

	public virtual void UpdateMoveDirection (Vector3 newDir)
	{

	}

	public virtual void SetVelocity (Vector3 newVelocity)
	{
		TargetVelocity = newVelocity;
		Velocity = newVelocity;
	}
  
	protected virtual bool IgnoreSpeedMaximum ()
	{
		return false;
	}

	public virtual void UpdateLookDirection ()
	{
	
		LookDirection = Tools.ProjectVectorOntoPlane (LookDirection, SmoothSurfaceNormal);		
		
		if (Velocity != Vector3.zero) {
			LookDirection = Vector3.Slerp (LookDirection.normalized, Velocity.normalized, Time.fixedDeltaTime * VelocityLookDirectionSpeed);			
			LookDirection.Normalize ();					
			
		} 
		if (LookDirection == Vector3.zero)
			LookDirection = transform.right;
		Debug.DrawRay (transform.position, LookDirection * 5.6f, Color.yellow);	
	}

    
}
