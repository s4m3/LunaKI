﻿using UnityEngine;
using System.Collections;

public class Decision_HighUlti : Decision {
	
	public Decision_HighUlti(AGPawn self, AGPawn enemy) : base(self, enemy){}
	
	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = new ActionDecision(ActionDecision.ActionDecisionType.MoveToEnemy);
		if(!falseNode) this.falseNode = new ActionDecision(ActionDecision.ActionDecisionType.ChargeEnergy);
		//if self has enough ultimate energy
		if(this.self.Energy.currentValue > this.self.Energy.max * 0.85)
			return trueNode;
		else
			return falseNode;
	}
}
