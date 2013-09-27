using UnityEngine;
using System.Collections;

public class Action_MoveAndMelee : Action {
	
	private bool completion = false;
	
	public Action_MoveAndMelee(AGPlayerController controller, int currentTime) : base(controller) 
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
		return true;
	}

	public override bool isComplete ()
	{
		return completion;
	}

	public override void execute ()
	{
		controller.MeleeAtEnemy();
		completion = true;
	}
}
