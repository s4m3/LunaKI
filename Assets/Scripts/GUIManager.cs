using UnityEngine;
using System.Collections;

public class GUIManager : Object {

	public Vector2 ScreenResolution;
	public Vector2 DefaultScreenResolution;
	
	public Vector2 ResizeTexture(Texture texture)
	{
		Vector2 size = new Vector2(texture.width, texture.height);
		Vector2 diff = DefaultScreenResolution - ScreenResolution;
		if(Mathf.Abs (diff.x) > Mathf.Abs(diff.y))
			size *= ScreenResolution.x / DefaultScreenResolution.x;
		else
			size *= ScreenResolution.y / DefaultScreenResolution.y;
		
		return size;
	}
	
}
