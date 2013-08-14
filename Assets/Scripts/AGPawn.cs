using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AGPawn : AGMoving
{
	public float VelocityChangeSpeed;
	public float VelocityDamp;
	public float InputDirVelocity;
	public float LookSpeed = 5;
	public Vector3 MuzzlePointOffset;
	public GameObject LightStateSwitchEffekt;
    #region STATS
	[HideInInspector]
	public AGStatSingle Attack;
	[HideInInspector]
	public AGStatSingle Defense;
	[HideInInspector]
	public AGStat Energy;
	[HideInInspector]
	public AGStatSingle Speed;
	[HideInInspector]
	public AGStatSingle EnergyGainRate;
	public float RegenerateHealthValue;
	public float RegenerateEnergyValue;
    #endregion   
	public GameObject AimHelp;
	[HideInInspector]
	public AGPlayerController Player;
	Vector3 InputLookDirection;
	Vector3 LastInputLookdirection;
	protected AGAnimationPawn m_AnimationPawn;
	[HideInInspector]
	public bool InputLook;
	[HideInInspector]
	public bool InputMove;
	[HideInInspector]
	public Vector3 InputMovementDir;
	protected List<AGEffect_Stun> Stuns;
	AGEffect_Push currentPush;
	[HideInInspector]
	public Vector3 TargetLookDirection;
	[HideInInspector]
	public AGStatSingle MeshSize;
	public LightState CurrentPlayerState = LightState.Real;
	
	private bool m_enableRenderer = false;
	private GameObject m_myObject;
	public bool m_roundIsOver = false;
	
	public override Vector3 GetLookDirection ()
	{
		if (TargetLookDirection == Vector3.zero)
			return base.GetLookDirection ();
		return TargetLookDirection;
	}

	public void registerAnimationPawn (AGAnimationPawn animationPawn)
	{
		m_AnimationPawn = animationPawn;
	}
	
	public void PlayAnimation (string anim)
	{
		m_AnimationPawn.doAnimation(anim);
	}

	protected override void Start ()
	{
		base.Start ();
		//TODO:Erase, Test Values
		if (RegenerateEnergyValue == 0)
			RegenerateEnergyValue = 1;
		if (RegenerateHealthValue == 0)
			RegenerateHealthValue = 1;

		InputMove = true;
		InputLook = true;
		Stuns = new List<AGEffect_Stun> ();
	}

	public override void InitActor (AGActor _owner)
	{
		//Override default init with "null" and pass this Pawn as Reference
        
		base.InitActor (_owner == null ? this : _owner);
		LookDirection = transform.forward;
		UpdateRotation ();
		m_roundIsOver = false;
		//Update Interface heads in the beginning
		Player.playerInterface.SetHeadTextureGroupID();
		Player.playerInterface.UpdateHeadTexture(currentLightState);
	}

	// Update is called once per frame
	protected override void FixedUpdate ()
	{
		base.FixedUpdate ();
       
	}
	
	void Update ()
	{
		if (AGGame.Instance.gameState == AGGame.GameState.Running) {
			
			UpdateHealthIncrease ();				
			UpdateEnergyIncrease ();
		}		
	}
	
	public override void Move ()
	{		
		if (TargetVelocity != Vector3.zero && Velocity != TargetVelocity) {
           
			Velocity = Vector3.Lerp (Velocity, TargetVelocity, VelocityChangeSpeed * Time.fixedDeltaTime);
          
		} else {
			if (Velocity.magnitude > 0.05f) {
				Velocity *= 1 - Time.fixedDeltaTime * VelocityDamp;
               
			} else
				Velocity = Vector3.zero;
		}

		if (currentPush == null) {
          
			Velocity = Vector3.ClampMagnitude (Velocity, Speed.currentValue);
			TargetVelocity = Vector3.ClampMagnitude (TargetVelocity, Speed.currentValue);
			//  if (Player.PlayerID == 1) Debug.Log(Speed.currentValue+" "+Speed.baseValue+"  currentSpeed:"+Velocity.magnitude);
		}
		base.Move ();
	}

	public override void UpdateMoveDirection (Vector3 newDir)
	{
      
		InputMovementDir = newDir;

		if (!InputMove)
			return;
		//Recives inputvector with 0 - 1 magnitude
		if (newDir != Vector3.zero) {
          
			newDir *= Speed.currentValue;          
			TargetVelocity = Tools.ProjectVectorOntoPlane (newDir, SmoothSurfaceNormal);
			//print(TargetVelocity.magnitude);
			Debug.DrawRay (transform.position, TargetVelocity, Color.green);
	
		} else {
			TargetVelocity = newDir;
		}
       
	}

	//Stops all Controller inputs for a certain time
	public void StopMovements ()
	{
     
		Velocity = Vector3.zero;
		TargetVelocity = Vector3.zero;
		if (currentPush) {
			currentPush.EffectEnd ();
		}
	}

	public override void SetVelocity (Vector3 newVelocity)
	{
		base.SetVelocity (newVelocity);
        
	}

	public void UpdateInputLook ()
	{
		foreach (AGEffect_Stun stun in Stuns) {
			if (stun.bActivated && stun.BlocksLook) {
				InputLook = false;
				return;
			}
		}
		InputLook = true;
	}

	public void UpdateInputMove ()
	{
		foreach (AGEffect_Stun stun in Stuns) {          
			if (stun.bActivated && stun.BlocksMovement) {
				InputMove = false;
				return;
			}
		}
		InputMove = true;
	}

	public bool AllowsAction ()
	{
		foreach (AGEffect eff in AppliedEffects) {

			if (eff.bActivated && eff.BlocksAction)
				return false;
		}
		return true;
	}

	public override bool  ApplyEffect (AGEffect effect)
	{
		if (base.ApplyEffect (effect)) {
			//effect.gameObject.transform.parent = this.transform;
			if (effect is AGEffect_Stun) {
				Stuns.Add ((AGEffect_Stun)effect);
			}
			return true;
		} else {
			return false;
		}
	}

	public override void  NotifyEffectEnded (AGEffect effect)
	{
       
		if (effect is AGEffect_Stun)
			Stuns.Remove ((AGEffect_Stun)effect);
       
		base.NotifyEffectEnded (effect);

        
	}

	//returns true if pawn is now stunned or does not have any effects with move-block applied to them
	public bool CheckIfControllerMovable ()
	{
		return true;
	}

	protected override void UpdateRotation ()
	{
		if (TargetLookDirection != Vector3.zero) {
			LookDirection = Vector3.Lerp (LookDirection, TargetLookDirection, LookSpeed * Time.deltaTime);         
		}

		Quaternion newRotation = Quaternion.FromToRotation (transform.up, SmoothSurfaceNormal);
	
		//RotationChange to match Surface-Normal
		newRotation = newRotation * Quaternion.FromToRotation (transform.right, Tools.ProjectVectorOntoPlane (-LookDirection, transform.up));
				
		Debug.DrawRay (transform.position, transform.up * 0.1f, Color.red);
		//Rotation Aligned to Normal
		
		transform.rotation = newRotation * transform.rotation;
		Debug.DrawLine (transform.position, transform.position + transform.rotation * MuzzlePointOffset);
	}

	public virtual void SetLookDirection (Vector3 vec)
	{
		if (!InputLook)
			return;
		InputLookDirection = Tools.ProjectVectorOntoPlane (vec, LocalGroundNormal).normalized;
			
	}
	
	public override void UpdateLookDirection ()
	{
		Debug.DrawLine (transform.position, GetMuzzlePos ());
		
		if (InputLookDirection == Vector3.zero) {
			base.UpdateLookDirection ();			
			TargetLookDirection = LookDirection;
		} else {
			InputLookDirection = Tools.ProjectVectorOntoPlane (InputLookDirection, LocalGroundNormal).normalized;
			TargetLookDirection = InputLookDirection;	
		}
	}

	public override void Die ()
	{
		StopMovements ();
		SetVelocity(new Vector3(0,0,0));
//		m_AnimationPawn.doAnimation ("death");
		m_AnimationPawn.Die ();
        AimHelp.renderer.enabled = false;
//		base.Die ();
		if (DeathEffects != null) {
			SpawnDeathEffects ();            
		}
		Player.PawnDied ();
	}

	public void Shoot (AGProjectile proj)
	{
//		m_AnimationPawn.doShoot();
		proj.InitProjectile (Player, TargetLookDirection);
		if (proj.Mesh)
			proj.Mesh.renderer.enabled = true;
	}

	public Vector3 GetMuzzlePos ()
	{
		return transform.position + transform.rotation * MuzzlePointOffset;
	}

	public bool EnoughEnergy (float amount)
	{
		//  Debug.Log("Current Energy: " + Energy.currentValue + "/" + amount);
		return Energy.currentValue - amount >= 0;
	}
	//return bool value: if returns false, energy use does not work
	//other solution: another method, that checks whether there is enough energy??? i think current solution is better
	public void ModifyEnergy (float amount)
	{
		Energy.currentValue += amount;
		Energy.currentValue = Mathf.Clamp (Energy.currentValue, 0, Energy.max);
		// Debug.Log(Energy.currentValue);
	}

	public virtual void ModifySpeedMaximum (float percent)
	{
		//Modifies SpeedMaximm always based on Base VelocityMaximum
		currentVelocityMaximum += VelocityMaximum * percent;
		// Debug.Log(Time.frameCount+" adding "+percent+" newSpeed Maximum: " + currentVelocityMaximum);
	}

	protected void UpdateEnergyIncrease ()
	{
		if (CurrentPlayerState == LightState.Real) {
          
			ModifyEnergy (Time.deltaTime * EnergyGainRate.currentValue);          
		}
	}

	protected void UpdateHealthIncrease ()
	{
		if (CurrentPlayerState == LightState.Real) {
			ModifyHealth (Time.deltaTime * RegenerateHealthValue);
		}
	}

	public override void PerformStateChanges (LightState newState)
	{
		// Debug.Log("Perform state Change " + Player.Action_Ultimate.Ultimate.bActivated);
      
		if ((Player.Action_Ultimate.Ultimate && Player.Action_Ultimate.Ultimate.bActivated) || m_roundIsOver)
			return;
       
		base.PerformStateChanges (newState);
		//  Debug.Log(currentLightState.ToString());
		if (currentLightState != LightState.Between) {
			CurrentPlayerState = newState;
			SetLightZoneStats (false);
			SetLightZoneMesh (true);
			SpawnSwitchEffect ();
			
			//TODO: put a update function in playerController
			Player.playerInterface.UpdateHeadTexture (currentLightState);
			
			AGGame.Instance.getSoundServer ().Play ("transform_pop");
		}
        
	}

	protected void SpawnSwitchEffect ()
	{
		Vector3 EffectSpawnPos = transform.position;
		EffectSpawnPos += transform.up * 1;
		GameObject go = (GameObject)Instantiate (LightStateSwitchEffekt, EffectSpawnPos, transform.rotation);
		go.GetComponent<AGVFX> ().SetPlayer (Player.PlayerID);
		go.transform.parent = this.transform;
	}

	public void SetLightZoneMesh (bool delay)
	{
		if (delay)
			Tools.Wait (0.2f);       

		AGPlayerClass pClass = AGGame.Instance.GetPlayerClass (Player.info.PlayerClass);
        
		GameObject MeshTemplate = currentLightState == LightState.Dream ? pClass.ClassMeshDream : pClass.ClassMeshReal;
		m_myObject = (GameObject)Instantiate (MeshTemplate);
		m_myObject.GetComponent<AGAnimationPawn> ().SetPawn (this);
		m_myObject.name = "PAWN MESH Player " + Player.PlayerID;
        

		  print("Changing Mesh lightstate:"+currentLightState.ToString()+" mesh:"+MeshTemplate.name );
		Tools.CopyTransform (Mesh, m_myObject.transform);   
    
		Destroy (Mesh.gameObject);
		Mesh = m_myObject.transform;      
		updateRenderer (m_myObject);
        
		ApplyMeshSize ();
	}
	
	public void setRenderer (bool enable)
	{
		m_enableRenderer = enable;
		updateRenderer (m_myObject);
	}
	
	private void updateRenderer (GameObject go)
	{
//		Print.Log("Setting Renderer " + m_enableRenderer + " ...");	
		foreach (var renderer in go.GetComponentsInChildren<Renderer>()) {
			renderer.enabled = m_enableRenderer;
            renderer.material.SetColor("_RimColor",  currentLightState == LightState.Dream ? AGGame.Instance.DreamColor : AGGame.Instance.RealColor);
          
		}
	}

	public void Push (AGEffect_Push p)
	{

		this.currentPush = p;
	}
	
	public void Spawn ()
	{
		m_AnimationPawn.doAnimation ("spawn");
		AGGame.Instance.getSoundServer().Play("spawn");
	}

    #region STATS STUFF

	public void ApplyMeshSize ()
	{
		Mesh.transform.localScale = Vector3.one * MeshSize.currentValue;
	}

	public void SetPlayerStats (AGPlayerClassStats newStats, bool setStartValues)
	{
		Energy.ChangeStat (newStats.Energy);
		Health.ChangeStat (newStats.Health);

		Speed.ChangeStat (newStats.Speed);
		Attack.ChangeStat (newStats.Attack);
		Defense.ChangeStat (newStats.Defense);
		MeshSize.ChangeStat (newStats.MeshSize);
		EnergyGainRate.ChangeStat (newStats.EnergyGainRate);

		if (setStartValues) {
			Health.startValue = newStats.Health.startValue;
			Energy.startValue = newStats.Energy.startValue;
			Energy.InitStat ();
			Health.InitStat ();
			Attack.InitStat ();
			Defense.InitStat ();
			Speed.InitStat ();
			MeshSize.InitStat ();
		}


		//Debug.Log("Set Player Stats  " + setStartValues + " value "+Health.currentValue);
	}

	public float GetAttackValue (float Damage)
	{
		//   Debug.Log(" Attack: " + Attack.currentValue);
		return Damage * Attack.currentValue / 100;
	}
	
	public override string getAudioTarget ()
	{
		return "char";
	}
	
	public string getAudioTargetState ()
	{
		return (currentLightState == AGActor.LightState.Real ? "day" : "night");
	}

	public override void TakeDamage (float amount, AGActor damageInstigator)
	{
		foreach (AGEffect e in AppliedEffects) {
			if (e is AGEffect_Invulnerable && e.bActivated) {   
				return;
			}
		}
		float DamageValue = -(amount / 100) * Defense.currentValue + amount; 
		// Debug.Log("inc amount: " + amount + "   newDamage:" + DamageValue + "  Defense: "+Defense.currentValue);
		base.TakeDamage (DamageValue, damageInstigator);
		
		//TODO: richtiger spot fuer vignette, oder lieber in playerPawn?
		Player.playerInterface.UpdateVignette (true, (Health.currentValue / Health.max));
		m_AnimationPawn.doAnimation ("hit");
		
	}

	public void SetLightZoneStats (bool setStartValues)
	{
		//print("SetStats"+ Player.PlayerID + currentLightState.ToString());
		AGPlayerClass pClass = AGGame.Instance.GetPlayerClass (Player.info.PlayerClass);
		SetPlayerStats (currentLightState == LightState.Dream ? pClass.DreamSideStats : pClass.RealSideStats, setStartValues);
	}
    #endregion 
}
