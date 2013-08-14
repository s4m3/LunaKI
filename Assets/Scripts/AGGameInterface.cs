using UnityEngine;
using System.Collections;

public class AGGameInterface : MonoBehaviour {
	public Texture2D crownBackground;
	public Texture2D crownWin;
	public Texture2D[] crownFinalTextures;
	public Texture2D controllerTexture;
	public Texture2D ultimateDescriptionTexture;
	private int rounds;
	public int Rounds
	{
		get { return rounds; }
		set { rounds = value; }
	}
	private int[] roundWin;
	private int guiStartPos;
	public int pixelsBetweenCrownTextures = 20;
	public int crownTopPosition = 50;
	public bool showRoundsVertical = false;
	private int[] roundView;
	public int ControllerTopPos;
	private bool showController = false;
	public bool ShowController
	{
		set{ showController = value; }
	}
	void Start () {
		roundWin = new int[Rounds];
		roundView = new int[Rounds];
		guiStartPos = Screen.width/2 - ((crownBackground.width+pixelsBetweenCrownTextures) * (Rounds/2)) + pixelsBetweenCrownTextures/2;
		if(Rounds % 2 == 1) guiStartPos -= (crownBackground.width/2 + pixelsBetweenCrownTextures/2);
	}
	
	public void SetWinner(int roundNumber, int WinnerID)
	{
		if(roundNumber > Rounds) return;
		roundWin[roundNumber-1] = WinnerID;
		roundView = calculateRoundView(roundWin);
	}
	
	void OnGUI()
	{
		Texture2D crownTex;
		//Group of GUI Items: Crowns
		GUI.BeginGroup(new Rect(guiStartPos, crownTopPosition, (Rounds-1)*(crownBackground.width + pixelsBetweenCrownTextures) + crownFinalTextures[0].width, crownFinalTextures[0].height));
		for(int i = 0; i<Rounds; i++)
		{
			if(i != (Rounds/2))
			{
				crownTex = roundView[i] == 0 ? crownBackground : crownWin;
				GUI.DrawTexture(new Rect(i*crownBackground.width + i*pixelsBetweenCrownTextures, 0, crownBackground.width, crownBackground.height), crownTex);
			}
		}
		GUI.EndGroup();
		
		//WinningCrown
		GUI.DrawTexture(new Rect(Screen.width/2 - crownFinalTextures[0].width/2, crownTopPosition - crownFinalTextures[0].height/4, crownFinalTextures[0].width, crownFinalTextures[0].height), crownFinalTextures[roundView[Rounds/2]]);
		
		if(showController) 
		{
			GUI.DrawTexture(new Rect(Screen.width/2 - controllerTexture.width/2, ControllerTopPos, controllerTexture.width, controllerTexture.height), controllerTexture);
			GUI.DrawTexture(new Rect(Screen.width/2 - ultimateDescriptionTexture.width/2, ControllerTopPos + controllerTexture.height + 30, ultimateDescriptionTexture.width, ultimateDescriptionTexture.height), ultimateDescriptionTexture);
		}
	}
	
	private int[] calculateRoundView(int[] wins)
	{
		int wins_p1 = 0, wins_p2 = 0, blanks = 0;
		int[] result = new int[Rounds];
		foreach(int i in wins){
			if(i == 1) wins_p1++;
			if(i == 2) wins_p2++;
		}
		blanks = Rounds - wins_p1 - wins_p2;
		for(int j = 0; j < Rounds; j++)
		{
			if(wins_p1 > 0)
			{
				result[j] = 1;
				wins_p1--;
			} else if(blanks > 0)
			{
				result[j] = 0;
				blanks--;
			} else
			{
				result[j] = 2;
			}
		}
		return result;
	}
	
}
