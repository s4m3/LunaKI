using UnityEngine;
using System.Collections;

public class AGAction_Melee : AGAction {
   public float Damage;
   public float MovementReduction;
   public float Range;
   public float Angle;
   public float UserStunTime;
   AGEffect PushEffect;
   public float PushDuration;

   public AnimationCurve UserSlowCurve;
   public AGEffect_Push.PushData PushValues;
   public GameObject ColliderTemplate;

   public GameObject HitEffect;
   public GameObject MeleeEffectTemplate;
	
	protected override string getSound ()
	{
		return "melee_swing";	
	}

   protected override void StartAction()
   {
       if (bHoldButton) return;

       base.StartAction();
       //owner.pawn.SetVelocity(Vector3.zero );
      // Debug.Break();
       GameObject obj = (GameObject) Instantiate(ColliderTemplate, owner.pawn.transform.position, owner.pawn.transform.rotation);
       obj.GetComponent<AGAction_MeleeCollider>().SetInstigator(this);

       
       AGEffect_Stun stun = owner.pawn.gameObject.AddComponent<AGEffect_Stun>();
       stun.BlocksMovement = false;
       stun.BlocksLook = false;
       stun.BlocksAction = true;
       stun.StopMovements = false;
       stun.EffectTime = UserStunTime;


       AGEffect_ModifySpeed speedMod = owner.pawn.gameObject.AddComponent<AGEffect_ModifySpeed>();
       speedMod.SpeedModification = -MovementReduction;
       speedMod.EffectTime = UserStunTime;
       speedMod.curve = UserSlowCurve;

       owner.pawn.ApplyEffect(stun);
       owner.pawn.ApplyEffect(speedMod);

       //Debug.Break();
      
       GameObject blaWurst = (GameObject)Instantiate(MeleeEffectTemplate, owner.pawn.transform.position, owner.pawn.transform.rotation);
       
       blaWurst.transform.parent = owner.pawn.transform;
      // Debug.Break();
       blaWurst.GetComponentInChildren<Animation>().Play();
      // Debug.Break();
   }

   public void NotifyCollision(AGActor actor)
   {
       //Check Range
       //print("NotifyCollision distance" + (actor.transform.position - owner.pawn.transform.position).magnitude);
       if ((actor.transform.position - owner.pawn.transform.position).magnitude > Range) return;

       float _angle = 180 * (1- ((Vector3.Dot((actor.transform.position - owner.pawn.transform.position).normalized, owner.pawn.GetLookDirection() )+1)/2));
       //Check Angle
       //print(_angle);
       if ( _angle > Angle / 2) return;
      // print(actor.name + " in attack Cone");
      actor.m_AudioWeapon = "melee";
       actor.TakeDamage( owner.pawn.GetAttackValue(Damage), owner.pawn);

       Tools.SpawnObjectForPlayerCameras(HitEffect, actor.transform.position);       

       if (actor is AGPawn)
       {          
           AGEffect_Push push = actor.gameObject.AddComponent<AGEffect_Push>();
          
           Vector3 pushDir = (actor.transform.position - owner.pawn.transform.position).normalized;        
           PushValues.PushStartDir = pushDir;
           PushValues.Duration = PushDuration;
           push.EffectTime = PushDuration;
           actor.ApplyEffect(push);
           push.InitPush(PushValues);

           AGEffect_Stun Stun = actor.gameObject.AddComponent<AGEffect_Stun>();
           Stun.BlocksAction = true;
           Stun.BlocksLook = true;
           Stun.EffectTime = PushDuration;
           Stun.StopMovements = false;
           actor.ApplyEffect(Stun);
       }
   }
    // Use this for initialization
    
}
