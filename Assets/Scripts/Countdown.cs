using UnityEngine;
using System.Collections;

public class Countdown : MonoBehaviour {
	private bool countdownActive = false;
	public int countdownSeconds = 5;
	public GUIStyle guiStyle;
	private float startTime;
	private int _restSeconds;
	private float restSecondsF;
	
	public int RestSeconds {
		get { return _restSeconds; }
	}
	
	private bool m_gotQueried = false;
	public bool isStartedOnce ()
	{
		if (m_gotQueried == true || countdownActive == false) {      
			return false;
		}
		
		m_gotQueried = true;
		return true;
	}
	
	
	void Start(){
		guiStyle.fontSize = 200;
	}
	
	// Update is called once per frame
	void Update () {
		if(countdownActive)
		{
			restSecondsF = countdownSeconds - (Time.time - startTime);
			_restSeconds = Mathf.RoundToInt(restSecondsF);
		}
		
	}
	
	public void StartCountdown ()
	{
		//print("start countdown");
		startTime = Time.time;
		countdownActive = true;
		m_gotQueried = false;
	}

	
	void OnGUI()
	{
		float aThirdWidth = Screen.width / 3;
		float aThirdHeight = Screen.height / 3;
		if(RestSeconds >= 1)
		{
//			guiStyle.normal.textColor = new Color(guiStyle.normal.textColor.r, guiStyle.normal.textColor.g, guiStyle.normal.textColor.b, 1);
			GUI.Label(new Rect(Screen.width / 2 - aThirdWidth, Screen.height/2 - aThirdHeight, 2*aThirdWidth, 2*aThirdHeight), RestSeconds.ToString(), guiStyle);
		}
//		else if(RestSeconds <= 1 && countdownActive)
//		{
//			float alpha = Mathf.Lerp(1, 2, restSecondsF/4);
//			guiStyle.normal.textColor = new Color(guiStyle.normal.textColor.r, guiStyle.normal.textColor.g, guiStyle.normal.textColor.b, alpha-1);
//			guiStyle.fontSize = Mathf.RoundToInt(guiStyle.fontSize + (2/alpha * 5));
//			GUI.Label(new Rect(Screen.width / 2 - aThirdWidth, Screen.height/2 - aThirdHeight, 2*aThirdWidth, 2*aThirdHeight), "FIGHT!", guiStyle);
//		}
	}
}
