using UnityEngine;
using System.Collections;

public class Decision_InDarkZone : Decision {
	
	public Decision_InDarkZone(AGPawn self, AGPawn enemy, float difficulty) : base(self, enemy, difficulty){}
	
	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = new ActionDecision(ActionDecision.ActionDecisionType.ActivateUltimate);
		if(!falseNode) this.falseNode = new Decision_DistanceEnemy(self, enemy, difficulty);
		//his in Dream/Dark Zone?
		if(this.self.CurrentPlayerState == AGActor.LightState.Dream && ChanceForRightDecision())
			return trueNode;
		else
			return falseNode;
	}
}
