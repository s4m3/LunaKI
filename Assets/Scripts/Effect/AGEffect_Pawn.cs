using UnityEngine;
using System.Collections;

public class AGEffect_Pawn : AGEffect {
    protected AGPawn pawnOwner;

    public override bool EffectApplied(AGActor _owner)
    {
       
        // print("Effect Applied to pawn");
        if (_owner is AGPawn)
        {
            pawnOwner = (AGPawn)_owner;
            return base.EffectApplied(_owner);

            //if(PlayStunAnimation) pawnOwner.PlayStunAnimation()..            
        }
        else
        {
            Destroy(this.gameObject);
        }
        return false;
    }

}
