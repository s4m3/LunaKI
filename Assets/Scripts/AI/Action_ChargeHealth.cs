using UnityEngine;
using System.Collections;

public class Action_ChargeHealth : Action {
	
	public Action_ChargeHealth(AGPlayerController controller, int currentTime) : base(controller) 
	{
		priority = 10;
		expiryTime = 2000 + currentTime;
	}
	
	public override bool canInterrupt ()
	{
		return true;
	}

	public override bool canDoBoth (Action otherAction)
	{
		return (otherAction is Action_Shoot || otherAction is Action_MoveAndMelee);
	}

	public override bool isComplete ()
	{
		return (controller.pawn.Health.currentValue > controller.pawn.Health.max * 0.5);
	}

	public override void execute ()
	{
		controller.MoveToSun();
	}
}
