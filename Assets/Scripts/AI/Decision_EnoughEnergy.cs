using UnityEngine;
using System.Collections;

public class Decision_EnoughEnergy : Decision {
	
	public Decision_EnoughEnergy(AGPawn self, AGPawn enemy, float difficulty) : base(self, enemy, difficulty){}
	
	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = new Decision_InDarkZone(self, enemy, difficulty);
		if(!falseNode) this.falseNode = new Decision_DistanceEnemy(self, enemy, difficulty);
		//has max ultimate energy?
		if(this.self.Energy.currentValue == this.self.Energy.max)
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
