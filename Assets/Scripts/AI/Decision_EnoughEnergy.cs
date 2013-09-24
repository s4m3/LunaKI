using UnityEngine;
using System.Collections;

public class Decision_EnoughEnergy : Decision {
	
	public Decision_EnoughEnergy(AGPawn self, AGPawn enemy) : base(self, enemy){}
	
	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = new Decision_InDarkZone(self, enemy);
		if(!falseNode) this.falseNode = new Decision_DistanceEnemy(self, enemy);
		//has max ultimate energy?
		if(this.self.Energy.currentValue == this.self.Energy.max)
			return trueNode;
		else
			return falseNode;
	}
}
