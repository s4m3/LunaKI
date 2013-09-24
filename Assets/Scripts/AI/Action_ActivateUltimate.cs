using UnityEngine;
using System.Collections;

public class Action_ActivateUltimate : Action {
	
	public Action_ActivateUltimate(AGPlayerController controller, int currentTime) : base(controller) 
	{
		this.expiryTime = 20 + currentTime;
		this.priority = 3;
	}
	
	public override bool canInterrupt ()
	{
		return base.canInterrupt ();
	}

	public override bool canDoBoth (Action otherAction)
	{
		return true;
	}

	public override bool isComplete ()
	{
		foreach(AGEffect effect in controller.pawn.AppliedEffects)
		{
			if(effect is AGEffect_Ultimate)
				return true;
		}
		return false;
	}

	public override void execute ()
	{
		controller.ExecuteAIAction(Vector3.zero, Vector3.zero, controller.Action_Ultimate);
	}
}
