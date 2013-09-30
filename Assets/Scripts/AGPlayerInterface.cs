using UnityEngine;
using System.Collections;

public class AGPlayerInterface : MonoBehaviour {
	
	public Vector2 CandyBarOffsetR;	
	public Vector2 CandyBarOffsetL;
	public Vector2 HealthBarOffsetR;
	public Vector2 HealthBarOffsetL;
	
	public Vector2 StatsBackPos;	
	public Vector2 HeadTexturePos;
	
	public Texture2D CandybarTexture;
	public Texture2D HealthbarTexture;
	public Texture2D StatsBackTexture;
	
	public HeadTextureGroup[] HeadTextures;
	private Texture2D currentHeadTexture;
	private int headGroupID;
	
	public GameObject VignettePrefab;
	public Vignette vignette;
	
	public GUIStyle guiStyle;
	Vector2 SplitOffset;
	AGPlayerController player;
	
	float test;
	
	public void SetOffset(Vector2 _offset){
		SplitOffset.x = Screen.width * _offset.x;
		SplitOffset.y = Screen.width * _offset.y;
		
	}
	
	public void SetPlayer(AGPlayerController _p){	
		player = _p;
		SetOffset(AGGame.Get2DCameraOffset(player));
	}
	
	// Use this for initialization
	void Start () {
		if(player.isAIPlayer) return;
		if(!vignette)
		{
			GameObject obj = (GameObject)GameObject.Instantiate (VignettePrefab);
			vignette = obj.GetComponent<Vignette> ();
			vignette.SetPlayer (player);
			if(player.hasAIOpponent && !AGGame.Instance.UseSplitScreen) vignette.SetSinglePlayerMode();
			SetHeadTextureGroupID();
		}
	}
	
	public void SetHeadTextureGroupID()
	{
		if(player.info.PlayerClass == AGPlayerClass.Classes.Werewolf)
			headGroupID = 0;
		else
			headGroupID = 1;
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void UpdateVignette(bool hit, float percnt)
	{
		if(!vignette)
			return;
        vignette.UpdateVignetteAlpha(hit, percnt);
	}
	
	public void DestroyVignette()
	{
		if(vignette) Destroy(vignette.gameObject);
	}
	
	public void UpdateHeadTexture(AGActor.LightState lightState)
	{
		if(lightState == AGActor.LightState.Dream)
			currentHeadTexture = HeadTextures[headGroupID].dreamHead;
		else if(lightState == AGActor.LightState.Real)
			currentHeadTexture = HeadTextures[headGroupID].realHead;
	}
	
	void OnGUI() {
		if(!player || !player.pawn)
			return;
		if(player.PlayerID == 1)
		{
			//HUD
			float health = (float) player.pawn.Health.currentValue / player.pawn.Health.max;
			float candy = (float) player.pawn.Energy.currentValue / player.pawn.Energy.max;
			GUI.BeginGroup(new Rect(Screen.width/2 - SplitOffset.x - StatsBackPos.x - StatsBackTexture.width, SplitOffset.y + StatsBackPos.y, StatsBackTexture.width, StatsBackTexture.height));
				GUI.DrawTexture(new Rect(StatsBackTexture.width, 0, -StatsBackTexture.width, StatsBackTexture.height), StatsBackTexture);
				GUI.BeginGroup(new Rect(HealthBarOffsetL.x + (1-health) * HealthbarTexture.width, HealthBarOffsetL.y, health * HealthbarTexture.width, HealthbarTexture.height));
					GUI.DrawTexture(new Rect(health * HealthbarTexture.width, 0, -HealthbarTexture.width, HealthbarTexture.height), HealthbarTexture);
				GUI.EndGroup();
				GUI.BeginGroup(new Rect(CandyBarOffsetL.x + (1-candy) * CandybarTexture.width, CandyBarOffsetL.y, candy * CandybarTexture.width, CandybarTexture.height));
					GUI.DrawTexture(new Rect(candy * CandybarTexture.width, 0, -CandybarTexture.width, CandybarTexture.height), CandybarTexture);
				GUI.EndGroup();
				
			GUI.EndGroup();
			GUI.DrawTexture(new Rect(Screen.width/2 - SplitOffset.x - HeadTexturePos.x - currentHeadTexture.width, SplitOffset.y + HeadTexturePos.y, currentHeadTexture.width, currentHeadTexture.height), currentHeadTexture);
		} else
		{
			float health = (float) player.pawn.Health.currentValue / player.pawn.Health.max;
			float candy = (float) player.pawn.Energy.currentValue / player.pawn.Energy.max;
			GUI.BeginGroup(new Rect(SplitOffset.x + StatsBackPos.x, SplitOffset.y + StatsBackPos.y, StatsBackTexture.width, StatsBackTexture.height));
				GUI.DrawTexture(new Rect(0, 0, StatsBackTexture.width, StatsBackTexture.height), StatsBackTexture);
				GUI.BeginGroup(new Rect(HealthBarOffsetR.x, HealthBarOffsetR.y, health * HealthbarTexture.width, HealthbarTexture.height));
					GUI.DrawTexture(new Rect(0, 0, HealthbarTexture.width, HealthbarTexture.height), HealthbarTexture);
				GUI.EndGroup();
				GUI.BeginGroup(new Rect(CandyBarOffsetR.x, CandyBarOffsetR.y, candy * CandybarTexture.width, CandybarTexture.height));
					GUI.DrawTexture(new Rect(0, 0, CandybarTexture.width, CandybarTexture.height), CandybarTexture);
				GUI.EndGroup();
				
			GUI.EndGroup();
			GUI.DrawTexture(new Rect(SplitOffset.x + HeadTexturePos.x + currentHeadTexture.width, SplitOffset.y + HeadTexturePos.y, -currentHeadTexture.width, currentHeadTexture.height), currentHeadTexture);
		}
	
	}
	
}


[System.Serializable]
public class HeadTextureGroup
{
	public Texture2D realHead;
	public Texture2D dreamHead;
}
