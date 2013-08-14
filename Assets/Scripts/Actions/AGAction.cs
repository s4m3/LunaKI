using UnityEngine;
using System.Collections;

public class AGAction : MonoBehaviour {
	public int Cost;
	public float CoolDown;
    public AGVFX VisualEffects;
    public string AnimName;

    [HideInInspector]
    public bool bHoldButton;
     [HideInInspector]
    public float ActionStartTime;
    protected float currentCoolDown;
 

    public AGCamera.CamShake ActivationCamShake;

    [HideInInspector]
	public AGPlayerController owner;
	protected float LastUseTime;

    
	// Use this for initialization
    public virtual void Start()
    {
        currentCoolDown = CoolDown;
    }

	public virtual void SetOwner(AGPlayerController player){	
		//Debug.Log("Owner = "+player.info.PlayerName);
		owner = player;
        LastUseTime = -CoolDown;
      
	}
    public virtual bool Activate ()
	{
       
		if (owner.pawn.EnoughEnergy (Cost) && LastUseTime + currentCoolDown < Time.time) {
//			m_AnimationPawn.doAnimation (
			owner.pawn.PlayAnimation(this.AnimName);
//				gameObject.BroadcastMessage ("doAnimation", this.AnimName);
			StartAction ();
			owner.pawn.ModifyEnergy (-Cost);
			return true;
		}
		return false;
	}
	
	protected virtual void PlaySound ()
	{
		string s = getSound ();
		string ss = getSoundState ();
		if (ss != null)
			s += "_" + ss;
		AGGame.Instance.getSoundServer ().Play (s);
	}
	
	protected virtual string getSound ()
	{
		return "shoot";
	}
	
	protected virtual string getSoundState ()
	{
		return null;
	}

	protected virtual void StartAction(){
		PlaySound();
        if (!bHoldButton)
        {
            ActionStartTime = Time.time;
        }
        bHoldButton = true;
        LastUseTime = Time.time;
        if (VisualEffects != null)
        {
          GameObject go =  (GameObject) Instantiate(VisualEffects.gameObject, owner.pawn.transform.position, owner.pawn.transform.rotation);
          go.GetComponent<AGVFX>().SetPlayer(owner.PlayerID);
            Quaternion oldRot = go.transform.rotation;
          
          go.transform.rotation = Quaternion.FromToRotation(go.transform.forward, owner.pawn.transform.up) * oldRot;

        }
        PlayActivationCamShake();
       
	}
    protected virtual void PlayActivationCamShake()
    {
        if (ActivationCamShake.Duration > 0) owner.AGCam.StartShake(ActivationCamShake);
    }
    public virtual void ReleaseButton()
    {
        //Debug.Log("ReleaseButton "+gameObject.name);
        bHoldButton = false;        
    }
    public virtual void PawnDied()
    {
        
    }

    public virtual void Reset()
    {
        LastUseTime = 0;
        bHoldButton = false;
    }
}
