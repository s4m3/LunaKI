using UnityEngine;
using System.Collections;

public class AGEffect_Scale : AGEffect_Pawn
{
    public AnimationCurve curve;
    public float ScaleFactor;
    float lastScaleValue;
	// Use this for initialization
	void Start () {
        DoublingBehavior = DoublingBehaviours.ignore;
	}

    public override bool EffectApplied(AGActor _owner)
    {
        if (base.EffectApplied(_owner))
        {
            ScaleFactor = (pawnOwner.MeshSize.baseValue * ScaleFactor) - pawnOwner.MeshSize.baseValue;
            
            return true;
        }
        return false;
    }
    protected override void EffectTick()
    {
        SetScale();
       
        base.EffectTick();        
    }

    public override void EffectEnd()
    {
       
        SetScale(-lastScaleValue);
        base.EffectEnd();
    }

    protected void SetScale(float targetScale)
    {
        if (targetScale == 0) return;
        pawnOwner.MeshSize.BuffStat(targetScale);
        pawnOwner.ApplyMeshSize();
    }

    protected void SetScale(){
           float currentScaleValue = curve.Evaluate(GetEffectActiveTimePercent()) * ScaleFactor;         
           float delta = currentScaleValue - lastScaleValue;  
        
           SetScale(delta);
           lastScaleValue = currentScaleValue;        
    }
}
