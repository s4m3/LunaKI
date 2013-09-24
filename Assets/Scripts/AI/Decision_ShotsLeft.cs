using UnityEngine;
using System.Collections;

public class Decision_ShotsLeft : Decision {
	
	public Decision_ShotsLeft(AGPawn self, AGPawn enemy) : base(self, enemy){}

	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = new ActionDecision(ActionDecision.ActionDecisionType.Shoot);
		if(!falseNode) this.falseNode = new ActionDecision(ActionDecision.ActionDecisionType.DashToEnemy);
		//enough shots left?
		if(self.Player.Action_Shot.ShotsLeft > 3)
			return trueNode;
		else
			return falseNode;
	}
}
