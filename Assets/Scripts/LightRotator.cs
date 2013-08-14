using UnityEngine;
using System.Collections;

public class LightRotator : MonoBehaviour {

    public Transform CenterOfRotation;
    public float speed;
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(CenterOfRotation.position);
        transform.RotateAround(CenterOfRotation.position, CenterOfRotation.up, speed * Time.deltaTime);
    }
}
