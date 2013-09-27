using UnityEngine;
using System.Collections;

public class Decision : DecisionTreeNode {
	protected DecisionTreeNode trueNode;
	protected DecisionTreeNode falseNode;
	
	public Decision(AGPawn self, AGPawn enemy, float difficulty)
	{
		this.self = self;
		this.enemy = enemy;
		this.difficulty = difficulty;
	}
	protected virtual DecisionTreeNode getBranch()
	{
		if(!trueNode)
		{
			this.trueNode = new Decision_EnemyInSight(this.self, this.enemy, this.difficulty);
		}
		return trueNode;
	}
	
	protected virtual bool ChanceForRightDecision()
	{
		return (Random.value <= difficulty);
	}
	
	public override DecisionTreeNode makeDecision ()
	{
		DecisionTreeNode branch = getBranch();
		return branch.makeDecision ();
	}
}