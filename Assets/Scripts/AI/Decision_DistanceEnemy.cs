using UnityEngine;
using System.Collections;

public class Decision_DistanceEnemy : Decision {
	
	public Decision_DistanceEnemy(AGPawn self, AGPawn enemy, float difficulty) : base(self, enemy, difficulty){}
	
	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = new ActionDecision(ActionDecision.ActionDecisionType.MoveToEnemyAndMelee);
		if(!falseNode) this.falseNode = new Decision_ShotsLeft(self, enemy, difficulty);
		//is enemy close?
		if(Vector3.Distance(self.transform.position, enemy.transform.position) < 1.7f && ChanceForRightDecision())
			return trueNode;
		else
			return falseNode;
	}
}
