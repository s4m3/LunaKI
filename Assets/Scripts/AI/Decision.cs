using UnityEngine;
using System.Collections;

public class Decision : DecisionTreeNode {
	protected DecisionTreeNode trueNode;
	protected DecisionTreeNode falseNode;
	//protected var testValue;



	protected virtual DecisionTreeNode getBranch()
	{
		if(!trueNode)
		{
			this.trueNode = new Decision_Health();
			this.trueNode.setupBaseValues(enemy, self);
		}
		return trueNode;
	}
	
	public override DecisionTreeNode makeDecision ()
	{
		DecisionTreeNode branch = getBranch();
		return branch.makeDecision ();
	}
}