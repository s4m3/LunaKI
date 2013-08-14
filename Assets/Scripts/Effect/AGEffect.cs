using UnityEngine;
using System.Collections;

public class AGEffect : MonoBehaviour {
    public float EffectTime;
    [HideInInspector]
    public bool bActivated;
    protected float EffectStartTime;
    [HideInInspector]
    public AGActor owner;
    public bool BlocksAction;

    public enum DoublingBehaviours
    {
        stack, reapply, update, ignore
    }
    public DoublingBehaviours DoublingBehavior;

    public float GetEffectActiveTime()
    {
        return Time.time - EffectStartTime;
    }

    public float GetEffectActiveTimePercent()
    {
        return GetEffectActiveTime() / EffectTime;
    }
    public virtual bool EffectApplied(AGActor _owner)
    {
        EffectStartTime = Time.time;
        owner = _owner;
        bActivated = true;
        return bActivated;
    }
 
    public virtual void EffectEnd()
    {
        owner.NotifyEffectEnded(this);       
        Destroy(this);
    }

    protected virtual void EffectTick()
    {
        if (EffectTime > 0 && EffectStartTime + EffectTime < Time.time)
        {
          
            bActivated = false;
            EffectEnd();
        }
    }

    public virtual void EffectReset(bool reapply)
    {
        if (reapply)
        {
            EffectApplied(owner);
        }
        else
        {
            EffectStartTime = Time.time;
        }
    }
	// Update is called once per frame
	void Update () {
       if(bActivated) EffectTick();
	}
}
