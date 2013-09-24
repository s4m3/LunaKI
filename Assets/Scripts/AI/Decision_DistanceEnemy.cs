using UnityEngine;
using System.Collections;

public class Decision_DistanceEnemy : Decision {
	
	public Decision_DistanceEnemy(AGPawn self, AGPawn enemy) : base(self, enemy){}
	
	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = new ActionDecision(ActionDecision.ActionDecisionType.MoveToEnemyAndMelee);
		if(!falseNode) this.falseNode = new Decision_ShotsLeft(self, enemy);
		//is enemy close?
		Vector3 asd = self.transform.position;
		Vector3 avs = enemy.transform.position;
		if(Vector3.Distance(self.transform.position, enemy.transform.position) < 1.2f)
			return trueNode;
		else
			return falseNode;
	}
}
