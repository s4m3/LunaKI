using UnityEngine;
using System.Collections;

public class Decision_EnoughHealth : Decision {
	
	public Decision_EnoughHealth(AGPawn self, AGPawn enemy) : base(self, enemy){}
	
	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = new Decision_DistanceEnemy(self, enemy);
		if(!falseNode) this.falseNode = new Decision_EnoughEnergy(self, enemy);
		//is health high enough?
		if(this.self.Health.currentValue > this.self.Health.max * 0.5)
			return trueNode;
		else
			return falseNode;
	}
}
