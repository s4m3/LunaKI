using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AGPlayerController : MonoBehaviour
{
	public AGCamera AGCam;
	public AGPlayerInfo info;
	public int PlayerID;
	static private int m_KM_PlayerID;
	static private int m_KM_CurrentPlayerID;
	static private int m_KM_MaxPlayerID;
	static private bool m_KM_UseKM;
	public float MovementInputDeadZone;
	public GameObject playerInterfacePrefab;
	public AGPlayerInterface playerInterface;
	public GameObject playerMenuPrefab;
	public PlayerMenu playerMenu;
	public GameObject characterChoiceMenuPrefab;
	public CharacterChoiceMenu charChoiceMenu;
	public AGPawn pawn;
	public AGAction_Shot Action_Shot;
	public AGAction_Dash Action_Dash;
    public AGAction_Ultimate Action_Ultimate;
    public AGAction_Melee Action_Melee;
    
	public bool playerReady;
	public bool characterChosen;
	private bool bIsControllable;
	
	public bool isAIPlayer = false;
	public bool hasAIOpponent = false;
	public float difficulty;
	public GameObject planet;
	public AGPawn enemy;
	public GameObject testObject;
	private Decision rootDecision;
	private DecisionTreeNode newDecision;
	private ActionManager actionManager;
	
	public Pathfinder pathfinder;
	private List<Node> path;
	private int currentPathID;
	private int currentNodeID;
	private bool pathComplete = true;
	private float stuckTime = 0.0f;
	
	private Node closestToSun;
	//if ai player is stuck...
	private Vector3 lastPosition;
	private int trials;

	private float reactionTime;
	private float nextDecisionTime = 0.0f;
	
	void Start ()
	{
		AGGame.Instance.SetPlayerController (this);      
	}

	public void SetRoundStart ()
	{
		playerReady = false;
 
		if (!playerMenu) {
			GameObject obj = (GameObject)GameObject.Instantiate (playerMenuPrefab);
			playerMenu = obj.GetComponent<PlayerMenu> ();
			playerMenu.SetPlayer (this);
			//if(hasAIOpponent) playerMenu.SetOffset(Vector2.zero);
          
		}
	}
	
	public void SetCharacterChoice()
	{
		characterChosen = false;
		
		if(!charChoiceMenu)
		{
			GameObject obj = (GameObject) GameObject.Instantiate(characterChoiceMenuPrefab);
			charChoiceMenu = obj.GetComponent<CharacterChoiceMenu>();
			charChoiceMenu.SetPlayer(this);
		}
	}
	
	public void SetupDecisionMaking()
	{
		//TODO: set reaction time 0.3 seconds for diff=1, 2 seconds for diff = 0.1, calculate scale
		reactionTime = 1.3f + 1f - this.difficulty;
		this.rootDecision = new Decision(this.pawn, this.enemy, this.difficulty);
	}
	public void SetupAIController(GameObject planet, AGPawn enemy, Pathfinder pathfinder, float difficulty)
	{
		this.isAIPlayer = true;
		this.planet = planet;
		this.enemy = enemy;
		this.pathfinder = pathfinder;
		this.currentPathID = -1;
		this.trials = 0;
		this.difficulty = difficulty;


	}
	
	public void SetCharacterChoiceBackgroundPlanet(Transform basePlanet, AGSpawnPoint[] spawnpoints)
	{
		if(!AGCam) return;
		AGCam.SetPlanetBackground(basePlanet, spawnpoints[PlayerID - 1].transform.position);
	}
	
	public void SetInterface()
	{
		if (!playerInterface) {
			GameObject obj = (GameObject)GameObject.Instantiate (playerInterfacePrefab);
			playerInterface = obj.GetComponent<AGPlayerInterface> ();
			playerInterface.SetPlayer (this);
		}
	}
	
	public void InitializePlayer ()
	{
		AGGame.Instance.SetPlayer (PlayerID, this);
		AGCam.CameraRig.gameObject.GetComponent<Camera> ().rect = new Rect (AGGame.Get2DCameraOffset (this).x, AGGame.Get2DCameraOffset (this).y, 0.5f, 1); 

		//(playerMenu);
		
		m_KM_PlayerID = PlayerID; // FIXME: Find right place for this line ...
		m_KM_CurrentPlayerID = m_KM_PlayerID;
		m_KM_MaxPlayerID = m_KM_PlayerID + 1;
		m_KM_UseKM = false;
	}


	public void SetPawn (AGPawn p)
	{
		pawn = p;
		pawn.Player = this;

		pawn.SetLightZoneStats (true);
        pawn.SetLightZoneMesh(false);

		// FIXME: do something with renderer, subrenderer, really add color? 
//		pawn.mesh.renderer.material = info.PlayerMaterial;
//		var renderers = pawn.mesh.GetComponentsInChildren<UnityEngine.Renderer> ();
//		foreach (var ren in renderers) {
//			ren.material.color += info.PlayerMaterial.color;
//		}
		pawn.AimHelp.renderer.material = info.AimHelpMaterial;
    
		pawn.AimHelp.layer = 8 + PlayerID;
		if (p.MyProjector != null) {
			//Debug.Log ("Setting Projector Material Player " + PlayerID);
			p.MyProjector.material = info.PlayerProjectorMaterial;
		}

		Action_Shot.SetShotDisplay ();

		AGCam.SetViewTarget (pawn);		
		print ("setting it up");
		if(isAIPlayer) 
		{
			this.enemy = AGGame.Instance.GetEnemy();
			SetupDecisionMaking();
			actionManager = new ActionManager(this);
			actionManager.ResetActionManager();
						
		}
	}
	
	public void ChangeCameraRig()
	{
		if(isAIPlayer) AGCam.CameraRig.camera.rect = new Rect(0,0,0,0);
		else AGCam.CameraRig.camera.rect = new Rect(0,0,1,1);
	}

	bool PlayerCanMovePawn ()
	{
        return true;
	}
	
	private ActionDecision.ActionDecisionType MakeDecision()
	{
		newDecision = rootDecision.makeDecision();
		return ((newDecision as ActionDecision).actionType);
	}

	void Update ()
	{
		if (!bIsControllable)
			return;
		
		if(isAIPlayer) {
			//MoveAIPlayer();
			MakeNewDecision();
		}	
		else
			CheckInputs ();
		
	}
	
	void LateUpdate ()
	{
		m_KM_PlayerID = m_KM_CurrentPlayerID;
	}
	
	bool hasThisPlayer (bool value)
	{
		if (isThisPlayer () && value && m_KM_UseKM)
			return true;
		return false;
	}
	
	bool isThisPlayer ()
	{
		if (PlayerID == m_KM_PlayerID)
			return true;
		return false;
	}

	void controlButtons ()
	{
		if (hasThisPlayer (Input.GetButton ("KM_Shot")) || 
			Mathf.Abs (Input.GetAxisRaw ("Fire1_p" + PlayerID)) > 0.5f || 
			Input.GetButton("FireA_p"+PlayerID)){
			// Debug.Break();
			ActivateAction (Action_Shot);
		} else if (Action_Shot.bHoldButton) {
			Action_Shot.ReleaseButton ();
		}
		
		if (hasThisPlayer (Input.GetButtonDown ("KM_SwitchPlayer"))) {
			m_KM_CurrentPlayerID = ((m_KM_PlayerID + 1) % m_KM_MaxPlayerID == 0) ? 1 : m_KM_PlayerID + 1;
		}
		
		if (isThisPlayer () && Input.GetButtonDown ("KM_SwitchKM")) {
			m_KM_UseKM = !m_KM_UseKM;
		}


		if (hasThisPlayer (Input.GetButton ("KM_Dash")) ||
			Input.GetButton ("Fire2_p" + PlayerID) || 
			Input.GetButton("FireB_p"+PlayerID)) {
			ActivateAction (Action_Dash);
		} else if (Action_Dash.bHoldButton)
			Action_Dash.ReleaseButton ();

		if (hasThisPlayer (Input.GetButton ("KM_Melee")) ||
			Input.GetButton ("Fire3_p" + PlayerID) || 
			Input.GetButton("FireX_p"+PlayerID)) {
			ActivateAction (Action_Melee);
		} else if (Action_Melee.bHoldButton)
			Action_Melee.ReleaseButton ();

        if (hasThisPlayer (Input.GetButton ("KM_Ultimate")) || 
			Input.GetButton("FireY_p"+PlayerID))
        {
            ActivateAction(Action_Ultimate);
        }
        else if(Action_Ultimate.bHoldButton)
            Action_Ultimate.ReleaseButton();
	}
	

	void controlAxis ()
	{
		float horizontal = Input.GetAxis ("Horizontal_p" + PlayerID);
		float horizontalLook = Input.GetAxis ("HorizontalLook_p" + PlayerID);
		float vertical = Input.GetAxis ("Vertical_p" + PlayerID);
		float verticalLook = -Input.GetAxis ("VerticalLook_p" + PlayerID);
		if (hasThisPlayer (true)) {
//			Print.Log(Screen.width + " x "  + Screen.height);
			float ycenter = Screen.height / 2.0f;
			float xcenter = Screen.width / 2.0f;
			float xPlaceCenter = xcenter / 2.0f;
			float xPlace = (PlayerID - 1) * xcenter + xPlaceCenter; // ||  x  |  o  ||
//			Print.Log(xcenter + " " + xPlaceCenter + " " + xPlace + "    " + Input.mousePosition.x);
//			Print.Log (Input.mousePosition);
			horizontal += Input.GetAxis ("KM_Horizontal"); // A-D
			horizontalLook += Input.mousePosition.x - xPlace; // Input.GetAxis ("KM_HorizontalLook"); // Mouse
			vertical += Input.GetAxis ("KM_Vertikal"); // W-S
			verticalLook += Input.mousePosition.y - ycenter; // Input.GetAxis ("KM_VertikalLook"); // Mouse
		}
		
		Vector3 InputVectorMovement = new Vector3 (horizontal, vertical, 0);
		Vector3 InputVectorLook = new Vector3 (horizontalLook, verticalLook, 0);

		if (pawn != null) {
			ExecuteMovement(InputVectorMovement, InputVectorLook);
		}
	}
	
	void ExecuteMovement(Vector3 InputVectorMovement, Vector3 InputVectorLook)
	{
		//if(isAIPlayer)Debug.Log(InputVectorLook);
		if (PlayerCanMovePawn ()) {
			float MovementInputPercent = Mathf.Clamp ((InputVectorMovement.magnitude - MovementInputDeadZone), 0, 1) / (1 - MovementInputDeadZone);
			InputVectorMovement *= MovementInputPercent;
			Vector3 MoveDirection;
			if(isAIPlayer) {
				MoveDirection = InputVectorMovement;
			} else {
				MoveDirection= AGCam.transform.rotation * InputVectorMovement;
			}
			pawn.UpdateMoveDirection (Tools.CameraVectorToObject (MoveDirection, pawn.gameObject, AGCam));
			Debug.DrawRay (pawn.transform.position, Tools.CameraVectorToObject (MoveDirection, pawn.gameObject, AGCam), Color.cyan); 
		
		}
	
		if (InputVectorLook.magnitude < MovementInputDeadZone)
			InputVectorLook = Vector3.zero;
		Vector3 LookDirection;
		if(isAIPlayer) {
			LookDirection = InputVectorLook;
		} else {
			LookDirection = AGCam.transform.rotation * InputVectorLook;
		}
		
		Debug.DrawRay (pawn.transform.position, Tools.CameraVectorToObject (LookDirection, pawn.gameObject, AGCam), Color.magenta);            
		pawn.SetLookDirection (Tools.CameraVectorToObject (LookDirection, pawn.gameObject, AGCam));	
	}
	
	
	
	public void ExecuteAIAction(Vector3 movementDirection, Vector3 lookDirection, AGAction action)
	{
		if(action is AGAction_Shot) {
			//spread shots to make imperfect
			lookDirection.Normalize();
			float randomizeFactor = Mathf.Clamp(1 - difficulty, 0, 0.8f);
			Tools.RandomizeVector(ref lookDirection, randomizeFactor);
		}
		ExecuteMovement(movementDirection, lookDirection);
		if(action.bHoldButton) action.ReleaseButton();
		else ActivateAction(action);
	}
	
	public void MoveAIPlayer()
	{
		//print("path id:" + currentPathID);
		//print ("actual path id:" + pathfinder.pathID);
//		if(areCloseTogether(pawn.transform.position, enemy.transform.position, 5f))
//		{
//			//print ("is close to enemy");
//			//ExecuteAIAction(new Vector3(0,0,0), (enemy.transform.position - pawn.transform.position), Action_Shot);
//			return;
//		}
		if(currentPathID != pathfinder.pathID)
		{
			//print ("stuck here 1");
			path = pathfinder.path;
			currentPathID = pathfinder.pathID;
		}
		if(path == null || path.Count < 1 || pathComplete) {
			//print ("stuck here 2");
			FindNewPath();
			pathComplete = false;
			return;
		}
		
		MoveAlongPath();
			
	}
	
	void MoveAlongPath() 
	{
		if(pathfinder.isFindingPath) 
		{
			//MoveToSun();
			return;
		}
		closestToSun = null;
		//print ("currentnodeid:" + currentNodeID + " with pathID:" + currentPathID);
		//print ("path count:" + path.Count + " with pathID:"+pathfinder.pathID);
		if(currentNodeID > path.Count - 1)
		{
			print ("path completed, node id: " + currentNodeID);
			pathComplete = true;
			return; 
		}
		Vector3 target = path[currentNodeID].position;
		Vector3 position = pawn.transform.position;
		
		if(areCloseTogether(position, target, 2f)) 
		{
			currentNodeID++;
			//print ("are close");
			return;
		}
//		if(areFarAway(position, target, 10f))
//		{
//			ActivateAction (Action_Dash);
//		}
		
		if(areFarAway(path[path.Count - 1].position, enemy.transform.position, 5))
		{
			print ("enemy too far away from target");
			pathComplete = true;
		}

		Vector3 projection2 = target - position;
		Debug.DrawRay (position, projection2 * 5, Color.white);
		Vector3 InputVectorMovement = projection2.normalized;
		Vector3 InputVectorLook = new Vector3 (0, 0, 0);
		
		if(position == lastPosition && Time.time > stuckTime) 
		{
			currentNodeID++;
			//only increase if character is stuck longer than a second
			stuckTime = Time.time + 1f;
			//print ("stuck here 3");
			InputVectorMovement *= -1;
//			InputVectorMovement = new Vector3(-1f, -1f, 0);
//			InputVectorLook = new Vector3(-10, -1, 0);
//			ExecuteMovement(InputVectorMovement, InputVectorLook);
//			ActivateAction (Action_Dash);
			//FindNewPath();
		}
//		if(trials > 4)
//		{
//			InputVectorMovement *= -1;
//			FindNewPath();
//			print ("finding new path because cannot shoot through");
//			
//			return;
//		}


		ExecuteMovement(InputVectorMovement, InputVectorLook);
		lastPosition = pawn.transform.position;
	}
	
	public void MoveToSun()
	{
		//print ("Moving to the sun");
		if(!closestToSun) {
			int num = pathfinder.kdTree.FindNearest(AGGame.Instance.Sun.transform.position);
			closestToSun = pathfinder.Nodes[num];
		}
		if(areCloseTogether(closestToSun.position, pawn.transform.position, 3f)) return;
			
		ExecuteMovement((closestToSun.position - pawn.transform.position), (enemy.transform.position - pawn.transform.position));
		
	}
	
	public void MoveToDark()
	{
		if(!closestToSun) {
			int num = pathfinder.kdTree.FindNearest(AGGame.Instance.Sun.transform.position);
			closestToSun = pathfinder.Nodes[num];
		}
		if(areFarAway(closestToSun.position, pawn.transform.position, 15f)) return;
			
		ExecuteMovement((pawn.transform.position - closestToSun.position), Vector3.zero);
	}
	
	public void DashToEnemy()
	{
		Vector3 enemyDirection = enemy.transform.position - pawn.transform.position;
		ExecuteAIAction(enemyDirection, enemyDirection, Action_Dash); 
	}
	
	public void MeleeAtEnemy()
	{
		Vector3 enemyDirection = enemy.transform.position - pawn.transform.position;
		if(areCloseTogether(enemy.transform.position, pawn.transform.position, Action_Melee.Range))
		{
			ExecuteAIAction(Vector3.zero, enemyDirection, Action_Melee);
		} else {
			ExecuteAIAction(enemyDirection, enemyDirection, Action_Melee);
		}
	}
	
	public bool IsCloseToEnemy()
	{
		return areCloseTogether(pawn.transform.position, enemy.transform.position, 5f);
	}
	
	public void Shoot()
	{
		ExecuteAIAction(new Vector3(0,0,0), (enemy.transform.position - pawn.transform.position), Action_Shot);
	}
	
	void FindNewPath()
	{
		currentNodeID = 0;
		pathfinder.CalculateNewPath(pawn.transform.position, enemy.transform.position);
	}
	
	
	bool areCloseTogether(Vector3 position, Vector3 target, float distance) 
	{
		return ((position - target).magnitude < distance);	
	}
	
	bool areFarAway(Vector3 position, Vector3 target, float distance) 
	{
		return ((position - target).magnitude > distance);	
	}
	
	void MakeNewDecision()
	{
		//TODO: Only schedule every half a second
		if(Time.time > nextDecisionTime) 
		{
			actionManager.scheduleAction(actionManager.createAction(MakeDecision()));
			nextDecisionTime = Time.time + reactionTime;
		}
		
		actionManager.execute();
	}
	
	void CheckInputs () // FIXME TODO Rewrite for use with Update !!
	{
		controlButtons ();
		controlAxis ();
	}
	
	public bool ActivateAction (AGAction action)
	{
		if (CanActivateAction (action) && pawn.AllowsAction ()) {
			return action.Activate ();
		} else {
			return false;	
		}
	}
	
	public void CleanUp()
	{
		if(pawn) Destroy (pawn.gameObject);
		if(Action_Shot)
		{
			Action_Shot.DestroyShotDisplay();
			Destroy (Action_Shot.gameObject);
		}
		if(Action_Dash) Destroy (Action_Dash.gameObject);
		if(Action_Ultimate) Destroy (Action_Ultimate.gameObject);
		if(Action_Melee) Destroy (Action_Melee.gameObject);
		if(playerInterface) 
		{
			playerInterface.DestroyVignette();
			Destroy(playerInterface.gameObject);
		}
		isAIPlayer = false;
	}

	protected bool CanActivateAction (AGAction action)
	{
		//Maybe check Gamestate or other stuff if an Action can be Performed
		return pawn != null;
	}

	public void PawnDied ()
	{
		//Action_Dash.Deactivate();
		AGGame.Instance.PlayerDied (PlayerID);
		// Action_Ultimate.Deactivate();
		//AGGame.Instance.SpawnPlayer(this);
	}
	
	public void NotifyGameState (AGGame.GameState gameState)
	{
		switch (gameState) {
		case AGGame.GameState.Running:
			bIsControllable = true;
			break;
		case AGGame.GameState.RoundOver:
			bIsControllable = false;
			pawn.UpdateMoveDirection (Vector3.zero);
            Action_Dash.Reset();
            Action_Melee.Reset();
            Action_Ultimate.Reset();
            Action_Shot.Reset();
			break;
		case AGGame.GameState.GameOver:
			CleanUp();
			break;
		default:
			bIsControllable = false;
			break;
		}
	}
}
