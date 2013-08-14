using UnityEngine;
using System.Collections;

public class AGActionOverTime : AGAction {
    public float Duration;
    protected bool bActivated;
    protected bool bWasActivated;
    public bool bUseShake;
    public GameObject TravelEffectTemplate;
    AGVFX TravelEffect;
    public override void SetOwner(AGPlayerController player)
    {
        base.SetOwner(player);
        if (bUseShake)
        {
            ActivationCamShake.Duration = Duration;
            ActivationCamShake.BlendOut = Duration / 2;            
        }
    }


    //Gets Called during activation duration
    protected virtual void PerformAction()
    {
        
    }

	void FixedUpdate (){
      
        if (IsActive())
        {
            PerformAction();
        }
        else if (bWasActivated)
        {
            Deactivate();
        }
   
	}
    
    public override bool Activate()
    {
        return base.Activate();  
        
    }

    protected override void StartAction()
    {
        base.StartAction();
        //Debug.Log("StartAction");
        bWasActivated = true;
        bActivated = true;
        if (TravelEffectTemplate != null)
        {
            GameObject obj = (GameObject)Instantiate(TravelEffectTemplate, owner.pawn.transform.position, owner.pawn.transform.rotation);
            obj.transform.parent = owner.pawn.transform;
            TravelEffect = obj.GetComponentInChildren<AGVFX>();
        }
    }

    public bool IsActive(){
        return LastUseTime + Duration > Time.time && bActivated;
    }

    /*Cancels Action*/
    public virtual void Deactivate()
    {
        bActivated = false;
        bWasActivated = false;
       
        if (TravelEffect != null)
        {
            TravelEffect.transform.parent = null;
            TravelEffect.Stop();
        }
        
    }
}
