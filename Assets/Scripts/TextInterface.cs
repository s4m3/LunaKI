using UnityEngine;
using System.Collections;

public class TextInterface : MonoBehaviour {
	public GUIStyle guiStyle;
	private string text;
	private bool printing;
	private bool fadeOut;
	private float startTime;
	private float secondsToRun;

	void Start () {
		guiStyle.fontSize = 250;
	}
	
	private IEnumerator startTimer(float seconds) 
	{
        yield return new WaitForSeconds(seconds);
        printing = false;
	}
	
	public void printToScreen(string text, float seconds, bool fadeOut) 
	{
		this.fadeOut = fadeOut;
		this.startTime = Time.time;
		this.text = text;
		this.secondsToRun = seconds;
		printing = true;
		StartCoroutine(startTimer(seconds));
	}
	
	void OnGUI() {
		if(fadeOut)
		{
			float alpha = Mathf.Lerp(1, 0, (Time.time - startTime)/secondsToRun);
			guiStyle.normal.textColor = guiStyle.normal.textColor = new Color(guiStyle.normal.textColor.r, guiStyle.normal.textColor.g, guiStyle.normal.textColor.b, alpha);
		}
		if(printing) GUI.Label(new Rect(0, 0, Screen.width, Screen.height), text, guiStyle);
	}
}
