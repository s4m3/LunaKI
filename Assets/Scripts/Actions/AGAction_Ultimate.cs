using UnityEngine;
using System.Collections;

public class AGAction_Ultimate : AGAction {
    public GameObject UltimateEffect;
   
    //[HideInInspector]
    public AGEffect_Ultimate Ultimate;
	
	protected override string getSound ()
	{
		return "ultimate";	
	}
	
	// Use this for initialization
    protected override void StartAction()
    {
           Ultimate = ((GameObject)(Instantiate(UltimateEffect))).GetComponent<AGEffect_Ultimate>();
            if (owner.pawn.ApplyEffect(Ultimate))
            {                  
                base.StartAction();
            }        
    }
    public override bool Activate()
    {
        if (owner.pawn.CurrentPlayerState != AGActor.LightState.Dream) return false;
        else return base.Activate();
    }
}
