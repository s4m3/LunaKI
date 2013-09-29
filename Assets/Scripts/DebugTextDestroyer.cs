using UnityEngine;
using System.Collections;

public class DebugTextDestroyer : MonoBehaviour {

	void Awake () {
		StartCoroutine(DestroyAfterSeconds(7f));
	}
	
	//delete gameobject after n seconds
	IEnumerator DestroyAfterSeconds(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		Destroy(gameObject);
	}
}
