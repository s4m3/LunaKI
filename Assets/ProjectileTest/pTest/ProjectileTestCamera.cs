using UnityEngine;
using System.Collections;

public class ProjectileTestCamera : MonoBehaviour {
    public Transform parentTarget;

    public void SetProjectile(Transform target){
        parentTarget.parent = target;
        parentTarget.localPosition = Vector3.zero;
    }
}
