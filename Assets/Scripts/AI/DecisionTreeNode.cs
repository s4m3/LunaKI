using UnityEngine;
using System.Collections;

public class DecisionTreeNode : Object {
	
	protected AGPawn self;
	protected AGPawn enemy;
	
	public virtual DecisionTreeNode makeDecision()
	{
		return this;
	}
	
	public void setupBaseValues(AGPawn _enemy, AGPawn _self)
	{
		this.enemy = _enemy;
		this.self = _self;
	}
}
