using UnityEngine;
using System.Collections;

public class Decision_EnemyInSight : Decision {
	
	public Decision_EnemyInSight(AGPawn self, AGPawn enemy) : base(self, enemy){}
	
	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = new Decision_EnoughHealth(self, enemy);
		if(!falseNode) this.falseNode = new Decision_LowHealth(self, enemy);
		//if enemy is in sight
		if(Vector3.Distance(self.transform.position, enemy.transform.position) < 12)
			return trueNode;
		else
			return falseNode;
	}
}
