using UnityEngine;
using System.Collections;

public class ActorStateParticles : MonoBehaviour {
    public AGActor.LightState activeIn;
    private ParticleSystem[] particles;

    void Start()
    {
        ((AGActor)transform.parent.gameObject.GetComponent<AGActor>()).onStateChange += OnStateChange;
        particles = gameObject.GetComponentsInChildren<ParticleSystem>();
        OnStateChange(((AGActor)transform.parent.gameObject.GetComponent<AGActor>()).currentLightState);
    }

    void OnStateChange(AGActor.LightState state)
    {
        if (activeIn != AGActor.LightState.Between && state == AGActor.LightState.Between) return;
        if (particles == null)
        {
            Debug.Log("ActorStateParticle: O PARTICLE SYSTEM ATACHED TO " + name);
            return;
        }
        if (state == activeIn)
        {
            SetParticles(true);
        }
        else
        {
            SetParticles(false);
        }
    }
    private void SetParticles(bool on)
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].enableEmission = on;
        }
    }

}
