using UnityEngine;
using System.Collections;

public class AGEffect_Push : AGEffect_Pawn {
    [System.Serializable]
    public class PushData
    {
        public float PushSpeed;
        public float PushSpeedAcceleration;
        public float SmoothOutTime;
     
        [HideInInspector]
        public Vector3 PushStartDir;
        [HideInInspector]
        public float Duration;
    }
    protected PushData currentPush;
    protected Vector3 CurrentPushVelocity;


    protected float currentPushSpeed;

    public void InitPush(PushData data)
    {
        
        currentPush = data;      
        currentPushSpeed = currentPush.PushSpeed;
        CurrentPushVelocity = currentPush.PushStartDir.normalized * currentPushSpeed;    
 
    }
    public void SetPushDirection(Vector3 newDir)
    {
        CurrentPushVelocity = newDir;
    }
    protected override void EffectTick()
    {
        base.EffectTick();
        Debug.DrawRay(pawnOwner.transform.position, CurrentPushVelocity, Color.cyan);

        //Time in smoothzone = time dashing - (duration - zone)
        float TimeInSmoothZone = (Time.time - EffectStartTime) - (currentPush.Duration - currentPush.SmoothOutTime);

        if (TimeInSmoothZone > 0)
        {
            //Calculate current maxspeed of pawn without the modification of the dash itself to reduce the speeed when in the smooth-zone
           // float NormalVelocity = pawnOwner.currentVelocityMaximum - (((currentPush.PushSpeed / pawnOwner.VelocityMaximum) - 1) * pawnOwner.VelocityMaximum);
            //The more we are in the zone, to more our currentDashspeed moves towards the "normal" velocity
            float SpeedWeight = Mathf.Clamp(TimeInSmoothZone / currentPush.SmoothOutTime, 0, 1);
            currentPushSpeed = Mathf.Lerp(currentPushSpeed, pawnOwner.Speed.currentValue, SpeedWeight);

            // print("Smoothing out dash // normal Velocty: " + NormalVelocity + "   weight: " + SpeedWeight + " speed:" + currentDashSpeed);
        }


        Vector3 UpdatedDashVelocty = Tools.ProjectVectorOntoPlane(CurrentPushVelocity, pawnOwner.LocalGroundNormal);
        Vector3 TargetVelocity = UpdatedDashVelocty.normalized * currentPushSpeed;
        CurrentPushVelocity = Vector3.Lerp(
                            UpdatedDashVelocty,
                            TargetVelocity,
                            Time.deltaTime * currentPush.PushSpeedAcceleration
                         );
       // Debug.Log(currentPushSpeed);
        pawnOwner.SetVelocity(CurrentPushVelocity);

    }

    public override void EffectEnd()
    {
    
        if (pawnOwner != null)
        {
                    
        }
        base.EffectEnd();
    }

    public override bool EffectApplied(AGActor _owner)
    {
      
       bool b = base.EffectApplied(_owner);
       if (b)
       {
           pawnOwner.Push(this);
       }
       return b;
    }
 
}
