using UnityEngine;
using System.Collections;

public class AGProjectile : AGMoving {
    [HideInInspector]
    public float Speed;
	public float MaximumLifeTime;
	public LayerMask ReflectWith;
  //  public ParticleSystem Particles;
    protected AGVFX vx;
    public GameObject TravelEffects;

	public bool FriendlyFire;
	
	public int Damage;
	[HideInInspector]
	public AGPlayerController Instigator;

    protected bool CanExplode = true;

    public void Awake()
    {
       // vx = gameObject.GetComponentInChildren<AGVFX>();
      //  vx.gameObject.name = "VX";
    }
	protected override void Start ()
	{
		base.Start ();
        
		StateChangeEnabled = false;
 //       Particles = gameObject.GetComponentInChildren<ParticleSystem>();
   //     Particles.Play(true);

	}

    public void SetEffects(GameObject tEffects, Material mat)
    {
       vx = (AGVFX) tEffects.GetComponentInChildren<AGVFX>();
       vx.gameObject.transform.parent = this.transform;
       if (mat != null)
       {
           vx.gameObject.GetComponentInChildren<MeshRenderer>().material = mat;
       }
    }
	public void Reflect(Vector3 HitNormal){
        Vector3 newDir =  Vector3.Reflect(TargetVelocity, HitNormal);
        TargetVelocity = newDir;
        Velocity = newDir;
        LookDirection = newDir;     
	}
	
	public void InitProjectile (AGPlayerController player, Vector3 dir)
	{
   		
		Instigator = player;
		Velocity = dir.normalized*Speed;
		TargetVelocity = Velocity;
		LookDirection = Velocity;
        MyProjector.material = player.info.PlayerProjectorMaterial;
  
		InitActor (player.pawn);
	}
	
	protected override void FixedUpdate ()
	{
		base.FixedUpdate ();
		if(SpawnTime + MaximumLifeTime <= Time.fixedTime){
			Explode();
		}
	}
    protected override void SpawnDeathEffects()
    {
        GameObject go = (GameObject) Instantiate(DeathEffects.gameObject, transform.position, transform.rotation);
        go.GetComponent<AGVFX>().SetPlayer(Instigator.PlayerID);
    }
	protected virtual void Explode(){
        CanExplode = false;
      
        GameObject _mesh = gameObject.GetComponentInChildren<MeshRenderer>().gameObject;
        if (_mesh != null) Destroy(_mesh);

        if(vx != null) vx.Stop();
       
        transform.DetachChildren();			
          
        if(MyProjector) Destroy(MyProjector.gameObject);
           
        Die();
	}
	public void OnTriggerEnter(Collider col){
		AGActor actor;
		//print ("Triggerenter "+col.gameObject.name);
		if(AGGame.CanHitTarget(col.gameObject, this, out actor)){
			if(actor != null){ 
				actor.m_AudioWeapon = "shot";
				actor.TakeDamage(Damage, this);
			}
		    if(CanExplode)	Explode();           
		}		
	}
}
