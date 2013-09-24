using UnityEngine;
using System.Collections;

public class Decision_EnemyLowHealth : Decision {
	
	public Decision_EnemyLowHealth(AGPawn self, AGPawn enemy) : base(self, enemy){}
	
	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = new ActionDecision(ActionDecision.ActionDecisionType.MoveToEnemy);
		if(!falseNode) this.falseNode = new Decision_HighUlti(self, enemy);
		//if enemy has low health
		if(this.enemy.Health.currentValue < this.enemy.Health.max * 0.15)
			return trueNode;
		else
			return falseNode;
	}
}
