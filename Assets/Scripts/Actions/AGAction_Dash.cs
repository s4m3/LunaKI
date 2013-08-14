using UnityEngine;
using System.Collections;

public class AGAction_Dash : AGActionOverTime {

    public AGEffect_Push.PushData DashData;

    public bool AllowLook;
    public bool AllowActions;

    protected AGEffect_Push Dash;
    protected AGEffect_Stun Stun;
	
	protected override string getSound ()
	{
		return "dash";	
	}
	
    protected override void StartAction()
    {
        if (bHoldButton) return;
        Vector3 dir;

        if (owner.pawn.Velocity.magnitude > 0.02f && owner.pawn.TargetVelocity != Vector3.zero)
        {
            dir = owner.pawn.TargetVelocity;
        } else {
            dir = owner.pawn.GetLookDirection();
        }
       // Debug.Log(dir);
        Dash = owner.pawn.gameObject.AddComponent<AGEffect_Push>();
        owner.pawn.ApplyEffect(Dash);
        DashData.PushStartDir = dir;
        DashData.Duration = Duration;
        Dash.InitPush(DashData);

      //  Stun = owner.pawn.gameObject.AddComponent<AGEffect_Stun>();
       // Stun.BlocksMovement = false;
        //Stun.BlocksAction = !AllowActions;
       // Stun.BlocksLook = !AllowLook;
        //owner.pawn.ApplyEffect(Stun);

        base.StartAction();
    }

 
    protected override void PerformAction()
    {
       // Vector3 updatedVelocity = owner.pawn.TargetVelocity;
     
        base.PerformAction();
       // Debug.Log(owner.pawn.InputMovementDir);
        if(owner.pawn.InputMovementDir != Vector3.zero){

            Dash.SetPushDirection(owner.pawn.InputMovementDir);
        }
        
    }
   
    public override void Deactivate()
    {
       if(Dash) Dash.EffectEnd();
       if(Stun) Stun.EffectEnd();

       base.Deactivate();       
    } 
    
}
