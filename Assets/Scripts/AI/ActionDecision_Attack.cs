using UnityEngine;
using System.Collections;

public class ActionDecision_Attack : ActionDecision {

	public override DecisionTreeNode makeDecision ()
	{
		this.actionType = ActionDecision.ActionDecisionType.Attack;
		return this;
	}
}
