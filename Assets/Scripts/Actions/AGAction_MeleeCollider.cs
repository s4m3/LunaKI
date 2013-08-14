using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AGAction_MeleeCollider : MonoBehaviour {
    public List<GameObject> CollidedActors;
    int spawnFrame;

    public AGAction_Melee InstigatorAction;
	// Use this for initialization
	void Awake () {
        spawnFrame = Time.frameCount;
      
	}
    public void SetInstigator(AGAction_Melee m)
    {
      //  Debug.Log("SetInstigator");
        InstigatorAction = m;
        transform.parent = m.owner.pawn.transform;
        collider.enabled = true;
    
    }
	// Update is called once per frame
	public void OnTriggerStay(Collider col){
       
        if (col.gameObject == InstigatorAction.owner.pawn.gameObject) return;
        if (CollidedActors.Contains(col.gameObject)) return;

        CollidedActors.Add(col.gameObject);
        //print("TriggerEnter "+col.gameObject.name);
        AGActor actor = col.GetComponent<AGActor>();

        if (actor)
        {
            InstigatorAction.NotifyCollision(actor);
        }
    }

    public void Update()
    {
        if (Time.frameCount > spawnFrame + 5)
        {
          //  print("destroy melee sphere");
            Destroy(this.gameObject);
        }
    }
}
