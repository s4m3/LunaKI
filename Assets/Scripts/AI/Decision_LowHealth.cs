using UnityEngine;
using System.Collections;

public class Decision_LowHealth : Decision {
	
	public Decision_LowHealth(AGPawn self, AGPawn enemy, float difficulty) : base(self, enemy, difficulty){}
	
	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = new ActionDecision(ActionDecision.ActionDecisionType.ChargeHealth);
		if(!falseNode) this.falseNode = new Decision_EnemyLowHealth(self, enemy, difficulty);
		//if health is low
		if(this.self.Health.currentValue < this.self.Health.max * 0.4)
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
