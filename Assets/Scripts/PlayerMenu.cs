using UnityEngine;
using System.Collections;

public class PlayerMenu : MonoBehaviour {
	Vector2 SplitOffset;
	private Vector2 ReadyButtonPos;
	AGPlayerController player;
	private bool joystickButtonDown;
	public Texture2D buttonTextureL;
	public Texture2D buttonTextureR;
	private Texture2D buttonTexture;
	public Texture2D readyTexture;
	public Texture2D greyTexture;
	public GUIStyle guiStyle;
	private bool greyOut;
	
	public void SetOffset(Vector2 _offset){
		SplitOffset.x = Screen.width * _offset.x;
		SplitOffset.y = Screen.width * _offset.y;
		
	}
	
	public void SetPlayer(AGPlayerController _p){	
		player = _p;
		SetOffset(AGGame.Get2DCameraOffset(player));
		buttonTexture = player.PlayerID == 1 ? buttonTextureL : buttonTextureR;
		ReadyButtonPos = new Vector2(Screen.width/4 - buttonTexture.width/2, Screen.height/2 - buttonTexture.height/2);
	}
	void Awake () {
		greyOut = true;
		joystickButtonDown = false; 
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("FireA_p"+player.PlayerID))
		{
			joystickButtonDown = true;
		}
		if(player.isAIPlayer) joystickButtonDown = true;
	}
	
	void OnGUI ()
	{
		if (!player || !player.pawn || !buttonTexture)
			return;
		
		if (greyOut) { 
			GUI.DrawTexture (new Rect (SplitOffset.x, SplitOffset.y, Screen.width / 2, Screen.height), greyTexture);
		}
		if (!player.playerReady && GUI.Button (new Rect (SplitOffset.x + ReadyButtonPos.x, SplitOffset.y + ReadyButtonPos.y, buttonTexture.width, buttonTexture.width), buttonTexture, guiStyle) || joystickButtonDown) {
			player.playerReady = true;
			greyOut = false;
			AGGame.Instance.getSoundServer ().Play ("menu");
		}
		
		if (player.playerReady) {
			GUI.Label (new Rect (SplitOffset.x + ReadyButtonPos.x, SplitOffset.y + ReadyButtonPos.y, readyTexture.width, readyTexture.height), readyTexture);
		}
	}
}
