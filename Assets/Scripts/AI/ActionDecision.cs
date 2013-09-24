using UnityEngine;
using System.Collections;

public class ActionDecision : DecisionTreeNode {
	
	public enum ActionDecisionType {None, Attack, ChargeHealth, ChargeEnergy, MoveToEnemy, MoveToEnemyAndMelee, Shoot, DashToEnemy, ActivateUltimate}
	public ActionDecisionType actionType;
	
	public ActionDecision(ActionDecisionType type)
	{
		this.actionType = type;
	}
	
	public override DecisionTreeNode makeDecision ()
	{
		return this;
	}
}
