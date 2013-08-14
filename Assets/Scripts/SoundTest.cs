using UnityEngine;
using System.Collections;




public class SoundTest : MonoBehaviour {
	public AudioClip acl;
	public AudioClip acr;
	public AudioClip aclr;
	public AudioSource asl;
	public AudioSource asr;
	public AudioListener al;
	
	public GameObject go1;
	public GameObject go2;
	
		
	// Use this for initialization
	void Start ()
	{
		
		asl.clip = acl;
		asr.clip = acr;
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	public void PlayAtPos (Transform pos, AudioSource source, AudioClip clip)
	{
		source.transform.position = pos.position - transform.position;
		source.PlayOneShot(clip);
	}
	
	void FixedUpdate ()
	{
		if (Input.GetKey (KeyCode.A)) {
//			asl.clip = acl;
//			asl.PlayOneShot (acl);
			PlayAtPos (go1.transform, asl, acl);
			
		}
		PlayAtPos (go1.transform, asl, acl);
		if (Input.GetKey (KeyCode.D)) {
//			asr.clip = acr;
//			asr.PlayOneShot (acr);
			PlayAtPos (go2.transform, asr, acr);
		}
		
		if (Input.GetKey (KeyCode.W)) {
			PlayAtPos (go1.transform, asl, aclr);
//			asl.clip = aclr;
//			asl.PlayOneShot (aclr);
		}
		
	}
	
}
