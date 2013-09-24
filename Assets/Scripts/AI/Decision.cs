using UnityEngine;
using System.Collections;

public class Decision : DecisionTreeNode {
	protected DecisionTreeNode trueNode;
	protected DecisionTreeNode falseNode;
	
	public Decision(AGPawn self, AGPawn enemy)
	{
		this.self = self;
		this.enemy = enemy;
	}
	protected virtual DecisionTreeNode getBranch()
	{
		if(!trueNode)
		{
			this.trueNode = new Decision_EnemyInSight(this.self, this.enemy);
		}
		return trueNode;
	}
	
	public override DecisionTreeNode makeDecision ()
	{
		DecisionTreeNode branch = getBranch();
		return branch.makeDecision ();
	}
}