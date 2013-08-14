using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionManager : ScriptableObject {
	List<Action> queue;
	List<Action> active;
	int currentTime;
	
	public void ResetActionManager()
	{
		queue = new List<Action>();
		active = new List<Action>();
		currentTime = 0;
	}
	
	public void scheduleAction(Action action)
	{
		queue.Add(action);
	}
	
	public void execute()
	{
		currentTime += 1;
		foreach(Action action in queue)
		{
			if(action.priority <= getHighestPriority(active))
				break;
			
			if(action.canInterrupt())
			{
				active.Clear();
				active.Add(action);
			}
		}
		Action[] copy = queue.ToArray();
		foreach(Action currAction in copy)
		{
			if(currAction.expiryTime < currentTime)
				queue.Remove(currAction);
			
			foreach(Action activeAction in active)
			{
				if(currAction.canDoBoth(activeAction))
				{
					queue.Remove(currAction);
					active.Add(currAction);
				}
			}
		}
		
		Action[] activeCopy = active.ToArray();
		foreach(Action actAction in activeCopy)
		{
			if(actAction.isComplete())
				active.Remove(actAction);
			else
				actAction.execute();
		}
		
	}
	
	private int getHighestPriority(List<Action> activeActions)
	{
		int highestPrio = -1;
		foreach(Action action in activeActions)
		{
			if(action.priority > highestPrio)
				highestPrio = action.priority;
		}
		return highestPrio;
	}
	
	public Action createAction(AIAction.ActionType actionType)
	{
		Action action = ScriptableObject.CreateInstance<Action>();
		return action;
	}
}
