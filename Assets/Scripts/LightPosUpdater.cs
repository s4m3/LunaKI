using UnityEngine;
using System.Collections;
//[ExecuteInEditMode]
public class LightPosUpdater : MonoBehaviour {
    public Transform LightPos;
    Material material;

    void Start()
    {
        material = renderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        material.SetVector("_LightPos", LightPos.position);
        material.SetVector("_PlanetPos", transform.position);
      //  Debug.DrawRay(transform.position, material.GetVector("_LightDir")*20, Color.red);
    }
}
