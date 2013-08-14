using UnityEngine;
using System.Collections;

public class AGEffect_ModifySpeed : AGEffect_Pawn {
    
    public AnimationCurve curve;
    public float SpeedModification;
    float currentModificationValue;

	
	// Update is called once per frame
    protected override void EffectTick()
    {
        float newCurveValue = curve.Evaluate( GetEffectActiveTimePercent() ) * pawnOwner.Speed.currentValue;

        float delta = newCurveValue - currentModificationValue;

        pawnOwner.Speed.BuffStat(delta);
       
        currentModificationValue += delta;
        base.EffectTick();
    }

    public override void EffectEnd()
    {
        pawnOwner.Speed.BuffStat(-currentModificationValue);
     //   print("End speedMod" + currentModificationValue)  
        base.EffectEnd();
      // 
      
    }
}
