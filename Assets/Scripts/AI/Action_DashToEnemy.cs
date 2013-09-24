using UnityEngine;
using System.Collections;

public class Action_DashToEnemy : Action {
	
	public Action_DashToEnemy(AGPlayerController controller, int currentTime) : base(controller) 
	{
		priority = 10;
		expiryTime = 20 + currentTime;
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
		controller.DashToEnemy();
	}
}
