using UnityEngine;
using System.Collections;

public class Decision_EnoughHealth : Decision {
	
	public Decision_EnoughHealth(AGPawn self, AGPawn enemy, float difficulty) : base(self, enemy, difficulty){}
	
	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = new Decision_DistanceEnemy(self, enemy, difficulty);
		if(!falseNode) this.falseNode = new Decision_EnoughEnergy(self, enemy, difficulty);
		//is health high enough?
		if(this.self.Health.currentValue > this.self.Health.max * 0.5 && ChanceForRightDecision())
			return trueNode;
		else
			return falseNode;
	}
}
