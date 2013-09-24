using UnityEngine;
using System.Collections;

public class Tools : MonoBehaviour {

	public static Vector3 ProjectVectorOntoPlane(Vector3 v, Vector3 normal){
		//Orthogonalprojektion auf Ebene der Normalen bei Erhaltung der Ausgangsmagnitüde
		return (v - Vector3.Dot(normal, v) * normal).normalized * v.magnitude;
	}
	
	public static Vector3 CameraVectorToObject(Vector3 vec, GameObject obj, AGCamera cam){

        Quaternion q = Quaternion.FromToRotation(cam.transform.forward, -obj.transform.up);
		return  q * vec;		
	}
	
	public static bool LayerCheck(LayerMask mask, GameObject obj){
		return true;
	}
	
	public static void RandomizeVector(ref Vector3 vector, float randomizeValue)
	{
		Vector3 temp = new Vector3(vector.x - Mathf.Clamp(Random.value - 0.5f, -randomizeValue, randomizeValue), 
									vector.y - Mathf.Clamp(Random.value - 0.5f, -randomizeValue, randomizeValue), 
									vector.z - Mathf.Clamp(Random.value - 0.5f, -randomizeValue, randomizeValue));
		
		vector = temp;
		
	}

    public static void SpawnObjectForPlayerCameras(GameObject objectToSpawn, Vector3 pos)
    {
        foreach (AGPlayerInfo info in AGGame.Instance.Players)
        {

            Quaternion rotation = info.Controller.AGCam.CameraRig.rotation;
            GameObject newObject = (GameObject)Instantiate(objectToSpawn, pos, rotation);
            newObject.GetComponentInChildren<LookToCamera>().Target = info.Controller.AGCam.CameraRig.transform;
            int newlayer = 8 + info.Controller.PlayerID;
            newObject.name = "CamObject for Player" + info.PlayerName;
            newObject.layer = newlayer;
            foreach (Transform child in newObject.transform.root)
            {
                child.gameObject.layer = newlayer;
            }
        }
    }

   public static IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
    }
    //public static ColorParticles(

   public static void CopyTransform(Transform srcTransForm, Transform dstTransform)
   {
       dstTransform.parent = srcTransForm.parent; 
       dstTransform.localPosition = srcTransForm.localPosition;
       dstTransform.localRotation = srcTransForm.localRotation;
       dstTransform.localScale = srcTransForm.localScale;
           
   }
	
}
