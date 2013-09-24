using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionManager : Object {
	List<Action> queue;
	List<Action> active;
	AGPlayerController controller;
	int currentTime;
	
	public ActionManager(AGPlayerController controller)
	{
		this.controller = controller;
	}
	
	public void ResetActionManager()
	{
		queue = new List<Action>();
		active = new List<Action>();
		currentTime = 0;
	}
	
	public void scheduleAction(Action action)
	{
		//action soll nicht zweimal in der queue sein
		Action[] copy = queue.ToArray();
		foreach(Action currAction in copy) {
//			Debug.Log (currAction.GetType());
			if(currAction.GetType().Equals(action.GetType()))
				return;
		}
		queue.Add(action);
		//Debug.Log("count:" + queue.Count);
	}
	
	public void execute()
	{
//		Debug.Log("executing in action manager, count:" + queue.Count);
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
		//Debug.Log (1);
		Action[] copy = queue.ToArray();
		Action[] copyActive = active.ToArray();
		foreach(Action currAction in copy)
		{
			//Debug.Log ("currentTime: "+ currentTime);
			//Debug.Log ("expiryTime: " + currAction.expiryTime);
			if(currAction.expiryTime < currentTime)
				queue.Remove(currAction);
			
			foreach(Action activeAction in copyActive)
			{
				if(currAction.canDoBoth(activeAction))
				{
					queue.Remove(currAction);
					active.Add(currAction);
					Debug.Log("added action");
				}
			}
			
		}
		//if nothing interrupts AND no action is added AND active is empty -> insert new action out of queue with highest prio
		if(active.Count == 0) 
		{
			Action actionWithHighestPrio = null;
			int currentHighestPrio = -1;
			bool found = false;
			foreach(Action currAction in copy)
			{
				if(currAction.priority > currentHighestPrio)
				{
					actionWithHighestPrio = currAction;
					currentHighestPrio = currAction.priority;
					found = true;
				}
			}
			if(found) active.Add(actionWithHighestPrio);
		}
				
		//Debug.Log (2);

		Action[] activeCopy = active.ToArray();
		foreach(Action actAction in activeCopy)
		{
			if(actAction.isComplete())
			{
//				Debug.Log("is complete");
				active.Remove(actAction);
			}
			else
			{
//				Debug.Log("executing action");
				actAction.execute();
			}
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
//		Debug.Log("highestPrio" + highestPrio );
		return highestPrio;
	}
	
	public Action createAction(ActionDecision.ActionDecisionType actionType)
	{
		Action action;
		switch(actionType)
		{
		case ActionDecision.ActionDecisionType.None:
			action = new Action(controller);
			Debug.Log("new Action: None");
			break;
		case ActionDecision.ActionDecisionType.ChargeHealth:
			action = new Action_ChargeHealth(controller, currentTime);
			Debug.Log("new Action: ChargeHealth");
			break;
		case ActionDecision.ActionDecisionType.ChargeEnergy:
			action = new Action_ChargeEnergy(controller, currentTime);
			Debug.Log("new Action: ChargeEnergy");
			break;
		case ActionDecision.ActionDecisionType.DashToEnemy:
			action = new Action_DashToEnemy(controller, currentTime);
			Debug.Log("new Action: DashToEnemy");
			break;
		case ActionDecision.ActionDecisionType.MoveToEnemy:
			action = new Action_MoveToEnemy(controller, currentTime);
			Debug.Log("new Action: MoveToEnemy");
			break;
		case ActionDecision.ActionDecisionType.MoveToEnemyAndMelee:
			action = new Action_MoveAndMelee(controller, currentTime);
			Debug.Log("new Action: MoveToEnemyAndMelee");
			break;
		case ActionDecision.ActionDecisionType.ActivateUltimate:
			action = new Action_ActivateUltimate(controller, currentTime);
			Debug.Log("new Action: ActivateUltimate");
			break;
		case ActionDecision.ActionDecisionType.Shoot:
			action = new Action_Shoot(controller, currentTime);
			Debug.Log("new Action: Shoot");
			break;
//		case ActionDecision.ActionDecisionType.Attack:
//			action = new Action_Attack(controller, currentTime);
//			Debug.Log("new Action: Attack");
//			break;
//		case ActionDecision.ActionDecisionType.Attack:
//			action = new Action_Attack(controller, currentTime);
//			Debug.Log("new Action: Attack");
//			break;
		default:
			action = new Action(controller);
			Debug.Log("new Action: None");
			break;
		}
		return action;
	}
}
