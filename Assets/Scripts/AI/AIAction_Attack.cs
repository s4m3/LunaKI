using UnityEngine;
using System.Collections;

public class AIAction_Attack : AIAction {

	public override DecisionTreeNode makeDecision ()
	{
		this.actionType = AIAction.ActionType.Attack;
		return this;
	}
}
