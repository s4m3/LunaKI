using UnityEngine;
using System.Collections;
[System.Serializable]
public class AGStatSingle
{
    public float baseValue;
    protected float modifiedValue;

    [HideInInspector]
    public virtual float currentValue
    {
        get { return baseValue + modifiedValue; }
    }

    public virtual void InitStat()
    {
       modifiedValue = 0;
    }
    public void BuffStat(float val)
    {
        modifiedValue += val;
        
    }

    public virtual void ChangeStat(AGStatSingle newStat)
    {
        baseValue = newStat.baseValue;    
    }

    public virtual void ModifyByBasePercent(float percent)
    {
        BuffStat(percent * baseValue);
    }
}

[System.Serializable]
public class AGStat : AGStatSingle {

   public float max
    {
        get
        {
            return baseValue + modifiedValue;
        }       
    }
    new public float currentValue;
    public float startValue;

    public void  ModifyMaximumAbsolute(float val){
        modifiedValue += val;
        currentValue = Mathf.Clamp(currentValue, 0, max);
    }

    public void ModifyMaximumPercent(float percent)
    {
        modifiedValue += baseValue * percent;
        currentValue = Mathf.Clamp(currentValue, 0, max);
    }

    public override void InitStat()
    {
        currentValue = startValue;
    }
    public void MaximizeStat()
    {    
        currentValue = baseValue + modifiedValue;
        Debug.Log(currentValue);
    }

    public override void ChangeStat(AGStatSingle newStat)
    {
        baseValue = newStat.baseValue;
        currentValue = Mathf.Clamp(currentValue, 0, max);
    }
}
