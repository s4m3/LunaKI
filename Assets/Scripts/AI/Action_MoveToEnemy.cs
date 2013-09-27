using UnityEngine;
using System.Collections;

public class Action_MoveToEnemy : Action {
	
	public Action_MoveToEnemy(AGPlayerController controller, int currentTime) : base(controller) 
	{
		priority = 4;
		expiryTime = 1000 + currentTime;
	}
	
	public override bool canInterrupt ()
	{
		return true;
	}

	public override bool canDoBoth (Action otherAction)
	{
		if(otherAction.GetType() == this.GetType()) return false;
		else return true;
	}

	public override bool isComplete ()
	{
		return controller.IsCloseToEnemy();
	}

	public override void execute ()
	{
		controller.MoveAIPlayer();
	}
}
