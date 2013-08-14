using UnityEngine;
using System.Collections;

public class UVFrame : MonoBehaviour {
    public int rows;
    public int cols;
    public int StartFrame;


    public Vector2 Scale;
	// Use this for initialization
	void Awake () {
        Scale.x = 1.0f / cols;
        Scale.y = 1.0f / rows;
        Material mat = new Material(renderer.material);
        renderer.material = mat;
        mat.SetTextureScale("_MainTex", Scale);
        SetFrame(StartFrame);
	}
	
	// Update is called once per frame
    public void SetFrame(int frame)
    {
        int targetRow = ((int)(frame / cols));
        int targetCol = frame % cols;    
 
        renderer.material.SetTextureOffset("_MainTex", new Vector2(Scale.x * (float) targetCol, 1 - ((Scale.y * (float) targetRow)) - Scale.y));
              
    }
}
