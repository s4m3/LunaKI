using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AGGame : MonoBehaviour {

    public Color DreamColor;
    public Color RealColor;

    public GameObject playerTemplate;
    public Material PlayerMaterialTemplate;
    public Material PlayerProjectorMaterialTemplate;
    public Material AimHelpMaterialTemplate;

    public AGPlayerInfo[] Players;
	public SpawnPointGroup[] SpawnPointList;
	AGSpawnPoint[] chosenSpawnPoints;
	private static AGGame game;

    public Transform Planet;
	public AGSun Sun;
	public int Rounds;
	private int currentRound = 1;
	public Vector2[] CameraOffsets = new Vector2[2];
	public Vector2 DefaultScreenSize = new Vector2(1920f, 1080f);
	
	public List <AGPlayerClass> PlayerClasses;
	
	public enum GameState { Running, RoundOver, Countdown, Initialize, InitializeRound, ReadyForNextRound, ChosingCharacter, GameOver, Wait, MainMenu }
	public GameState gameState;
	public GameObject countdownPrefab;
	private Countdown countdown;
	public GameObject scoreMenuPrefab;
	private ScoreMenu scoreMenu;
	public GUIStyle guiStyle;
	public GameObject textInterfacePrefab;
	private TextInterface textInterface;
	public GameObject gameInterfacePrefab;
	private AGGameInterface gameInterface;
	public GameObject mainMenuPrefab;
	private MainMenu mainMenu;
	
	public GUIManager guiManager;
	
	public GameObject pathfinderPrefab;
	private Pathfinder pathfinder;
	
	//DEBUG VARIABLES
	public GameObject textTest;
	public GameObject nodeSphere;
	
	public bool DebugMode;

    bool m_spawnDelay = true;
	protected AGSoundServer m_SoundServer = null; // TODO
	public AGSoundServer m_SoundServerPrefab = null;
	public AGSoundServer getSoundServer ()
	{
		return m_SoundServer;
	}

	public static Vector2 Get2DCameraOffset(AGPlayerController _player){
		return Instance.CameraOffsets[_player.PlayerID-1];	
	}
	
    public static AGGame Instance
    {
        get
        {
            return game != null ? game : GetInstance();
        }
    }

    private static AGGame GetInstance()
    {
	
		game = (AGGame) GameObject.FindObjectOfType(typeof(AGGame));
       	return game;
    }
    

    // First time initialization of players 
    // Materials, Actionns, PlayerInfo
    public void SetPlayer(int Id, AGPlayerController cont)
    {       
		int index = Id-1;
		
		Players[index].Controller = cont;
        Players[index].InitPlayer();
        Players[index].Controller.info = Players[index];

        //Get PlayerClass Instance from GameLogic
		AGPlayerClass pClass = GetPlayerClass(Players[index].PlayerClass);

        //create a object of that Action for that Class by instantiating the Prefab-Action assigend to the prior recieved PlayerClass
        GameObject shotActionObj = (GameObject)Instantiate(pClass.Shot.gameObject);
        shotActionObj.transform.parent = cont.transform;
        cont.Action_Shot = shotActionObj.GetComponent<AGAction_Shot>();			
		cont.Action_Shot.SetOwner(cont);

        GameObject dashActionObj = (GameObject)Instantiate(pClass.Dash.gameObject);
        dashActionObj.transform.parent = cont.transform;
        dashActionObj.name = cont.PlayerID.ToString();
        cont.Action_Dash = (AGAction_Dash)dashActionObj.GetComponent<AGAction>();
        cont.Action_Dash.SetOwner(cont);
		//if(gameInterface) gameInterface.SetPlayerColor(cont.info.PlayerColor, index);

        GameObject meleeActionObj = (GameObject)Instantiate(pClass.Melee.gameObject);
        meleeActionObj.transform.parent = cont.transform;
        cont.Action_Melee = meleeActionObj.GetComponent<AGAction_Melee>();
        cont.Action_Melee.SetOwner(cont);

        GameObject ultimateActionObj = (GameObject)Instantiate(pClass.Ultimate.gameObject);
        ultimateActionObj.transform.parent = cont.transform;
        cont.Action_Ultimate = ultimateActionObj.GetComponent<AGAction_Ultimate>();
        cont.Action_Ultimate.SetOwner(cont);

        //if (gameInterface) gameInterface.SetPlayerColor(cont.info.PlayerColor, index);
    }

	void Start () {
		gameState = GameState.Initialize;
		InitializeGUIManager();
	}
	
	void InitializeGUIManager()
	{
		guiManager = new GUIManager();
		guiManager.ScreenResolution = new Vector2(Screen.width, Screen.height);
		guiManager.DefaultScreenResolution = DefaultScreenSize;
	}


    void Awake ()
	{
		GetSpawnPoints ();
		//SETUP Planet
		Planet.gameObject.layer = 8;

		LightPosUpdater updater = Planet.gameObject.AddComponent<LightPosUpdater> ();
		updater.LightPos = Sun.transform;

		Sun.GetComponent<AGSun> ().centerOfRotation = Planet;
		
		m_SoundServer = (AGSoundServer)Instantiate(m_SoundServerPrefab);
	}
	void Update () {
		PerformGameLoop();
	}
	
	void PerformGameLoop() 
	{
		switch (gameState)
		{
			case GameState.Initialize:
				InitializeGame();
				break;
			case GameState.MainMenu:
				WaitForPlayersToHaveChosenMainMenuSettings();
				break;
			case GameState.ChosingCharacter:
				WaitForPlayersToHaveChosenCharacter();
				break;
			case GameState.InitializeRound:
				WaitForPlayersToBeReady();
				break;
			case GameState.ReadyForNextRound:
				ReadyForNextRound();
				break;
			case GameState.Countdown:
				Countdown();
				break;
			case GameState.Running:
                Sun.Spin();
				NotifyGameStateToPlayerController(gameState);
				if(countdown) Destroy(countdown);
                
				break;
			case GameState.RoundOver:
				NotifyGameStateToPlayerController(gameState);
				ShowRoundResults();
				break;
			case GameState.GameOver:
				NotifyGameStateToPlayerController(gameState);
				CleanUp();
				break;
			default:
				break;
		}
	
	}
    
    public void SetPlayerController(AGPlayerController c)
    {
        Players[c.PlayerID - 1].Controller = c;
    }

	void InitializeGame()
	{
		currentRound = 1;
		if(!textInterface)
		{
			GameObject obj = (GameObject) GameObject.Instantiate(textInterfacePrefab);
			textInterface = obj.GetComponent<TextInterface>();
		}
		foreach(AGPlayerInfo player in Players)
		{             
			player.Controller.InitializePlayer();
	    }
		
		InitMainMenu();
	}
	
	void InitMainMenu()
	{
		if(!mainMenu)
		{
			GameObject obj1 = (GameObject) GameObject.Instantiate(mainMenuPrefab);
			mainMenu = obj1.GetComponent<MainMenu>();
		}
		gameState = GameState.MainMenu;
	}

	
	void ChooseCharacters()
	{
		gameState = GameState.ChosingCharacter;
		if(mainMenu) Destroy(mainMenu);
		AssignSpawnPoints();
		foreach (AGPlayerInfo info in Players)
        {
			
            info.Controller.SetCharacterChoice();
			if(Planet && chosenSpawnPoints[0]) info.Controller.SetCharacterChoiceBackgroundPlanet(Planet, chosenSpawnPoints);
        }
	}
	
    void StartRound()
    {
		InitGameInterface();
		foreach(AGPlayerInfo player in Players)
		{
			int classIndex = player.Controller.charChoiceMenu.ChosenCharacter;

            player.PlayerClass = PlayerClasses[classIndex].ClassType;
		}
		
		if(scoreMenu) Destroy(scoreMenu.gameObject);
        SpawnPlayers();
        foreach (AGPlayerInfo info in Players)
        {
            info.Controller.SetRoundStart();
        }
		gameInterface.ShowController = true;

        gameState = GameState.InitializeRound;
    }
	void WaitForPlayersToBeReady()
	{
		bool allPlayersReady = true;
        
		foreach(AGPlayerInfo player in Players)
		{
			if(!player.Controller.playerReady) allPlayersReady = false;

		}
		if(allPlayersReady) 
		{
			gameState = GameState.ReadyForNextRound;
			gameInterface.ShowController = false;
		}
		
	}
	
	void WaitForPlayersToHaveChosenMainMenuSettings()
	{
		if(!mainMenu || mainMenu.gameMode == MainMenu.GameMode.None) return;
		
		DebugMode = mainMenu.isDebugMode();
		m_SoundServer.Un_Mute(mainMenu.isMuted);
		if(mainMenu.gameMode == MainMenu.GameMode.Multiplayer)
		{
			ChooseCharacters();	
		} else if(mainMenu.gameMode == MainMenu.GameMode.Singleplayer)
		{
			SetupAIPlayer(mainMenu.Difficulty());
			ChooseCharacters();
		}
		//TODO: SOUND
		m_SoundServer.m_AudioListenerPrefab.enabled = false;
	}
	
	void SetupAIPlayer(float difficulty)
	{
		GameObject obj1 = (GameObject) GameObject.Instantiate(pathfinderPrefab);
		pathfinder = obj1.GetComponent<Pathfinder>();
		pathfinder.Planet = Planet.gameObject;
		pathfinder.nodeSphere = nodeSphere;
		pathfinder.Init();
		pathfinder.text = textTest;
		print (Players[0].Controller.pawn);
		Players[Players.Length - 1].Controller.SetupAIController(Planet.gameObject, Players[0].Controller.pawn, pathfinder, difficulty);
		
		if(!DebugMode) 
		{
			foreach(AGPlayerInfo player in Players) 
			{
				player.Controller.ChangeCameraRig();
				if(!player.Controller.isAIPlayer) player.Controller.hasAIOpponent = true;
			}
		}
		
	}
	
	public AGPawn GetEnemy()
	{
		return Players[0].Controller.pawn;
	}
	
	void WaitForPlayersToHaveChosenCharacter()
	{
		bool allPlayersHaveChosen = true;
        
		foreach(AGPlayerInfo player in Players)
		{
			if(!player.Controller.characterChosen) allPlayersHaveChosen = false;
		}
		if(allPlayersHaveChosen) 
		{
			foreach(AGPlayerInfo player in Players)
			{
				if(player.Controller.charChoiceMenu) Destroy(player.Controller.charChoiceMenu.gameObject);
			}
			StartRound();
		}
		
	}
	
	void InitGameInterface()
	{
		if(!gameInterface)
		{
			GameObject obj1 = (GameObject) GameObject.Instantiate(gameInterfacePrefab);
			gameInterface = obj1.GetComponent<AGGameInterface>();
			gameInterface.Rounds = this.Rounds;
		}
	}

    void SpawnPlayers()
    {
        foreach (AGPlayerInfo player in Players)
        {
            if (player.Controller.pawn)
            {
                Destroy(player.Controller.pawn.gameObject);
            }
           SpawnPlayer(player.Controller);
        }
    }

	void ReadyForNextRound()
	{
        //print("Ready for nexst Round");
		gameState = GameState.Countdown;
		if(!countdown)
		{
			GameObject obj = (GameObject) GameObject.Instantiate(countdownPrefab);
			countdown = obj.GetComponent<Countdown>();
		}
		countdown.StartCountdown();

        foreach (AGPlayerInfo info in Players)
        {
            Destroy(info.Controller.playerMenu.gameObject);
  
        }
		
	}
	
	void Countdown()
	{
		
        if (m_spawnDelay == false)
        {
            spawnPlayer();
            m_spawnDelay = true;
        }
        if (countdown.isStartedOnce())
        {
            setPawnRenderer(true);
            m_spawnDelay = false;
        }

        if (countdown.RestSeconds <= 0)
        {
            gameState = GameState.Running;
            textInterface.printToScreen("FIGHT!", 0.5f, true);
        }
    }
	
	
	public void PlayerDied(int ID)
	{
		//print ("player_" + ID + " died");
        int Winner = 0;
        if (ID == 1) Winner = 2;
        if (ID == 2) Winner = 1;
       
       // print("Playerdied :" + ID + "    winner = " + Winner);
        Players[Winner-1].Score++;

        Players [0].Controller.pawn.m_roundIsOver = true;
        Players [1].Controller.pawn.m_roundIsOver = true;
//        Players [Winner - 1].Controller.pawn.BroadcastMessage("doAnimation","win");
		Players [Winner - 1].Controller.pawn.PlayAnimation ("win");
        
		//SpawnPlayer(Players[ID].Controller);
        EndRound(Winner);
	}
    void EndRound(int WinnerID)
    {
        if(!scoreMenu)
		{
			GameObject obj = (GameObject) GameObject.Instantiate(scoreMenuPrefab);
			scoreMenu = obj.GetComponent<ScoreMenu>();
            scoreMenu.SetWinner(WinnerID);
		}
		if(gameInterface)gameInterface.SetWinner(currentRound, WinnerID);
		currentRound++;
        gameState = GameState.RoundOver;
        Players[WinnerID - 1].Controller.AGCam.StartViewDolly(Players[WinnerID - 1].Controller.pawn);
		ResetSpawnPoints();

    }
	void ShowRoundResults()
	{
		if(!isGameFinished())
		{
			if(scoreMenu.closeGUI)
			{
				StartRound();
			}
		} else
		{
			gameState = GameState.Wait;
			StartCoroutine(startFinalCameraRide());
		}
	}
	//@PHIL: hier die kamera fahrt rein
	private IEnumerator startFinalCameraRide()
	{
		//TODO:das hier nur uebergangsweise...
		//mit yield eine bestimmte zeit abwarten und dann den gamestate auf GameState.GameOver setzen.
		yield return new WaitForSeconds(4f);
		gameState = GameState.GameOver;
	}
	
	private bool isGameFinished()
	{
		int scoreToReach = Rounds/2 + 1;
		foreach(AGPlayerInfo player in Players)
		{
			if(player.Score >= scoreToReach) 
			{
				//print ("Player " + player.PlayerName + " wins the game!");
				return true;
			}
		}
		return false;
	}
	
	private void CleanUp()
	{
		if(gameInterface) Destroy(gameInterface.gameObject);
		if(countdown) Destroy(gameInterface.gameObject);
		if(textInterface) Destroy(textInterface.gameObject);
		if(scoreMenu) Destroy(scoreMenu.gameObject);
        foreach (AGPlayerInfo inf in Players)
        {
            inf.Score = 0;
        }
		ResetSpawnPoints();
		gameState = GameState.Initialize;
		
	}
	
	void GetSpawnPoints(){
//		Object[] arr = GameObject.FindObjectsOfType(typeof(AGSpawnPoint));
//		int amountOfSpawnPointTuples = arr.Length/2;
//		spawnPointList = new SpawnPointGroup[amountOfSpawnPointTuples];
//		int counter = 0;
//		for (int i = 0; i<amountOfSpawnPointTuples;i++)
//		{
//			spawnPointList[i].spawnPoint1 = (AGSpawnPoint) arr[counter++];
//			spawnPointList[i].spawnPoint2 = (AGSpawnPoint) arr[counter++];
//		}
		AssignSpawnPoints();
	}
	
	void AssignSpawnPoints()
	{
		chosenSpawnPoints = new AGSpawnPoint[2];
		int rand = Random.Range(0, SpawnPointList.Length);
		chosenSpawnPoints[0] = SpawnPointList[rand].spawnPoint1;
		chosenSpawnPoints[1] = SpawnPointList[rand].spawnPoint2;
	}
	
    public bool SpawnPlayer(AGPlayerController controller)
    {
        AGSpawnPoint pos = GetStart();      
		GameObject pl  = (GameObject) GameObject.Instantiate(playerTemplate, pos.transform.position, pos.transform.rotation);
        pl.name = "PAWN ("+controller.info.PlayerName+")";
		AGPawn newPawn = pl.GetComponent<AGPawn>();
		if(newPawn != null) {
            newPawn.BasePlanet = Planet.GetComponent<MeshCollider>();
			newPawn.InitActor();				
			controller.SetPawn(newPawn);
			controller.SetInterface();
			return true;
		}
       return false;
    }
		
    AGSpawnPoint GetStart()
    {
        foreach (AGSpawnPoint point in chosenSpawnPoints)
        {
            if (!point.isUsed) 
			{
				point.isUsed = true;
				return point;
			}
        }

        //Debug.Log("No Spawn for Player " + c.PlayerID + " found");
        return null;
    }
	
	void ResetSpawnPoints()
	{
		foreach(SpawnPointGroup spg in SpawnPointList)
		{
			spg.spawnPoint1.isUsed = false;
			spg.spawnPoint2.isUsed = false;
		}
	}
	
	public static bool CanHitTarget(GameObject obj, AGProjectile proj, out AGActor actor){
        bool canHit = true;
		actor = obj.GetComponent<AGActor>();
        if (proj.Instigator.pawn && obj == proj.Instigator.pawn.gameObject)
        {
            canHit = proj.FriendlyFire;
        }

        return canHit;
	}
	
	public AGPlayerClass GetPlayerClass(AGPlayerClass.Classes c){
		foreach (AGPlayerClass pc in PlayerClasses){
			if (c == pc.ClassType){
             
				return pc;				
			}
		}
		return null;
	}
	private void NotifyGameStateToPlayerController(GameState gameState)
	{
		foreach(AGPlayerInfo player in Players)
		{
			player.Controller.NotifyGameState(gameState);
		}
	}

    private void setPawnRenderer(bool enable)
    {
        foreach (AGPlayerInfo info in Players)
        {
            info.Controller.pawn.setRenderer(enable);
        }
    }

    private void spawnPlayer()
    {
        foreach (AGPlayerInfo info in Players)
        {
            info.Controller.pawn.Spawn();
        }
    }

        
}

//PlayerInfo holds all individual Playerproperties

[System.Serializable]
public class AGPlayerInfo {
    public string PlayerName;
    public int Score;
    public Texture2D PlayerImage;
    public Color PlayerColor;
	
    
	public AGPlayerClass.Classes PlayerClass;
	[HideInInspector]
	public AGPlayerController Controller;
	[HideInInspector]
    public Material PlayerProjectorMaterial;
	[HideInInspector]
    public Material PlayerMaterial;
    [HideInInspector]
    public Material AimHelpMaterial;	

    public void InitPlayer()
    {
        PlayerMaterial = new Material(AGGame.Instance.PlayerMaterialTemplate);
        PlayerMaterial.SetColor("_Color", PlayerColor);

        PlayerProjectorMaterial = new Material(AGGame.Instance.PlayerProjectorMaterialTemplate);
        PlayerProjectorMaterial.SetColor("_Color", PlayerColor);

        AimHelpMaterial = new Material(AGGame.Instance.AimHelpMaterialTemplate);
        AimHelpMaterial.SetColor("_Color", PlayerColor);		
    }
}

[System.Serializable]
public class AGPlayerClass{
	public enum Classes {
		Werewolf,
	    Witch
	};
	public Classes ClassType;
	public string ClassName;
	public AGAction Shot;
	public AGAction Dash;
    public AGAction Melee;
	public AGAction Ultimate;
	public Texture2D Image;
	public GameObject ClassMeshReal;
    public GameObject ClassMeshDream;

  public AGPlayerClassStats RealSideStats;
  public AGPlayerClassStats DreamSideStats;
}

[System.Serializable]
public class AGPlayerClassStats
{
    public AGStat Health;
    public AGStat Energy;

    public AGStatSingle Speed;
    public AGStatSingle Attack;
    public AGStatSingle Defense;
    public AGStatSingle MeshSize;
    public AGStatSingle EnergyGainRate;

    public void InitStats()
    {      
        Health.InitStat();
        Energy.InitStat();
   
    }
}

[System.Serializable]
public class SpawnPointGroup
{
	public AGSpawnPoint spawnPoint1;
	public AGSpawnPoint spawnPoint2;
}