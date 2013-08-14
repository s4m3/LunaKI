using UnityEngine;
using System.Collections;

public class Action : ScriptableObject {
	public int expiryTime;
	public int priority;
		
	public bool canInterrupt()
	{
		return false;
	}
	
	public bool canDoBoth(Action otherAction)
	{
		return false;
	}
	
	public bool isComplete()
	{
		return false;
	}
	
	public void execute()
	{
		
	}
}
