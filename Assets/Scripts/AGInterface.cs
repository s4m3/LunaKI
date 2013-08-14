using UnityEngine;
using System.Collections;

public class AGInterface : MonoBehaviour {
	public Texture2D HealthBarTexture;
	public Texture2D HealthBarBackGround;

	public Vector2 HealthBarPos;
	public Vector2 HealthBarSize;
    public float ScoreSize;

	public Color HealthBarColor;
    public GUIStyle style;
	
	public Texture2D EnergyBarTexture;
	public Texture2D EnergyBarBackGround;
	
	public Vector2 EnergyBarPos;
	public Vector2 EnergyBarSize;
	
	public Color EnergyBarColor;
	
	Vector2 SplitOffset;
   

	AGPlayerController player;
	
	public void SetOffset(Vector2 _offset){
		SplitOffset.x = Screen.width * _offset.x;
		SplitOffset.y = Screen.width * _offset.y;
		
	}
	
	public void SetPlayer(AGPlayerController _p){	
		player = _p;
		SetOffset(AGGame.Get2DCameraOffset(player));
        HealthBarColor = _p.info.PlayerColor;
 
	}
	
	void Start () {
	
	}
	
	void Update () {
		
	}
	void OnGUI(){
		if(player != null && player.pawn != null){
			if(player.PlayerID == 1)
			{
				GUI.color = Color.gray;
				GUI.DrawTexture( new Rect(Screen.width/2 - SplitOffset.x - HealthBarPos.x, SplitOffset.y + HealthBarPos.y, -HealthBarSize.x, HealthBarSize.y), HealthBarBackGround);
				GUI.DrawTexture( new Rect(Screen.width/2 - SplitOffset.x - EnergyBarPos.x, SplitOffset.y + EnergyBarPos.y, -EnergyBarSize.x, EnergyBarSize.y), EnergyBarBackGround);
				
				GUI.color = HealthBarColor;
				GUI.DrawTexture( new Rect(Screen.width/2 - SplitOffset.x - HealthBarPos.x, SplitOffset.y + HealthBarPos.y, -HealthBarSize.x * (float) player.pawn.Health.currentValue / player.pawn.Health.max, HealthBarSize.y), HealthBarTexture, ScaleMode.StretchToFill, false);
		
				GUI.color = EnergyBarColor;

                style.fontSize = 30;
                GUI.TextArea(new Rect(Screen.width / 2 - SplitOffset.x - HealthBarPos.x - 100, SplitOffset.y + HealthBarPos.y + 50, 45, 30), player.pawn.Health.currentValue.ToString(), style);
				//GUI.DrawTexture( new Rect(Screen.width/2 - SplitOffset.x - EnergyBarPos.x, SplitOffset.y + EnergyBarPos.y, -EnergyBarSize.x * (float) player.pawn.Energy / player.pawn.MaxEnergy, EnergyBarSize.y), EnergyBarTexture, ScaleMode.StretchToFill, false);
              //  GUI.DrawTexture(new Rect(Screen.width / 2 - SplitOffset.x - EnergyBarPos.x, SplitOffset.y + EnergyBarPos.y, -EnergyBarSize.x * player.Action_Shot.CoolDownModifer / player.Action_Shot.MaxCoolDownModifer, EnergyBarSize.y), EnergyBarTexture, ScaleMode.StretchToFill, false);
                
            }
			else
			{
				GUI.color = Color.gray;
				GUI.DrawTexture( new Rect(SplitOffset.x + HealthBarPos.x, SplitOffset.y + HealthBarPos.y, HealthBarSize.x, HealthBarSize.y), HealthBarBackGround);
				GUI.DrawTexture( new Rect(SplitOffset.x + EnergyBarPos.x, SplitOffset.y + EnergyBarPos.y, EnergyBarSize.x, EnergyBarSize.y), EnergyBarBackGround);
				
				GUI.color = HealthBarColor;
				GUI.DrawTexture( new Rect(SplitOffset.x + HealthBarPos.x, SplitOffset.y + HealthBarPos.y, HealthBarSize.x * (float) player.pawn.Health.currentValue / player.pawn.Health.max, HealthBarSize.y), HealthBarTexture, ScaleMode.StretchToFill, false);
		
				GUI.color = EnergyBarColor;
                style.fontSize = 30;
                GUI.TextArea(new Rect(SplitOffset.x + HealthBarPos.x +55, SplitOffset.y + HealthBarPos.y + 50, 45, 30), player.pawn.Health.currentValue.ToString(), style);
               // GUI.DrawTexture(new Rect(SplitOffset.x + EnergyBarPos.x, SplitOffset.y + EnergyBarPos.y, EnergyBarSize.x * player.Action_Shot.CoolDownModifer / player.Action_Shot.MaxCoolDownModifer, EnergyBarSize.y), EnergyBarTexture, ScaleMode.StretchToFill, false);
			    

            }
		}

        if (player != null && player.info != null)
        {
            style.normal.textColor = player.info.PlayerColor;
            style.fontSize = (int) ScoreSize;
            GUI.Label(new Rect(((Screen.width / 2 - ScoreSize) + ScoreSize * (player.PlayerID - 1)), ScoreSize/2, ScoreSize, ScoreSize), player.info.Score.ToString(), style);
        }
	}
}
