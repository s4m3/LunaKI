using UnityEngine;
using System.Collections;

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
	private bool canChangeMenuItem = true;
	private bool canChangeDifficulty = true;
	private int selectedButtonIndex = 0;
	private float aiSkill = 10f;
	private bool debugPathfinding = false;
	private bool useSplitScreen = true;
	
	private string[] menuOptions = new string[5];
	
	private Vector2 screenRes;//TODO GUI-Manager zum speichern solcher werte
	private bool creditsVisible;
	
	public float Difficulty()
	{
		return aiSkill / 10.0f;
	}
	
	public bool isDebugMode()
	{
		return debugPathfinding;
	}
	
	public bool UseSplitScreen()
	{
		return useSplitScreen;
	}
	
	void Awake () {
		menuOptions[0] = "Multiplayer";
		menuOptions[1] = "Singleplayer";
		menuOptions[2] = "Credits";
		menuOptions[3] = "Music";
		menuOptions[4] = "Quit";
		
		gameMode = GameMode.None;
		//if(CreditsXPos == 0) CreditsXPos = Screen.width - credits.width;
		screenRes = AGGame.Instance.guiManager.ScreenResolution;
		creditSize = AGGame.Instance.guiManager.ResizeTexture(credits);
		creditsVisible = false;
		isMuted = false;
	}
	
	void Update () {
		
		if (Input.GetButton("FireA_p1") || Input.GetButton("FireA_p2"))
		{
			if(canChangeMenuItem) StartCoroutine(activateMenuItem());
		}
		
		float input = Mathf.Abs(Input.GetAxis("Vertical_p1")) >= Mathf.Abs(Input.GetAxis("Vertical_p2")) ? Input.GetAxis("Vertical_p1") : Input.GetAxis("Vertical_p2");
		input = Mathf.Round(input);
		if((input == 1 || input == -1) && canChangeMenuItem) 
		{
			StartCoroutine(changeMenuItem(-1*input));
		}
		
		if(selectedButtonIndex != 1) return;
		//otherwise check for change for slider
		float inputHorizontal = Mathf.Abs(Input.GetAxis("Horizontal_p1")) >= Mathf.Abs(Input.GetAxis("Horizontal_p2")) ? Input.GetAxis("Horizontal_p1") : Input.GetAxis("Horizontal_p2");
		inputHorizontal = Mathf.Round(inputHorizontal);
		if(Mathf.Abs(inputHorizontal) == 1)
		{
			StartCoroutine(changeDifficultySlider(inputHorizontal));
		}
		
	}
	
	private IEnumerator activateMenuItem()
	{
		canChangeMenuItem = false;
		switch(selectedButtonIndex)
		{
		case 0:
			gameMode = GameMode.Multiplayer;
			break;
		case 1:
			gameMode = GameMode.Singleplayer;
			break;
		case 2:
			creditsVisible = !creditsVisible;
			break;
		case 3:
			isMuted = !isMuted;
			AGGame.Instance.Un_Mute(isMuted);
			break;
		case 4:
			Application.Quit();
			break;
		default:
			gameMode = GameMode.None;
			break;
		}
		yield return new WaitForSeconds(0.4f);
		canChangeMenuItem = true;
	}
	
	private IEnumerator changeMenuItem(float input)
	{
		if(!canChangeMenuItem) yield break;
		canChangeMenuItem = false;
		
		selectedButtonIndex += (int)input;
		if(selectedButtonIndex > menuOptions.Length -1)
			selectedButtonIndex = 0;
		if(selectedButtonIndex < 0)
			selectedButtonIndex = menuOptions.Length -1;
		
		Debug.Log(selectedButtonIndex);
		//to prevent the change to happen multiple times per frame, there has to be waiting time
		yield return new WaitForSeconds(0.3f);
		canChangeMenuItem = true;

	}
	
	private IEnumerator changeDifficultySlider(float input)
	{
		if(!canChangeDifficulty) yield break;
		canChangeDifficulty = false;
		float difficulty = aiSkill + input;
		aiSkill = Mathf.Clamp(difficulty, 1.0f, 10.0f);
		yield return new WaitForSeconds(0.2f);
		canChangeDifficulty = true;
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
		GUI.SetNextControlName(menuOptions[0]);
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
		
		GUI.SetNextControlName(menuOptions[1]);
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
		
		GUI.SetNextControlName(menuOptions[2]);
		if(GUILayout.Button ("Credits", buttonStyle))
		{
			creditsVisible = !creditsVisible;
		}
				
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		GUI.SetNextControlName(menuOptions[3]);
		string buttontext = isMuted ? "Music is Off" : "Music is On";
		if(GUILayout.Button (buttontext, buttonStyle))
		{
			//Mute&Unmute
			isMuted = !isMuted;
			AGGame.Instance.Un_Mute(isMuted);
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		GUI.SetNextControlName(menuOptions[4]);
		if(GUILayout.Button ("Quit", buttonStyle))
		{
			//Quit Game
			Application.Quit();	
		}
				
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
		GUILayout.Space(20);
		GUILayout.EndVertical();
		GUILayout.EndArea();
		
		if(creditsVisible) showCredits();
		
		if(selectedButtonIndex == 1)
		{
			GUI.BeginGroup(new Rect (screenRes.x * 0.1f, screenRes.y * 0.5f, 400, 500));
			string difficultyText = aiSkill < 2f ? "very easy" : aiSkill < 3f ? "easy" : aiSkill < 5.5f ? "normal" : aiSkill < 7f ? "hard" : aiSkill < 8.5f ? "very hard" : "expert";
			
			GUI.Label(new Rect(0,0,200,80), "Difficulty: ", buttonStyle);
			GUI.Label(new Rect(150,0,200,80), difficultyText, buttonStyle);
			GUI.Label(new Rect(0,50,200,80), "Value: ", buttonStyle);
			GUI.Label(new Rect(150,50,200,80), Mathf.RoundToInt(aiSkill).ToString(), buttonStyle);
			aiSkill = GUI.HorizontalSlider(new Rect (0, 130, 400, 80), aiSkill, 1f, 10.0f);
			GUI.EndGroup();
			
			//DEBUG MODE AND SPLIT SCREEN TOGGLE BUTTON:
			useSplitScreen = GUI.Toggle(new Rect (screenRes.x - 200, screenRes.y - 150, 180,50), useSplitScreen, "Split Screen");
			debugPathfinding = GUI.Toggle (new Rect (screenRes.x - 200, screenRes.y - 100, 180,50), debugPathfinding, "Debug Pathfinding");
		}

		
		GUI.FocusControl(menuOptions[selectedButtonIndex]);
	}
	
}

[System.Serializable]
public class ButtonGroup
{
	public Texture selected;
	public Texture unSelected;
}

