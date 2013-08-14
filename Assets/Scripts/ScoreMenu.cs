using UnityEngine;
using System.Collections;

public class ScoreMenu : MonoBehaviour {
	public bool closeGUI;

    public Texture2D WinScreen;
    public Texture2D LooseScreen;

    private Texture2D[] Screens = new Texture2D[2];

    public int ScreenSize;

	// Use this for initialization
	void Awake () {
		closeGUI = false;
        Screens[0] = LooseScreen;
        Screens[1] = LooseScreen;
	}
    IEnumerator CloseGUI()
    {

        yield return new WaitForSeconds(1.5f);
        closeGUI = true;
    }
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown)
        {
           StartCoroutine(CloseGUI());
            
        }
	}
    public void SetWinner(int ID)
    {
      //  print("Setwinner in ScoreMenu " + ID);
        Screens[ID - 1] = WinScreen;
    }
	void OnGUI()
	{
		//GUI.Label(new Rect(Screen.width / 2 - 100, 50, 200, 100), "Score");
		//if(GUI.Button(new Rect(Screen.width / 2 - 50, 150, 100, 100), "OK"))
		//{
		//	closeGUI = true;
		//}

        GUI.DrawTexture(new Rect(Screen.width * 0.25f - ScreenSize / 2, Screen.height / 2, ScreenSize, ScreenSize), Screens[0], ScaleMode.ScaleToFit, true);
        GUI.DrawTexture(new Rect(Screen.width * 0.75f - ScreenSize / 2, Screen.height / 2, ScreenSize, ScreenSize), Screens[1], ScaleMode.ScaleToFit, true);
	}
}
