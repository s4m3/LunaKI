using UnityEngine;
using System.Collections;

public class Action : Object {
	
	public int expiryTime;
	public int priority;
	public AGPlayerController controller;
	
	public Action(AGPlayerController controller)
	{
		this.controller = controller;
	}
	
	public Action(AGPlayerController controller, int expiryTime, int priority) : this(controller)
	{
		this.expiryTime = expiryTime;
		this.priority = priority;
	}
	
	public virtual bool canInterrupt()
	{
		return false;
	}
	
	public virtual bool canDoBoth(Action otherAction)
	{
		return false;
	}
	
	public virtual bool isComplete()
	{
		return true;
	}
	
	public virtual void execute()
	{
		
	}
}
