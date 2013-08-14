using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AGActor : MonoBehaviour
{
    public delegate void NotifyActorStateChange(AGActor.LightState state);
    public event NotifyActorStateChange onStateChange;

	public List<AGEffect> AppliedEffects;
   
	public MeshCollider BasePlanet;
	public float DistanceToSurface;
	public Transform Mesh;

	AGActor owner;
	public bool CanTakeDamage;
    public AGStat Health;
	public Projector MyProjector;
	public bool UseLightZoneShader;
	public LayerMask SurfaceNormalRayCast;
	protected float SpawnTime;
	private float angleToSun;
	private float timeSinceState;

	public float TimeInState {
		get { return Time.time - timeSinceState; }
		set { timeSinceState = value; }
	}

	protected static int nFramesForStateChangeCheck = 3;
	public float ThresholdForStateChangeInDegrees = 30.0f;
	public bool StateChangeEnabled = true;
	public LightState currentLightState;
	private float thresholdForStateChange;
	public AGVFX DeathEffects;
	[HideInInspector]
	public Vector3 Normal;
	[HideInInspector]
	public Vector3 LocalGroundNormal;
	[HideInInspector]
	public Vector3 BasePos;
	[HideInInspector]
    public enum LightState
	{
		Real,
		Dream,
		Between
	};

	private AGMorph Morpher;
	public float morphValue;

	protected virtual void Start ()
	{
		InitActor (null);        
		thresholdForStateChange = ThresholdForStateChangeInDegrees / 360.0f;
		Morpher = gameObject.GetComponentInChildren<AGMorph> ();
		CheckState();
	}
	
	public virtual void InitActor (AGActor _owner)
	{
		if (_owner != null) {
			//print ("new owner "+_owner);
			owner = _owner;
			BasePlanet = owner.BasePlanet;   
		}

		if (UseLightZoneShader)		
             renderer.material = new Material (renderer.material);
		if(BasePlanet){
		transform.parent = BasePlanet.transform;
		} else Debug.Log(gameObject.name+"   has no Basesphere");
		//Distance = BaseSphere.radius;
		SpawnTime = Time.time;
        Health.InitStat();
		UpdateHeight ();
        UpdateMorphing(true);
	}
	
	public void InitActor ()
	{
		if (UseLightZoneShader)		
             renderer.material = new Material (renderer.material);
		if(BasePlanet){
		transform.parent = BasePlanet.transform;
		} else Debug.Log(gameObject.name+"   has no Basesphere");
		//Distance = BaseSphere.radius;
		SpawnTime = Time.time;
        Health.InitStat();
		UpdateHeight ();  
	}
	// Update is called once per frame
	protected virtual void FixedUpdate ()
	{	
		//if(this.currentLightState == LightState.Dream) ModifyHealth(-1);
		//UpdateHeight ();
        CheckState();
		UpdateMorphing (false);
	}
	protected void CheckState(){

         if (StateChangeEnabled && Time.frameCount % nFramesForStateChangeCheck == 0) {
			CheckLight ();
			UpdateState ();
		}

    }
	protected Vector3 GetBasePos ()
	{
		// return BaseSphere.transform.position + Normal * (Distance + DistanceToSurface); 
		return Vector3.zero;
	}

	public virtual void SetHeightPos ()
	{
		RaycastHit hit;
		if (Physics.Raycast (transform.position, -Normal, out hit, Mathf.Infinity, SurfaceNormalRayCast)) {
			LocalGroundNormal = hit.normal;
			Debug.DrawRay (hit.point, hit.normal);
			transform.position = hit.point + Normal * DistanceToSurface;
		}
       
	}

	public virtual void UpdateHeight ()
	{

		if (BasePlanet != null) {
			Normal = (transform.position - BasePlanet.transform.position).normalized;       
			SetHeightPos ();
			Debug.DrawRay (transform.position, Normal * 0.3f, Color.white);
		}
		if (LocalGroundNormal != Vector3.zero) {
			Quaternion newRotation = Quaternion.FromToRotation (transform.up, LocalGroundNormal);
            
			transform.rotation = newRotation * transform.rotation;
			//  transform.rotation = Quaternion.LookRotation(LocalGroundNormal, Vector3.zero);
		}

	}
	
	public string m_AudioWeapon = "melee";
	
	public virtual string getAudioTarget ()
	{
		return "env";
	}
	
	public virtual void TakeDamage (float amount, AGActor damageInstigator)
	{
		if (!CanTakeDamage || AGGame.Instance.gameState != AGGame.GameState.Running)
			return;    
		// TODO play
		AGGame.Instance.getSoundServer().Play(m_AudioWeapon, getAudioTarget());
		ModifyHealth (-amount);	
	}
	
	public void ModifyHealth (float amount)
	{
        Health.currentValue += amount;
        if (Health.currentValue < 0)
        {
			Die ();
		}
		Health.currentValue = Mathf.Clamp (Health.currentValue, 0, Health.max);
	}

	protected virtual void SpawnDeathEffects ()
	{
		Instantiate (DeathEffects, transform.position, transform.rotation);
	}

	public virtual void Die ()
	{
		if (DeathEffects != null) {
			SpawnDeathEffects ();            
		}
		Destroy (gameObject);
	}

    protected void CheckLight()
    {
        //scalar product between Normal and direction to sun
        //angle between the two: at 1 the sun is facing the actor directly
        //at 0 it is perpendicular
        //at -1 it is facing the opposite side of the sphere
        angleToSun = Vector3.Dot(this.Normal.normalized, (AGGame.Instance.Sun.transform.position - transform.position).normalized);
        //if (gameObject.name == "PAWN (ONE)") Debug.Log(angleToSun);
    }

	protected void UpdateState ()
	{
		LightState oldState = this.currentLightState;
		
        LightState newState = oldState;

		if (this.angleToSun >= 0.0f + thresholdForStateChange) {
	
			newState = LightState.Real;
		} else if (this.angleToSun <= 0.0f - thresholdForStateChange) {
			
			newState = LightState.Dream;
		} else if (this.angleToSun > -thresholdForStateChange && this.angleToSun < thresholdForStateChange) {
		
			newState = LightState.Between;
		}
	
        if( newState != oldState){
            PerformStateChanges(newState);
        }
	
	}
	
	private void UpdateMorphing (bool force)
	{
		if (!Morpher)
			return;
		if (force || currentLightState == LightState.Between) {
			morphValue = 1 - ((this.angleToSun / thresholdForStateChange) + 1) / 2;
			Morpher.SetMorph (morphValue);
			if (UseLightZoneShader) {
				renderer.material.SetFloat ("_AngleToSun", (this.angleToSun / thresholdForStateChange));
                renderer.material.SetColor("_SelfEmissionColor", Color.Lerp(AGGame.Instance.RealColor, AGGame.Instance.DreamColor, this.angleToSun / thresholdForStateChange));
			}
		} 	
	}
	
	private void resetTimeInState ()
	{
		TimeInState = Time.time;
	}
	

	//virtual function that can be overwritten
	//is called when LightStates change
	public virtual void PerformStateChanges (LightState newState)
	{
  
		resetTimeInState ();
        currentLightState = newState;
		//this is the final value to complete morph (would take up too much performance in UpdateMorphing())
		if(Morpher) {
			if(currentLightState == LightState.Dream) {
				Morpher.SetMorph(1);
			} else if(currentLightState == LightState.Real) {
				Morpher.SetMorph(0);
			}
		}

      if(onStateChange != null) onStateChange(currentLightState);
	}

	//ENTRYPOINT FOR EFFECTS
	public virtual bool ApplyEffect (AGEffect effect)
	{
		//if checkforsimiliareffect //Do Stuff
      
		AppliedEffects.Add (effect);
		effect.EffectApplied (this);
		return true;
	}

	public virtual void NotifyEffectEnded (AGEffect effect)
	{
        AppliedEffects.Remove(effect);
	}
}
/*
[System.NonSerialized]
public class OnDeathBehavior
{
    public bool Destroy;
    public bool DisableGameObject;
    public bool HideMesh;
    public bool DisableCollider;
}
*/