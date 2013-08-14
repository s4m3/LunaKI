using UnityEngine;
using System.Collections;

public class AGAction_Shot : AGAction
{

    public GameObject Projectile;

    public GameObject TravelEffectDream;
    public GameObject TravelEffectReal;
    public GameObject TravelEffectUltimate;

    public Material ProjectileMaterialReal;
    public Material ProjectileMaterialDream;
    public Material ProjectileMaterialUltimate;

    public AGVFX ExplosionEffectDream;
    public AGVFX ExplosionEffectReal;
    public AGVFX ExplosionEffectUltimate;

    public int Damage;
    public float ProjectileSpeed;

    public int MaxShots;
    public float ShotRestoreTime;


    float LastShotRestoreTime;

   
    public int ShotsLeft;
    private int StartShots;
    int DisplayCoolDown;

    public GameObject ShotDisplayPlaneTemplate;
    public float ShotDisplayDistance;
    GameObject ShotDisplayPlane;
    UVFrame ShotDisplayUVFrame;
     

    // Use this for initialization

    public override void SetOwner(AGPlayerController player)
    {
        base.SetOwner(player);
        Color playerColor = player.info.PlayerColor;
        playerColor =  new Color(playerColor.r + 0.5f, playerColor.g + 0.5f,playerColor.b + 0.5f);

        ProjectileMaterialDream = new Material(ProjectileMaterialDream);
        ProjectileMaterialDream.SetColor("_Color", playerColor);

        ProjectileMaterialReal = new Material(ProjectileMaterialReal);
        ProjectileMaterialReal.SetColor("_Color", playerColor);

        ProjectileMaterialUltimate = new Material(ProjectileMaterialUltimate);
        ProjectileMaterialUltimate.SetColor("_Color", playerColor);
        
        
        if (!ShotDisplayPlane) ShotDisplayPlane = (GameObject)Instantiate(ShotDisplayPlaneTemplate);
    }

    public void SetShotDisplay()
    {
       // print("SetShotDisplay");
        ShotDisplayPlane.gameObject.layer = 8 + owner.PlayerID;

        ShotDisplayPlane.transform.position = owner.pawn.MyProjector.transform.position;
        ShotDisplayPlane.transform.rotation = owner.pawn.MyProjector.transform.rotation;

        ShotDisplayUVFrame = ShotDisplayPlane.GetComponentInChildren<UVFrame>();
        ShotDisplayUVFrame.SetFrame(ShotsLeft);
        ShotDisplayUVFrame.gameObject.layer = 8 + owner.PlayerID;
        ShotDisplayUVFrame.gameObject.renderer.material.SetColor("_Color", owner.info.PlayerColor);
    }
    
    public void Update()
    {

        if (owner.pawn)
        {
            UpdateShotDisplayTransform();
            UpdateShots();
        }
    }

    protected void UpdateShotDisplayTransform()
    {
        ShotDisplayPlane.transform.position = owner.pawn.transform.position;
        Quaternion rotation = Quaternion.LookRotation(owner.pawn.SmoothSurfaceNormal, owner.AGCam.CameraRig.transform.up);
        ShotDisplayPlane.transform.rotation = rotation;
    }
    protected void UpdateShots()
    {
        if (Time.time > LastShotRestoreTime + ShotRestoreTime)
        {
            SetShots(ShotsLeft + 1);
            LastShotRestoreTime = Time.time;
        }
    }

    public override bool Activate ()
	{
		if (ShotsLeft > 0) {
			return base.Activate ();
		} else
			return false;
	}
	
 	protected override string getSound ()
	{
		return "shot";
	}
	
	protected override string getSoundState ()
	{
		return (owner.pawn.CurrentPlayerState == AGActor.LightState.Real?"day":"night");
	}
	
	protected override void StartAction()
    {
         
        base.StartAction();
       // Debug.Log("Shooting in state:" + owner.pawn.CurrentPlayerState);
        GameObject EffectToSpawn = null;
        Material mat = null;
        AGVFX ExplEffect = null;
        if (owner.Action_Ultimate.Ultimate && owner.Action_Ultimate.Ultimate.bActivated)
        {
            ExplEffect = ExplosionEffectUltimate;
            EffectToSpawn = TravelEffectUltimate;
            mat = ProjectileMaterialUltimate;
        } else
        {
        
            switch (owner.pawn.CurrentPlayerState)
            {

                case AGActor.LightState.Dream:
                    EffectToSpawn = TravelEffectDream;
                    mat = ProjectileMaterialDream;
                    ExplEffect = ExplosionEffectDream;
                    break;
                case AGActor.LightState.Real:
                    EffectToSpawn = TravelEffectReal;
                    mat = ProjectileMaterialReal;
                    ExplEffect = ExplosionEffectReal;
                    break;

            }
        }
        //   Debug.Log("Shooting since:" + ActionStartTime + " for " + (Time.time - ActionStartTime));
        GameObject ob = (GameObject)Instantiate(Projectile, owner.pawn.GetMuzzlePos(), Quaternion.LookRotation(owner.pawn.LookDirection, owner.pawn.transform.up));
        AGProjectile proj = ob.GetComponent<AGProjectile>();

        GameObject Effects = (GameObject)Instantiate(EffectToSpawn, proj.transform.position, proj.transform.rotation);        
        Effects.gameObject.GetComponentInChildren<AGVFX>().SetPlayer(owner.PlayerID);
        proj.DeathEffects = ExplEffect;
        proj.SetEffects(Effects, mat);

        //Pass Values from Skill (just to make the Balancing easier)
        proj.Damage = (int) owner.pawn.GetAttackValue(Damage);
        proj.Speed = ProjectileSpeed;

       

        owner.pawn.Shoot(proj);

        SetShots(ShotsLeft - 1);
    }
	
	public void DestroyShotDisplay()
	{
		if(ShotDisplayPlane) Destroy(ShotDisplayPlane.gameObject);
	}


    public void SetShots(int newShots)
    {
        if (newShots != ShotsLeft)
        {
            ShotsLeft = Mathf.Clamp(newShots, 0, MaxShots);
            if(ShotDisplayUVFrame) ShotDisplayUVFrame.SetFrame(ShotsLeft);
        }
    }
    public override void ReleaseButton()
    {
        base.ReleaseButton();
   
    }

    public override void PawnDied()
    {
        ShotsLeft = StartShots;
        Destroy(ShotDisplayPlane);
        base.PawnDied();
    }
}