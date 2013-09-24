using UnityEngine;
using System.Collections;

public class Decision_LowHealth : Decision {
	
	public Decision_LowHealth(AGPawn self, AGPawn enemy) : base(self, enemy){}
	
	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = new ActionDecision(ActionDecision.ActionDecisionType.ChargeHealth);
		if(!falseNode) this.falseNode = new Decision_EnemyLowHealth(self, enemy);
		//if health is low
		if(this.self.Health.currentValue < this.self.Health.max * 0.4)
			return trueNode;
		else
			return falseNode;
	}
}
