using UnityEngine;
using System.Collections;

public class DecisionTreeNode : Object {
	
	protected AGPawn self;
	protected AGPawn enemy;
	protected float difficulty;
	
	public virtual DecisionTreeNode makeDecision()
	{
		return this;
	}
	
}
