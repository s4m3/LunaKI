using UnityEngine;
using System.Collections;
//
//public class MainMenu : MonoBehaviour {
//
//	public Vector2 ButtonPosition;
//	public int ButtonOffset;
//	private bool joystickButtonDown = false;
//	private int selectedButtonIndex = 0;
//	private bool upClick = false;
//	private bool downClick = false;
//	public GUIStyle buttonStyle;
//	public Texture2D backgroundTexture;
//	private float input;
//
//	public enum GameMode {None, Singleplayer, Multiplayer}
//	public GameMode gameMode;
//	public bool isMuted = false;
//	
//	//temp
//	private string[] buttonTexts;
//	
//	void Awake () {
//		gameMode = GameMode.None;
//		//temp
//		buttonTexts = new string[] {"Start Multiplayer Game", "Start Singleplayer Game", "Mute Music", "Quit Game"};
//	}
//	
//	void Start () {
//	
//	}
//	
//	void Update () {
//		
//		if (Input.GetButton("FireA_p1") || Input.GetButton("FireA_p2"))
//		{
//			activateMenuItem();
//		}
//		
//		input = Mathf.Abs(Input.GetAxis("Vertical_p1")) >= Mathf.Abs(Input.GetAxis("Vertical_p2")) ? Input.GetAxis("Vertical_p1") : Input.GetAxis("Vertical_p2");
//		if((input == 1 && !upClick) || (input == -1 && !downClick)) 
//		{
//			print (input);
//			StartCoroutine(changeMenuItem(-1*input));
//		}
//		
//	}
//	
//	private void activateMenuItem()
//	{
//		switch(selectedButtonIndex)
//		{
//		case 0:
//			gameMode = GameMode.Multiplayer;
//			break;
//		case 1:
//			gameMode = GameMode.Singleplayer;
//			break;
//		case 2:
//			isMuted = !isMuted;
//			break;
//		case 3:
//			Application.Quit();
//			break;
//		default:
//			gameMode = GameMode.None;
//			break;
//		}
//	}
//	
//	private IEnumerator changeMenuItem(float input)
//	{
//		upClick = input == -1;
//		downClick = input == 1;
//		//canChangeMenuItem = false;
//		//TODO: change selected menu item
//		selectedButtonIndex += (int)input;
//		if(selectedButtonIndex > 3)
//			selectedButtonIndex = 0;
//		if(selectedButtonIndex < 0)
//			selectedButtonIndex = 3;
//		//to prevent the change to happen multiple times per frame, there has to be waiting time
//		yield return new WaitForSeconds(0.1f);
//		upClick = downClick = false;
//		//canChangeMenuItem = true;
//
//	}
//	
//	void OnGUI ()
//	{
//		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), backgroundTexture);
//
//		if(GUI.Button (new Rect(ButtonPosition.x, ButtonPosition.y, 300, 80), buttonTexts[0]) || (joystickButtonDown && selectedButtonIndex == 0))
//		{
//			//Start Multiplayer
//			gameMode = GameMode.Multiplayer;
//		}
//		if(GUI.Button (new Rect(ButtonPosition.x, ButtonPosition.y + ButtonOffset + 80, 300, 80), buttonTexts[1]) || (joystickButtonDown && selectedButtonIndex == 1))
//		{
//			//Start Singleplayer
//			gameMode = GameMode.Singleplayer;
//		}
//		if(GUI.Button (new Rect(ButtonPosition.x, ButtonPosition.y + ButtonOffset*2 + 160, 300, 80), buttonTexts[2]) || (joystickButtonDown && selectedButtonIndex == 2))
//		{
//			//Mute Music
//			isMuted = !isMuted;
//		}
//		if(GUI.Button (new Rect(ButtonPosition.x, ButtonPosition.y + ButtonOffset*3 + 240, 300, 80), buttonTexts[3]) || (joystickButtonDown && selectedButtonIndex == 3))
//		{
//			//Quit Game
//			Application.Quit();
//		}
//	}
//	
//}

//////////////////////////////////////

public class MainMenu : MonoBehaviour {

	public Vector2 LogoOffset;

	public GUIStyle buttonStyle;

	public Texture2D backgroundTexture;
	public Texture2D credits;
	public Texture2D creditBackground;
	public Texture2D logo;
	public Texture muteButtonTexture;
	private Vector2 creditSize;
	public enum GameMode {None, Multiplayer, Singleplayer}
	public GameMode gameMode;
	public bool isMuted;
	
	private Vector2 screenRes;//TODO GUI-Manager zum speichern solcher werte
	private bool creditsVisible;
	void Awake () {
		gameMode = GameMode.None;
		//if(CreditsXPos == 0) CreditsXPos = Screen.width - credits.width;
		screenRes = AGGame.Instance.guiManager.ScreenResolution;
		creditSize = AGGame.Instance.guiManager.ResizeTexture(credits);
		creditsVisible = false;
	}
	


	private void showCredits()
	{
		//Credits
		GUI.DrawTexture(new Rect (screenRes.x * 0.7f, 0, screenRes.x * 0.3f, screenRes.y), creditBackground);
		GUI.DrawTexture(new Rect (screenRes.x * 0.7f, 0, creditSize.x, creditSize.y), credits);
	}
	
	void OnGUI ()
	{
		//GUI.matrix = Matrix4x4.TRS( Vector3.zero, Quaternion.identity, new Vector3( Screen.width / 1024.0f, Screen.height / 768.0f, 1.0f ) );
		GUI.DrawTexture (new Rect (0, 0, screenRes.x, screenRes.y), backgroundTexture);
		GUI.DrawTexture (new Rect (screenRes.x/2 - logo.width/2 + LogoOffset.x, LogoOffset.y, logo.width, logo.height), logo);

		GUILayout.BeginArea(new Rect(0,screenRes.y/2, screenRes.x, screenRes.y/2));
		
		GUILayout.BeginVertical(buttonStyle);
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		if(GUILayout.Button ("Start Multiplayer Game", buttonStyle))
		{
		//Start Multiplayer
			gameMode = GameMode.Multiplayer;
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		if(GUILayout.Button ("Start Singleplayer Game", buttonStyle))
		{
		//Start Singleplayer
			gameMode = GameMode.Singleplayer;
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		if(GUILayout.Button ("Credits", buttonStyle))
		{
			creditsVisible = !creditsVisible;
		}
				
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		if(GUILayout.Button ("Quit", buttonStyle))
		{
			//Quit Game
			Application.Quit();	
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		string buttontext = isMuted ? "Sound On" : "Sound Off";
		if(GUILayout.Button (buttontext, buttonStyle))
		{
			//Mute&Unmute
			isMuted = !isMuted;
		}
				
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
		GUILayout.Space(20);
		GUILayout.EndVertical();
		GUILayout.EndArea();
		
		if(creditsVisible) showCredits();
	}
	
}

[System.Serializable]
public class ButtonGroup
{
	public Texture selected;
	public Texture unSelected;
}

