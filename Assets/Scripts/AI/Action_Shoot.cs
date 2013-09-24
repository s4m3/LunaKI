using UnityEngine;
using System.Collections;

public class Action_Shoot : Action {
	
	public Action_Shoot(AGPlayerController controller, int currentTime) : base(controller) 
	{
		priority = 9;
		expiryTime = 5 + currentTime;
	}
	
	public override bool canInterrupt ()
	{
		return true;
	}

	public override bool canDoBoth (Action otherAction)
	{
		return false;
	}

	public override bool isComplete ()
	{
		return false;
	}

	public override void execute ()
	{
		controller.Shoot();
	}
}
