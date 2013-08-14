using UnityEngine;
using System.Collections;

public class AGEffect_Stun : AGEffect_Pawn {
    public bool BlocksMovement = true;
    public bool BlocksLook = true;
    public bool StopMovements = true;
    public bool PlayStunAnimation;
   
    public override bool EffectApplied(AGActor _owner)
    {
     
        if (base.EffectApplied(_owner))
        {
      
            if (StopMovements) pawnOwner.StopMovements();
            if (pawnOwner.InputLook && BlocksLook) pawnOwner.InputLook = false;
            if (pawnOwner.InputMove && BlocksMovement) pawnOwner.InputMove = false;
            
            return true;
        }
        return false;
       

    }

    public override void EffectEnd()
    {
        bActivated = false;
        if (BlocksMovement) pawnOwner.UpdateInputMove();
        if (BlocksLook) pawnOwner.UpdateInputLook();
       // print("Effect_Stun ended");
        base.EffectEnd();
       
    }
}
