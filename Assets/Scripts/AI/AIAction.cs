using UnityEngine;
using System.Collections;

public class AIAction : DecisionTreeNode {
	
	public enum ActionType {None, Attack, ChargeHealth}
	public ActionType actionType;
	
	public override DecisionTreeNode makeDecision ()
	{
		return this;
	}
}
