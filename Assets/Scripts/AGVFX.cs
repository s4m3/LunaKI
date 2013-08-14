using UnityEngine;
using System.Collections;

public class AGVFX : MonoBehaviour {
    public SphereCollider CamShakeCollider;
    public AGCamera.CamShake Shake;
    public Projector Projector;
    TrailRenderer trail;

    public float EffectDuration;
    public Transform AttachPoint;
    ParticleSystem[] Particles;
    float StartTime;

    // Use this for initialization
	void Start () {
	//Check for Cam-ShakePurposeCollisions
        StartTime = Time.time;
        trail = GetComponent<TrailRenderer>();
	}
    public void SetPlayer(int id)
    {
        if (Projector != null)
        {
            AGPlayerInfo info = AGGame.Instance.Players[id - 1];
            Projector.material = info.PlayerProjectorMaterial;           
        }

        SetColor(AGGame.Instance.Players[id - 1].PlayerColor);

    }
    public void Stop()
    {
        if (Particles != null)
        {
            for (int i = 0; i < Particles.Length; i++)
            {
                Particles[i].Stop();
            }
        }
        
      gameObject.AddComponent<DeleteParticles>();
    }
    public void SetColor(Color pColor)
    {
        Particles = gameObject.GetComponentsInChildren<ParticleSystem>();
       
        for (int i = 0; i < Particles.Length; i++)
        {
            Particles[i].startColor = pColor;
        }
    }
	// Update is called once per frame
	void Update () {
        if (EffectDuration > 0 && Time.time > StartTime + EffectDuration)
        {
            EndEffects();
        }
	}

    void EndEffects()
    {
        Destroy(gameObject);
    }
}
