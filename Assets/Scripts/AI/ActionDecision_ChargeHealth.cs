using UnityEngine;
using System.Collections;

public class ActionDecision_ChargeHealth : ActionDecision {

	public override DecisionTreeNode makeDecision ()
	{
		this.actionType = ActionDecision.ActionDecisionType.ChargeHealth;
		return this;
	}
}
