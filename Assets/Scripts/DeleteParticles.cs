using UnityEngine;
using System.Collections;



public class DeleteParticles : MonoBehaviour {
    
    float CheckFrequency = 0.5f;
    float lastCheck;
   public ParticleSystem[] Particles;

	// Use this for initialization
	void Start () {
        lastCheck = Time.time;
        Particles  = GetComponentsInChildren<ParticleSystem>();
        if (Particles == null) Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time > lastCheck + CheckFrequency)
        {
            int count = 0;

            for (int i = 0; i < Particles.Length; i++)
            {
                count += Particles[i].particleCount;
            }
            
            if (count <= 0)
            {
                Destroy(gameObject);
                return;
            
            }
            
            lastCheck = Time.time;
        }
	}
}
