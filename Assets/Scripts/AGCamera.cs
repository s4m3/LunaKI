using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AGCamera : MonoBehaviour {
	
	AGMoving MovingViewTarget;
	public AGActor ViewTarget;

    public Transform CameraRig;

	public Vector3 BaseOffsetRotation;
	public Vector3 BaseOffsetPosition;
    Vector3 FocusPos;
	
	//for start rotation at character choice menu
	public float rotAngle;
	private Vector3 rotationAxis = Vector3.zero;
	private Transform centerOfRotation;
	
	public float SpeedAdjustRange;
	public float SpeedAdjustThreshold;
	public float Distance;
	public float SpeedPredictionLerpSpeed;
    public float WinDollySpeed;
    public float WinDollyDistance;

	Vector3 SpeedAdjustTarget;
	Vector3 CurrentSpeedAdjust;

    Vector3 ShakeRotationOffset;
    Vector3 ShakePositionOffset;
    Vector3 currentShakePos;
    Vector3 currentShakeRot;

    private AGPawn WinTarget;

    public float MaxCumulatedShakeStrength = 4;
    public float AngleShakeMod = 2;
	// Use this for initialization
	
    [System.Serializable]
    public class CamShake
    {
        public float strength;
        public float seismic;
        public float Duration;
       
        public float BlendIn;
        public float BlendOut;
        [HideInInspector]
        public float NextUpdateTime;
         [HideInInspector]
        public float StartTime;
         [HideInInspector]
        public Vector3 posShake;
         [HideInInspector]
        public Vector3 rotShake;
         [HideInInspector]
        public float RemainingTime;
    }
    List<CamShake> CamShakes;

    public enum CameraState { MovingTarget, Static, WinDolly, StartRotation };
    public CameraState CurrentCameraState;

    void Start()
    {
        CamShakes = new List<CamShake>();
        SetViewTarget(ViewTarget);

    }

	// Update is called once per frame
	void FixedUpdate () {

        //UPDATE CamShake
        if (CamShakes.Count > 0 || currentShakeRot != Vector3.zero || currentShakePos != Vector3.zero)
        {
            ShakeCamera();
        }

        //Execute Cameramovement etc
        switch (CurrentCameraState)
        {
			case CameraState.StartRotation:
				RotateAroundPlanet();
				break;
			case CameraState.Static:
            case CameraState.MovingTarget:
                FollowMovingTarget();
                break;
            case CameraState.WinDolly:
                WinDolly();
                break;
        }
	}

    public void StartShake(float strength, float seismic, float duration, float BlendIn, float BlendOut)
    {
        //		Debug.Log("Shake it Baby");	
        CamShake newShake = new CamShake();
        newShake.strength = strength;
        newShake.Duration = duration;
        newShake.BlendIn = BlendIn;
        newShake.BlendOut = BlendOut;
        newShake.NextUpdateTime = Time.time;
        newShake.StartTime = Time.time;
        newShake.seismic = 1 / seismic;
        newShake = UpdateShake(newShake);
        CamShakes.Add(newShake);
        //		Debug.Log("Camshake added: "+CamShakes.Count);
        ShakeCamera();
    }

    public void StartShake(CamShake newShake)
    {
        StartShake(newShake.strength, newShake.seismic, newShake.Duration, newShake.BlendIn, newShake.BlendOut);
    }

    CamShake UpdateShake(CamShake shake)
    {
        //Calculate next Shake Time
        shake.NextUpdateTime = Time.time + Random.value * shake.seismic;

        //Update Shake Values (Modifies Strength based on time)
        shake.RemainingTime = shake.Duration - (Time.time - shake.StartTime);

        //APPLY Blends (Calculating wether to blend in or blend out, required quite as much calculation as if just doing both - since the shake should
        //only blend in OR out, there should be no problem with this.
        //blend IN
        float currentStrength = Mathf.Lerp(0, shake.strength, (shake.Duration - shake.RemainingTime) / shake.BlendIn);

        //blend OUT
        currentStrength = Mathf.Lerp(0, shake.strength, shake.RemainingTime / shake.BlendOut);

        //Calculate Vector based on strength to apply it to the current ShakeOffsets		
        shake.rotShake = Random.insideUnitSphere * currentStrength * AngleShakeMod;
        shake.posShake = Random.insideUnitSphere * currentStrength;
        
        return shake;
    }
    void ShakeCamera()
    {
        //UPdate each Camshakeobject and apply its Values
        Vector3 newShakePos = Vector3.zero;
        Vector3 newShakeRot = Vector3.zero;

        for (int i = 0; i < CamShakes.Count; i++)
        {
            if (CamShakes[i].NextUpdateTime < Time.time)
            {
                CamShakes[i] = UpdateShake(CamShakes[i]);
                if (CamShakes[i].RemainingTime <= 0)
                {
                    CamShakes.RemoveAt(i);
                    break;
                }
            }
            newShakePos += CamShakes[i].posShake;
           
          //  Debug.Log("newShakePos:" + newShakePos);
            newShakeRot += CamShakes[i].rotShake;
        }
        newShakePos = Vector3.ClampMagnitude(newShakePos, MaxCumulatedShakeStrength);

        ShakePositionOffset = newShakePos;
        ShakeRotationOffset = newShakeRot;

        currentShakePos = Vector3.Lerp(currentShakePos, ShakePositionOffset, Time.deltaTime * 5);
        currentShakeRot = Vector3.Lerp(currentShakeRot, ShakeRotationOffset, Time.deltaTime * 5);
    }

	void FollowMovingTarget(){
        if (MovingViewTarget == null)
          {
               CurrentCameraState = CameraState.Static;
               return;
           }
      
        UpdateMovingFocus();
		Quaternion rotation = Quaternion.LookRotation(-MovingViewTarget.Normal, transform.up);	
      
        Vector3 newPosition = FocusPos + MovingViewTarget.Normal * Distance;	
		transform.position = newPosition;
        transform.rotation = rotation;
       
       CameraRig.rotation = rotation * Quaternion.Euler(BaseOffsetRotation) * Quaternion.Euler(currentShakeRot);
       CameraRig.LookAt(MovingViewTarget.transform.position + CurrentSpeedAdjust, transform.up);

        CameraRig.position = newPosition + rotation * BaseOffsetPosition + rotation * currentShakePos;
        
	
	}
    private void WinDolly()
    {
        if (WinTarget == null) return;
        // Lerp Positions
        CameraRig.transform.position = Vector3.Lerp(CameraRig.transform.position, WinTarget.transform.position + WinTarget.GetLookDirection() * WinDollyDistance, Time.deltaTime * WinDollySpeed);
        
        //Smooth UP-Axis Transition between Planetnormal & Camera Normal
        Vector3 up = Vector3.Lerp(CameraRig.transform.up, WinTarget.Normal, Time.deltaTime * 5);
        Quaternion camRotation = Quaternion.LookRotation(WinTarget.transform.position-CameraRig.transform.position, up);
        CameraRig.transform.rotation = camRotation;
    }   
    public void SetViewTarget(AGActor ag, CameraState state)
    {
        ViewTarget = ag;
        CurrentCameraState = state;
    }

    public void StartViewDolly(AGPawn p)
    {
        CurrentCameraState = CameraState.WinDolly;
        WinTarget = p;
    }

    public void SetViewTarget(AGActor ag)
    {
        if (ag is AGMoving)
        {
            MovingViewTarget = ag.GetComponent<AGMoving>();
            SetViewTarget(MovingViewTarget, CameraState.MovingTarget);
        } else
        {
            SetViewTarget(ag, CameraState.Static);
        }
    }
	
	public void SetPlanetBackground(Transform target, Vector3 position)
	{
		CameraRig.position = position * 3;
		centerOfRotation = target;
		Vector3 normal = CameraRig.transform.position - centerOfRotation.position;
		Vector3.OrthoNormalize(ref normal, ref rotationAxis);
		CameraRig.LookAt(target.position, transform.up);
		CurrentCameraState = CameraState.StartRotation;
		if(rotAngle == 0) rotAngle = 5;
	}
	
	private void RotateAroundPlanet()
	{
		CameraRig.RotateAround(centerOfRotation.position, rotationAxis, rotAngle * Time.deltaTime);
        CameraRig.LookAt(centerOfRotation);
	}

    void UpdateMovingFocus()
    {
		float range = ( MovingViewTarget.VelocityMaximum * SpeedAdjustThreshold );	
		
		float speedOverMargin = Mathf.Clamp(MovingViewTarget.Velocity.magnitude - range, 0f,MovingViewTarget.VelocityMaximum);	
		
		float speedPercent = Mathf.Clamp(speedOverMargin / (MovingViewTarget.VelocityMaximum - range),0f,1f);

        //SpeedAdjustTarget = MovingViewTarget.Velocity.normalized * speedPercent * SpeedAdjustRange;
        SpeedAdjustTarget = MovingViewTarget.GetLookDirection().normalized * speedPercent * SpeedAdjustRange;

		CurrentSpeedAdjust = Vector3.Lerp(CurrentSpeedAdjust, SpeedAdjustTarget, Time.fixedDeltaTime*SpeedPredictionLerpSpeed);
				
		//Debug.DrawRay(ViewTarget.transform.position, CurrentSpeedAdjust, Color.cyan);
		
        FocusPos = ViewTarget.transform.position - CurrentSpeedAdjust * 1.5f;
		
      //FocusPos += Vector3.Cross(ViewTarget.Normal, Vector3.up) * BaseOffsetPosition;
		Debug.DrawRay(transform.position, Vector3.Cross(ViewTarget.Normal, Vector3.up));
    }
}
