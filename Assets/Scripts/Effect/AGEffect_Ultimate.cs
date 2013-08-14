using UnityEngine;
using System.Collections;

public class AGEffect_Ultimate : AGEffect_Pawn {

    public AnimationCurve StartSpeedModCurve;
    public AnimationCurve ScalerCurve;
    public float ScaleFactor;

    public int HealthBonus;
    public int AttackBonus;
    public int DefenseBonus;
    public float SpeedBonus;
    public float InvulnerableTime;
    

    public float StunTime;
    public bool CanMove;
    public bool CanLook;

    private AGEffect_Scale lastScaler;  
    private AGEffect_ModifySpeed lastSpeedMod;
    private AGEffect_Invulnerable lastInv;
    private AGEffect_Stun lastStun;

    protected override void EffectTick()
    {
       
        base.EffectTick();
    }
    public override bool EffectApplied(AGActor _owner)
    {
        bool block = BlocksAction;
        BlocksAction = false;
        if (base.EffectApplied(_owner))
        {
            Debug.Log("APPLY ULTIMATE");

            GameObject obj = new GameObject();
            obj.name = "KartoffelSalat";
            //Make Immovable
            AGEffect_Stun newStun = pawnOwner.gameObject.AddComponent<AGEffect_Stun>();
            newStun.EffectTime = StunTime;
            newStun.BlocksAction = block;
            newStun.BlocksLook = !CanLook;
            newStun.BlocksMovement = !CanMove;
            pawnOwner.ApplyEffect(newStun);
            lastStun = newStun;

            //Make Invulnerable
            AGEffect_Invulnerable newInv = pawnOwner.gameObject.AddComponent<AGEffect_Invulnerable>();
            newInv.EffectTime = StunTime;
            pawnOwner.ApplyEffect(newInv);
            lastInv = newInv;

            //Apply MoveSpeed effect (slow down)
       //     AGEffect_ModifySpeed newSpeedMod = pawnOwner.gameObject.AddComponent<AGEffect_ModifySpeed>();
         //   newSpeedMod.curve = StartSpeedModCurve;
          //  newSpeedMod.EffectTime = StunTime;
          //  pawnOwner.ApplyEffect(newSpeedMod);
           // lastSpeedMod = newSpeedMod;

            //Apply Scaler
            AGEffect_Scale newScale = pawnOwner.gameObject.AddComponent<AGEffect_Scale>();
            newScale.curve = ScalerCurve;
            newScale.ScaleFactor = ScaleFactor;
            newScale.EffectTime = EffectTime;
            pawnOwner.ApplyEffect(newScale);
            lastScaler = newScale;

            //Modify Stats
            pawnOwner.Speed.BuffStat(SpeedBonus);
            pawnOwner.Attack.BuffStat(AttackBonus);
            pawnOwner.Defense.BuffStat(DefenseBonus);
            pawnOwner.ModifyHealth(HealthBonus);

            pawnOwner.Player.Action_Shot.SetShots(30);
            return true;
        }
        return false;
    }
    public override void EffectEnd()
    {
        //Reset Stats
        Debug.Log("ULTIMATE END");
        pawnOwner.Speed.BuffStat(-SpeedBonus);
        pawnOwner.Attack.BuffStat(-AttackBonus);
        pawnOwner.Defense.BuffStat(-DefenseBonus);

        if (lastScaler && lastScaler.bActivated) lastScaler.EffectEnd();
        if (lastSpeedMod && lastSpeedMod.bActivated) lastSpeedMod.EffectEnd();
        if (lastInv && lastInv) lastInv.EffectEnd();
        if (lastStun && lastStun.bActivated) lastStun.EffectEnd();

        base.EffectEnd();
    }
}
