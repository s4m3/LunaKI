using UnityEngine;
using System.Collections;

public class ActionDecision : DecisionTreeNode {
	
	public enum ActionDecisionType {None, Attack, ChargeHealth}
	public ActionDecisionType actionType;
	
	public override DecisionTreeNode makeDecision ()
	{
		return this;
	}
}
