using UnityEngine;
using System.Collections;

public class ProjectileTestManager : MonoBehaviour {
    public GameObject p1;
    public GameObject p2;
    public GameObject p3;
    public Color pColor;

    ProjectileTestCamera ptestCam;

    GameObject currentP;
	// Use this for initialization
	void Start () {
        ptestCam = (ProjectileTestCamera) GameObject.FindObjectOfType(typeof(ProjectileTestCamera));
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Mouse0)){
            Destroy(currentP);
            currentP  = (GameObject)  Instantiate(p1, this.transform.position, this.transform.rotation);        
            ptestCam.SetProjectile(currentP.transform);

            ParticleSystem[] Particles = currentP.gameObject.GetComponentsInChildren<ParticleSystem>();
            Debug.Log(currentP.gameObject.GetComponentInChildren<MeshRenderer>().gameObject.name);
            currentP.gameObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", new Color(pColor.r + 0.5f, pColor.g + 0.5f,pColor.b + 0.5f));
            for (int i = 0; i < Particles.Length; i++)
            {
                Particles[i].startColor = pColor;
            }

        } else if(Input.GetKeyDown(KeyCode.Mouse1)){
            Destroy(currentP);
            currentP  = (GameObject)  Instantiate(p2, this.transform.position, this.transform.rotation);       
            ptestCam.SetProjectile(currentP.transform);

            ParticleSystem[] Particles = currentP.gameObject.GetComponentsInChildren<ParticleSystem>();
            currentP.gameObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", new Color(pColor.r + 0.5f, pColor.g + 0.5f, pColor.b + 0.5f));
            for (int i = 0; i < Particles.Length; i++)
            {
                Particles[i].startColor = pColor;
            }
        } else if(Input.GetKeyDown(KeyCode.Mouse2)){
            Destroy(currentP);
            currentP = (GameObject)Instantiate(p3, this.transform.position, this.transform.rotation);          
            ptestCam.SetProjectile(currentP.transform);

            ParticleSystem[] Particles = currentP.gameObject.GetComponentsInChildren<ParticleSystem>();
            currentP.gameObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", new Color(pColor.r + 0.5f, pColor.g + 0.5f, pColor.b + 0.5f));
            for (int i = 0; i < Particles.Length; i++)
            {
                Particles[i].startColor = pColor;
            }
        }
	}
}
