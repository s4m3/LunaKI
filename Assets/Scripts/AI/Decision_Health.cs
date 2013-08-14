using UnityEngine;
using System.Collections;

public class Decision_Health : Decision {
	
	protected override DecisionTreeNode getBranch()
	{
		if(!trueNode) this.trueNode = CreateInstance<AIAction_Attack>();
		if(!falseNode) this.falseNode = CreateInstance<AIAction_ChargeHealth>();
		//if health falls under 10 percent, go charge
		if(this.self.Health.currentValue < this.self.Health.max * 0.1)
			return falseNode;
		else
			return trueNode;
	}
}
