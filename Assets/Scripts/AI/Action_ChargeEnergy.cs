using UnityEngine;
using System.Collections;

public class Action_ChargeEnergy : Action {
	
	public Action_ChargeEnergy(AGPlayerController controller, int currentTime) : base(controller) 
	{
		this.expiryTime = 200 + currentTime;
		this.priority = 1;
	}
	
	public override bool canInterrupt ()
	{
		return false;
	}

	public override bool canDoBoth (Action otherAction)
	{
		return true;
	}

	public override bool isComplete ()
	{
		return (controller.pawn.Energy.currentValue > controller.pawn.Energy.max * 0.9);
	}

	public override void execute ()
	{
		controller.MoveToDark();
	}
}

