using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterChoiceMenu : MonoBehaviour {
	private Vector2 SplitOffset;
	public Vector2 CharacterPos = new Vector2(100, 200);
	public Vector2 ChooseCharacterTextPos;
	private AGPlayerController player;
	private bool joystickButtonDown;
	public List<Texture2D> charactersL;
	public List<Texture2D> charactersR;
	private List<Texture2D> characters;
	public Texture2D chooseCharacterText;
	public Texture2D[] buttonL;
	private Texture2D btnL;
	public Texture2D[] buttonR;
	private Texture2D btnR;
	private string currentCharacter;
	private int characterIndex;
	private bool canChangeCharacter = true;
	private bool leftClick = false;
	private bool rightClick = false;
	public Vector2 leftButtonPos;
	public Vector2 rightButtonPos;
	public GUIStyle buttonStyle;
	public Texture2D greyTexture;
	
	public int ChosenCharacter
	{
		get { return characterIndex; }
	}
	
	public void SetOffset(Vector2 _offset){
		SplitOffset.x = Screen.width * _offset.x;
		SplitOffset.y = Screen.width * _offset.y;
		
	}
	
	public void SetPlayer(AGPlayerController _p){	
		player = _p;
		SetOffset(AGGame.Get2DCameraOffset(player));
		characters = player.PlayerID == 1 ? charactersL : charactersR;
	}
	
	void Awake () {
		joystickButtonDown = false; 
		List<AGPlayerClass> playerClasses = AGGame.Instance.PlayerClasses;
		characterIndex = 0;
		characters = characters = new List<Texture2D>();
	}
	
	void Update () {
		if(!player || player.characterChosen) return;
		
		if(player.isAIPlayer)
			DoAIChoice();
		else
			CheckInput();
		
	}
	
	private void DoAIChoice() 
	{
		if(player.characterChosen) return;
		characterIndex = Random.Range(0, characters.Count);
		player.characterChosen = true;
		canChangeCharacter = false;
		
		
	}
	
	private void CheckInput()
	{
		if(Input.GetButton("FireB_p"+player.PlayerID))
		{
			joystickButtonDown = false;
			player.characterChosen = false;
			canChangeCharacter = true;
		}
		
		if (Input.GetButton("FireA_p"+player.PlayerID))
		{
			joystickButtonDown = true;
		}
		if(canChangeCharacter)
		{
			float input = Input.GetAxis("Horizontal_p"+player.PlayerID);
			if(input == 1 || input == -1) 
			{
				StartCoroutine(changeCharacter(input));
			}
		}
	}
	
	private IEnumerator changeCharacter(float input)
	{
		leftClick = input == -1;
		rightClick = input == 1;
		canChangeCharacter = false;
		characterIndex += (int)input;
		if(characterIndex > characters.Count - 1)
			characterIndex = 0;
		if(characterIndex < 0)
			characterIndex = characters.Count - 1;
		//characterIndex = Mathf.Clamp(characterIndex, 0, characters.Count - 1);
		//to prevent the change to happen multiple times per frame, there has to be waiting time
		yield return new WaitForSeconds(0.1f);
		leftClick = rightClick = false;
		canChangeCharacter = true;

	}
	
	
	void OnGUI ()
	{
		GUI.DrawTexture (new Rect (SplitOffset.x, SplitOffset.y, Screen.width / 2, Screen.height), greyTexture);
		
		if (!player.characterChosen)
			GUI.DrawTexture (new Rect (SplitOffset.x + ChooseCharacterTextPos.x, SplitOffset.y + ChooseCharacterTextPos.y, chooseCharacterText.width, chooseCharacterText.height), chooseCharacterText);

		if (GUI.Button (new Rect (SplitOffset.x + CharacterPos.x, SplitOffset.y + CharacterPos.y, characters [characterIndex].width, characters [characterIndex].height), characters [characterIndex], buttonStyle) || joystickButtonDown) {
			player.characterChosen = true;
			AGGame.Instance.getSoundServer ().Play ("menu");
		}
		btnL = !leftClick ? buttonL [0] : buttonL [1];
		btnR = !rightClick ? buttonR [0] : buttonR [1];
		
		if (!player.characterChosen && GUI.Button (new Rect (SplitOffset.x + leftButtonPos.x, SplitOffset.y + leftButtonPos.y, buttonL [0].width, buttonL [0].height), btnL, buttonStyle)) {
			AGGame.Instance.getSoundServer ().Play ("menu");
			StartCoroutine (changeCharacter (-1));
		}
		
		if (!player.characterChosen && GUI.Button (new Rect (SplitOffset.x + rightButtonPos.x, SplitOffset.y + rightButtonPos.y, buttonR [0].width, buttonR [0].height), btnR, buttonStyle)) {
			AGGame.Instance.getSoundServer ().Play ("menu");
			StartCoroutine (changeCharacter (1));
		}
		
//		if(player.playerReady)
//			GUI.Label(new Rect(SplitOffset.x + ReadyButtonPos.x, SplitOffset.y + ReadyButtonPos.y, ReadyButtonSize.x * Screen.width / 2, ReadyButtonSize.y * Screen.height), readyTexture);
	}
}
