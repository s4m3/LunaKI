using UnityEngine;
using System.Collections;

public class AIAction_ChargeHealth : AIAction {

	public override DecisionTreeNode makeDecision ()
	{
		this.actionType = AIAction.ActionType.ChargeHealth;
		return this;
	}
}
