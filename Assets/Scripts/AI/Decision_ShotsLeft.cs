using UnityEngine;
using System.Collections;

public class Decision_ShotsLeft : Decision {
	
	public Decision_ShotsLeft(AGPawn self, AGPawn enemy, float difficulty) : base(self, enemy, difficulty){}

	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = new ActionDecision(ActionDecision.ActionDecisionType.Shoot);
		if(!falseNode) this.falseNode = new ActionDecision(ActionDecision.ActionDecisionType.DashToEnemy);
		//enough shots left?
		if(self.Player.Action_Shot.ShotsLeft > 3 && ChanceForRightDecision())
			return trueNode;
		else
			return falseNode;
	}
}
