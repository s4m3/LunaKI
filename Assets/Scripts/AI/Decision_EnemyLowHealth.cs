using UnityEngine;
using System.Collections;

public class Decision_EnemyLowHealth : Decision {
	
	public Decision_EnemyLowHealth(AGPawn self, AGPawn enemy, float difficulty) : base(self, enemy, difficulty){}
	
	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = new ActionDecision(ActionDecision.ActionDecisionType.MoveToEnemy);
		if(!falseNode) this.falseNode = new Decision_HighUlti(self, enemy, difficulty);
		//if enemy has low health
		if(this.enemy.Health.currentValue < this.enemy.Health.max * 0.15)
		{
			if(ChanceForRightDecision())
				return trueNode;
			else
				return falseNode;
		}
		else
		{
			if(ChanceForRightDecision())
				return falseNode;
			else
				return trueNode;
		}
	}
}
