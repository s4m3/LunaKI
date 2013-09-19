using UnityEngine;
using System.Collections;

public class Action_Attack : Action {
	
	public Action_Attack(AGPlayerController controller, int currentTime) : base(controller) 
	{
		this.expiryTime = 100 + currentTime;
		this.priority = 2;
	}
	
	public override bool canInterrupt ()
	{
		return base.canInterrupt ();
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
		Debug.Log("executing");
		controller.MoveAIPlayer();
	}
}
