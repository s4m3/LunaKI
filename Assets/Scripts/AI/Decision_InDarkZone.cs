using UnityEngine;
using System.Collections;

public class Decision_InDarkZone : Decision {
	
	public Decision_InDarkZone(AGPawn self, AGPawn enemy) : base(self, enemy){}
	
	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = new ActionDecision(ActionDecision.ActionDecisionType.ActivateUltimate);
		if(!falseNode) this.falseNode = new Decision_DistanceEnemy(self, enemy);
		//his in Dream/Dark Zone?
		if(this.self.CurrentPlayerState == AGActor.LightState.Dream)
			return trueNode;
		else
			return falseNode;
	}
}
