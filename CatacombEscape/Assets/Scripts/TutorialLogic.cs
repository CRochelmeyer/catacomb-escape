///
/// GameLogic.cs
/// This seems to be a bit of a 'god class' atm, it does far too many things. 
/// I'd like to split it up into it's components i.e. player.cs, mouseMovement.cs etc. ~ Nick
/// 
/// This script is attached to the main camera in unity.
///

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

// Game Analytics currently tracks the number of levels the player reaches.
using GameAnalyticsSDK;


public class TutorialLogic : MonoBehaviour
{
	#region gameVariables
	[Header("Game Variables")]
	public int standardStamina;
	public int maxStamina;
	public int greenLocIdx;
	public int redLocIdx;
	public int initRedDmg;
	public int greenAmt;
	public int newHandCost;
	#endregion

	#region sprites
	[Header("Sprites")]
	public Sprite[] tileSprite;
	public Sprite[] gridSprite;
	public Sprite eventGreenLrg;
	public Sprite eventGreenSml;
	public Sprite eventRed;
	public Sprite[] eventEnemy;
	#endregion

	public AudioSource audioSource;
	#region audioClips
	[Header("Audio Clips")]
	public AudioClip startGameClip;
	public AudioClip chestPlacementClip;
	public AudioClip noPlacementClip;
	public AudioClip[] placementClips;
	public AudioClip[] dealingClips;
	public AudioClip[] movementClips;
	public AudioClip[] greenTileClips;
	public AudioClip[] redTileClips;
	public AudioClip[] lvlCompClips;
	#endregion

	#region uiPanels
	[Header("UI Panels")]
	public float fadeTime;
	private bool faderRunning = false;
	public GameObject enemyPanel;
	public Text enemyStamDown;
	public Text diamondAmount;
	public GameObject tutCompletePanel;
	public float handTweenWait;
	public Transform handTileSource;
	#endregion

	#region stamPanels
	[Header("Stamina Panels")]
	public Transform stamPopupsContainer;
	public GameObject stamUpPrefab;
	public GameObject stamDownPrefab;
	#endregion

	#region removeTileFunctions
	[Header("Remove Tile Function")]
	public Button newHandButton;
	public Button removeTileButton;
	public Image removeTileImage;
	public Sprite pickaxeSprite;
	public Sprite backArrowSprite;
	public ManageRemoveTile manageRemoveTileScript;
	#endregion

	#region tutorialPanels
	[Header("Tutorial Panels")]
	public GameObject[] arrows;
	private int arrowIdx = 0;
	public GameObject[] tileOverlays;
	#endregion

	//boolean game conditions
	public bool displayingEvent { get; set; }
	public bool mouseClicked { get; set; }
	public bool exiting { get; set; }
	private bool emptyhand = true;
	private bool nextlevel = false;
	private int playerStamina;
	private string playerLoc="";
	private string destLoc = "";
	private string mouseLocation = "";
	private bool deletingTile = false;

	//tutorial conditions
	private GameObject handTile0 = null;
	private GameObject handTile1 = null;
	private GameObject handTile2 = null;
	private GameObject handTile3 = null;
	private int feedHand = 0;
	private string[] placementLocations = new string[] {"10", "11", "21", "20", "22", "22"};
	private int placementIndex = 0;
	private int stage = 1;

	//dictionary to match cell strings of 00-04 10-14 to an index from 0-29
	Dictionary<string, int> cellindex = new Dictionary<string, int>();
	//dictionary to store event tile clone name and original grid panel location int
	Dictionary<string, string> eventindex = new Dictionary<string, string>();
	//sprite holders drag sprites via inspector

	public GameObject btmPanel;
	public GameObject[] handTiles;
	private GameObject[] handTilesDrag;
	private string exit;
	//initiate a static instance of gamelogic to be used globally...
	private static TutorialLogic instance = null;

	private int level = 1;
	private GridPanelsTutorial gridPanelsScript;
	private Direction validMove;
	private PlayerMove movePlayer;
	private PathFinder Pathing;
	private CoinController coinCont;
	private EnemyController enemyCont;
	private GameObject[] gridPanels;
	private Tile[,] tileBoard;

	// Init, update, awake etc.
	#region General Game Functions

	//awake called behind start
	void Awake()
	{
		exiting = false;
		displayingEvent = false;
		mouseClicked = false;

		if (PlayerPrefs.HasKey ("Diamonds"))
		{
			diamondAmount.text = PlayerPrefs.GetInt ("Diamonds").ToString();
		}

		PlayerPrefs.SetString ("TutorialScene", "true");
		audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource> ();
		audioSource.PlayOneShot (startGameClip, 0.5f);

		GameObject mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		if (mainCamera != null)
			mainCamera.GetComponent<BackGroundMusic> ().ResetScript ();

		Pathing = GameObject.FindGameObjectWithTag("Scripts").GetComponent<PathFinder>(); //PathFinder.cs
		validMove = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<Direction> ();
		movePlayer = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<PlayerMove> ();
		coinCont = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<CoinController> ();
		enemyCont = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<EnemyController> ();

		playerStamina = standardStamina;
		UpdateUI();

		newHandButton.interactable = false;
		removeTileButton.interactable = false;

		InitLevel(level);

		// Stage 1: Tile 1
		arrows[0].SetActive (true);
		tileOverlays[0].SetActive (true);
		tileOverlays[1].SetActive (true);
		tileOverlays[3].SetActive (true);
	}

	// Initialize the level with enemies, loot and entrance/exit tiles
	void InitLevel(int pLevel)
	{
		int temp = 0;

		//fill cellindex dictionary
		//temp index int to fill dictonary
		cellindex.Clear();
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				cellindex.Add(i.ToString() + j.ToString(), temp);
				temp++;
			}
		}
		//if instance is null create instance of this GameLogic 
		if (instance == null)
		{
			instance = this;
		}
		//else instance is not null but not this GameLogic destroy 
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		FindGridPanels();

		PlayerPrefs.SetString ("Paused", "false");

		tileBoard = new Tile[4, 3];
		for (int i =0; i<4;i++)
		{
			for (int j =0; j<3;j++)
			{
				tileBoard[i, j] = new Tile(0);
			}
		}

		//setup board with rng sprites
		GenerateBoard();

		GenerateBoardLogic();
	}

	// Update is called once per frame
	void Update()
	{
		if (PlayerPrefs.GetString ("Paused") != "true")
		{
			//check if handTiles has been filled
			if (emptyhand == true)
			{
				if (feedHand == 0)
				{
					// Generate hand with "right,left", "down,left", "up,right,down" and "up, right, left"
					GenerateHand (3, 2, 8, 10);
					feedHand++;
					emptyhand = false;
					handTile2.AddComponent<Draggable>();
				}
				else if (feedHand == 1)
				{
					// Generate hand with "up, right", "up,right,down", "right,left" and "up,down"
					GenerateHand (7, 8, 3, 4);
					feedHand++;
					emptyhand = false;
				}
			}

			if (deletingTile)
			{
				// Needs to run in update to check for the mouse click.
				DeleteTile();
			}
			else// if (mouseClicked == false)
			{
				PlayerClick();				
			}


			if (nextlevel)
			{
				PlayerPrefs.SetString ("Paused", "true");
				tutCompletePanel.SetActive (true);
			}
		}
	}

	private void UpdateStage ()
	{
		switch (stage)
		{
		case 2:	// Stage 2: Tile 2
			NextArrow ();
			tileOverlays[1].SetActive (false);
			break;
		case 3:	// Stage 3: Tile 3
			NextArrow ();
			tileOverlays[3].SetActive (false);
			break;
		case 4:	// Stage 4: Tile 4
			NextArrow ();
			tileOverlays[0].SetActive (false);
			break;
		case 5:	// Stage 5: Move to chest
			NextArrow ();
			tileOverlays[0].SetActive (true);
			tileOverlays[1].SetActive (true);
			tileOverlays[2].SetActive (true);
			tileOverlays[3].SetActive (true);
			break;
		case 6:	// Stage 6: Tile 5
			NextArrow ();
			tileOverlays[3].SetActive (false);
			break;
		case 7:	// Stage 7: Remove tile 5
			removeTileButton.interactable = true;
			NextArrow ();
			break;
		case 8:	// Stage 8: Discard hand
			NextArrow ();
			newHandButton.interactable = true;
			break;
		case 9:	// Stage 9: Tile 6
			NextArrow ();
			tileOverlays[3].SetActive (true);
			tileOverlays[2].SetActive (false);
			break;
		case 10: // Stage 10: Move to exit
			NextArrow ();
			break;
		}
	}

	/// <summary>
	/// Replenishes player stamina when stamina reaches 0. 
	/// </summary>
	private void CheckStamina()
	{
		if (playerStamina <= 0)
		{
			playerStamina = 30;
			coinCont.UpdateCoins (30, playerLoc);
			UpdateUI();
			// Display click panel: Haha, very funny. Get moving!
		}
	}

	#endregion

	#region Tutorial Functions

	private void NextArrow ()
	{
		arrows[arrowIdx].SetActive (false);
		arrowIdx++;
		arrows[arrowIdx].SetActive (true);
	}

	private void SetNextDraggable()
	{
		switch (placementIndex)
		{
		case 0:
			handTile2.AddComponent<Draggable>();
			break;
		case 1:
			handTile1.AddComponent<Draggable>();
			break;
		case 2:
			handTile3.AddComponent<Draggable>();
			break;
		case 3:
			handTile0.AddComponent<Draggable>();
			break;
		}
	}

	#endregion

	#region Player Functions


	// Called by PlayerMove once the character animation is complete, to stop events from occuring before player has stopped on the destination tile
	public void SetPlayerLoc()
	{
		playerLoc = destLoc;
		//play event for event tiles    
		if (tileBoard[System.Int32.Parse(playerLoc.Substring(0, 1)), System.Int32.Parse(playerLoc.Substring(1, 1))]._event != "")
		{
			PlayEvent(playerLoc);
		}

		if (playerLoc == "20")
		{
			stage++;
			UpdateStage ();
			handTile3.AddComponent<Draggable>();
		}

		//check if next level...
		if (playerLoc == exit)
		{
			nextlevel = true;
		}else
		{
			//move events
			MoveEvents();		
		}
	}

	//overload for above with a passed in player location
	public void SetPlayerLoc(string loc)
	{
		//udate occupied tile
		validMove.UpdateOccupiedTile(tileBoard[GetRow(playerLoc), GetCol(playerLoc)], tileBoard[GetRow(loc), GetCol(loc)]);
		// update the player's location
		playerStamina--;
		InstantiateStamDownPanel("-1", movePlayer.PlayerLocation);
		UpdateUI();

		playerLoc = loc;

		int idxA = System.Int32.Parse(playerLoc.Substring(0, 1));
		int idxB = System.Int32.Parse(playerLoc.Substring(1, 1));

		//play event for event tiles    
		if (tileBoard [idxA, idxB]._event != "")
		{
			PlayEvent(playerLoc);
		}

		if (playerLoc == "20")
		{
			stage++;
			UpdateStage ();
			handTile3.AddComponent<Draggable>();
		}

		//check if next level...
		if (playerLoc == exit)
		{
			nextlevel = true;
		}
		else
		{
			//move events
			MoveEvents();
			CheckStamina();
		}
	}

	// Initialise player data
	public void InitPlayer()
	{
		CheckStamina();
		if (playerStamina < standardStamina)
		{
			int stamUpAmt = standardStamina - playerStamina;
			string stamUpStr = stamUpAmt.ToString();
			coinCont.UpdateCoins (stamUpAmt, playerLoc);
			Vector3 playerPos = GetGridPanelPosition (playerLoc);
			InstantiateStamUpPanel (stamUpStr, playerPos);
			playerStamina = standardStamina;
			UpdateUI();
		}
	}

	#endregion

	#region Mouse Functions

	// ~Mouse related method
	public string MouseLocation
	{
		get
		{
			UpdateMouseLocation();
			return mouseLocation;
		}
		set { mouseLocation = value; }
	}

	// ~Mouse related method
	private void UpdateMouseLocation()
	{
		bool foundMouse = false;
		int i = 0;
		while (!foundMouse && i < 12)
		{
			foundMouse = gridPanels[i].GetComponent<Panel>().MouseOverPanel();
			i++;
		}
	}

	/// <summary>
	/// Gets location of mouse click.
	/// </summary>
	/// <param name="pv3"></param>
	/// <returns>Click Location</returns>
	private string GetClickLocation(Vector3 pv3)
	{
		string location = "invalid location";
		bool foundMouse = false;
		int i = 0;

		if (stage == 5) // move to chest
		{
			i = 6;
		}
		else if (stage == 10) // move to exit
		{
			i = 11;
		}

		if (!foundMouse && (i == 6 || i == 11))
		{
			location = gridPanels[i].GetComponent<Panel>().MouseClickPanel(pv3);
		}
		return location;
	}

	/// <summary>
	/// After tile has completed its dragging action, place pTile in location pCell
	/// </summary>
	/// <param name="ptile">Ptile.</param>
	/// <param name="pcell">Pcell.</param>
	public void UpdateDrag( Tile ptile , string pcell)
	{
		//grab index based on pcell in cellindex dictionary
		int _gridIndex = 33;
		int _spriteIndex = 0;
		cellindex.TryGetValue(pcell, out _gridIndex);
		//grab corresponding gridsprite[index] based on ptile
		for (int i = 0; i < tileSprite.Length; i++)
		{
			string temp1 = tileSprite[i].name.ToString();
			string temp2 = ptile._tileID.ToString();

			if (string.Compare (temp1, temp2) == 0 && _gridIndex != 33 )
			{
				_spriteIndex = i;
				gridPanels [_gridIndex].GetComponent<Image>().sprite = tileSprite [_spriteIndex] as Sprite;
				gridPanels [_gridIndex].GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);

				//update tileBoard
				if (tileBoard [System.Int32.Parse (pcell.Substring (0, 1)), System.Int32.Parse (pcell.Substring (1, 1))]._event != "") // if cell has an event on it
				{
					tileBoard [System.Int32.Parse (pcell.Substring (0, 1)), System.Int32.Parse (pcell.Substring (1, 1))].UpdateTile (ptile);
					if (tileBoard [System.Int32.Parse (pcell.Substring (0, 1)), System.Int32.Parse (pcell.Substring (1, 1))]._event == "green")
					{
						GameObject temp = GameObject.Find (tileBoard [System.Int32.Parse (pcell.Substring (0, 1)), System.Int32.Parse (pcell.Substring (1, 1))]._boardLocation + "(Clone)");
						temp.GetComponent <Image>().sprite = eventGreenSml as Sprite; 

						audioSource.PlayOneShot (chestPlacementClip);
					}
					else if (tileBoard [System.Int32.Parse (pcell.Substring (0, 1)), System.Int32.Parse (pcell.Substring (1, 1))]._event == "red")
					{
						GameObject temp = GameObject.Find (tileBoard [System.Int32.Parse (pcell.Substring (0, 1)), System.Int32.Parse (pcell.Substring (1, 1))]._boardLocation + "(Clone)");
						temp.GetComponent <Image>().sprite = eventEnemy [Random.Range (0, eventEnemy.Length)] as Sprite; 

						int rand = Random.Range (0,placementClips.Length);
						audioSource.PlayOneShot (placementClips[rand]);
					}
				}
				else
				{
					tileBoard [System.Int32.Parse (pcell.Substring (0, 1)), System.Int32.Parse (pcell.Substring (1, 1))] = ptile;

					int rand = Random.Range (0,placementClips.Length);
					audioSource.PlayOneShot (placementClips[rand]);
				}

				GameObject tempObj = GameObject.Find (tileBoard [System.Int32.Parse (pcell.Substring (0, 1)), System.Int32.Parse (pcell.Substring (1, 1))]._boardLocation);

				//decrease stamina
				playerStamina -= 2;
				InstantiateStamDownPanel ("-2", tempObj.transform.position);

				stage++;
				UpdateStage ();
				UpdateUI();
				coinCont.UpdateCoins (-2, tempObj.name);
				break;
			}
		}
		//check if hand is empty (one remaining to be destroyed), draw it using tag handDrag
		handTilesDrag = GameObject.FindGameObjectsWithTag("handDrag");

		if (handTilesDrag.Length == 1)
		{
			//assign emptyhand bool
			emptyhand = true;
		}
	}

	/// <summary>
	/// Handles player movement
	/// </summary>
	public void PlayerClick()
	{
		//check for left click
		if(Input.GetMouseButtonDown(0))
		{
			string clickLoc = "";			

			clickLoc = MouseLocation;

			if (clickLoc != "")
			{
				string dest = "";

				if (stage == 5) // move to chest
				{
					dest = "20";
				}
				else if (stage == 10) // move to exit
				{
					dest = "32";
				}

				if (clickLoc == dest)
				{
					int temprow = System.Int32.Parse(clickLoc.Substring(0, 1));
					int tempcol = System.Int32.Parse(clickLoc.Substring(1, 1));

					// Check that the target tile was player-placed.
					if ((tileBoard[temprow, tempcol]._isEntrySet) && (playerLoc != ""))
					{
						mouseClicked = true;

						// Check for a path
						List<string> path = Pathing.PathFind(tileBoard, playerLoc, clickLoc);
						//if ( (!path.Contains("invalid") || !path.Contains("Invalid")) && Pathing.CheckExitIsLast(path))
						if ((!path.Contains("invalid") || !path.Contains("Invalid")))
						{
							//Pathing.PrintPathTiles ();
							destLoc = clickLoc;
							movePlayer.UpdatePlayer(gridPanels, path, tileBoard);
						}
						else
						{
							mouseClicked = false;
							Debug.Log("Path is invalid");
						}
					}
					else
					{
						mouseClicked = false;
						Debug.Log("Invalid player move");
					}
				}
			}
			Debug.Log("mouseClicked: " + mouseClicked);
		}
	}

	#endregion

	#region Level Functions

	public bool SetNextLevel
	{
		set {nextlevel = value;}
	}

	// ~Level related method
	public int GetLevel
	{
		get {return level;}
	}

	#endregion

	// Events are the enemies and loot tiles
	#region Event Functions

	/// <summary>
	/// Handles movement of the red event tiles
	/// </summary>
	public void MoveEvents()
	{
		string etloc = "";
		string newloc = "";
		int panelkey = 0;
		int currow = 0;
		int curcol = 0;
		int newcol = 0;

		GameObject[] eventTiles = GameObject.FindGameObjectsWithTag("eventTile");
		for(int i =0; i<eventTiles.Length;i++)
		{
			// For red tiles only. Green tiles don't move.
			if (eventTiles[i].GetComponent<Image>().sprite.name.Substring (0, 5) == "enemy")
			{
				etloc = eventTiles[i].name.Substring(0, 2);
				System.Int32.TryParse(etloc.Substring(0, 1), out currow);
				System.Int32.TryParse(etloc.Substring(1, 1), out curcol);
				newcol = Mathf.Abs(curcol - 1);

				Tile dummy = new Tile(0);

				newloc = currow.ToString() + newcol.ToString();
				cellindex.TryGetValue(newloc, out panelkey);

				if ((tileBoard[currow, newcol]._tileID != "tile_exit") && (tileBoard[currow, newcol]._tileID != "tile_entrance") && (newloc != playerLoc) && (tileBoard[currow, newcol]._event != "green") && (tileBoard[currow, newcol]._event != "red"))
				{
					//move thetile
					eventTiles[i].transform.localPosition = gridPanels[panelkey].transform.localPosition;
					//update tileboard
					//clone the current tile to dummy
					dummy.CloneTile(tileBoard[currow, curcol]);
					//set current tile event htats moving to no event
					tileBoard[currow, curcol].ClearEvent();
					//flush dummy entry if it is set from the previous tile cloned
					if (dummy._isEntrySet)
					{
						dummy.FlushEntry();
					}
					//update the new board position with the dummy clone
					tileBoard[currow, newcol].UpdatePosition(dummy);
					//update objclone name to be used for destroying the game obj
					eventTiles[i].name = newloc + "(Clone)";
				}
			}
		}
	}

	public Vector3 GetGridPanelPosition (string panelName)
	{
		for (int i = 0; i < gridPanels.Length; i++)
		{
			if (gridPanels[i].name == panelName)
			{
				return gridPanels[i].transform.position;
			}
		}

		return gridPanels[0].transform.position;
	}

	public bool PlacementValid (string cell)
	{
		if (placementLocations [placementIndex] == cell)
		{
			placementIndex++;
			SetNextDraggable();
			return true;
		}
		else
			return false;
	}

	/// <summary>
	/// Updates events. Deals damage etc.
	/// </summary>
	/// <param name="pcell"></param>
	public void PlayEvent(string pcell)
	{
		if (tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))]._isActive)
		{
			int temprow = System.Int32.Parse(pcell.Substring(0, 1));
			int tempcol = System.Int32.Parse(pcell.Substring(1, 1));

			Tile tempTile = tileBoard[temprow, tempcol];
			GameObject tempObj = GameObject.Find(tempTile._boardLocation + "(Clone)");

			//play event = remove stamina and destroy the event clone...
			if (tempTile._event == "red")
			{
				//stamUp.text = tempTile.combat.ToString();
				//stamUp.enabled = true;

				int damage = tempTile.combat;

				// Prevent player gaining stamina from enemy. Don't want no eating of strange creatures...
				if (damage > 0)
				{
					damage = 0;
				}

				enemyStamDown.text = damage.ToString();
				DisplayClickPanel (enemyPanel);

				if (damage != 0) //don't show stam popup if stamina is not reduced
				{
					string newText = damage.ToString();
					InstantiateStamDownPanel (newText, tempObj.transform.position);
					playerStamina += damage;
					UpdateUI();
					coinCont.UpdateCoins (damage, tempTile._boardLocation);
				}

				int rand = Random.Range (0,redTileClips.Length);
				audioSource.PlayOneShot (redTileClips[rand]);
			}
			else if (tempTile._event == "green")
			{
				string newText = "+" + tempTile.combat.ToString();
				InstantiateStamUpPanel (newText, tempObj.transform.position);
				playerStamina += tempTile.combat;
				coinCont.UpdateCoins (tempTile.combat, tempTile._boardLocation);

				int rand = Random.Range (0,greenTileClips.Length);
				audioSource.PlayOneShot (greenTileClips[rand]);
			}

			if (tempObj != null)
			{
				Debug.Log("destroy clone");
				Destroy(tempObj);
				tempTile._isActive = false;
			}
		}
	}

	#endregion

	#region Board Functions

	/// <summary>
	/// 
	/// </summary>
	public void GenerateBoardLogic()
	{
		Tile temptile;
		int temp = 0;
		//if tileboard is not empty
		if (tileBoard.Length != 0)
		{
			//generate tileboard...
			for (int row = 0; row < 4; row++)
			{
				for (int col = 0; col < 3; col++)
				{
					cellindex.TryGetValue(row.ToString() + col.ToString(), out temp);

					if (gridPanels[temp].GetComponent<Image>().sprite != null)
					{
						temptile = new Tile(gridPanels[temp].GetComponent<Image>().sprite.name.ToString(), row.ToString() + col.ToString());
						tileBoard[row, col] = temptile;
					}
				}
			}
			//add adding eventitems into event tiles...
			foreach (KeyValuePair<string, string> pair in eventindex)
			{
				string tempstring = pair.Key.ToString();
				string _eventitem = "";
				int temprow = System.Int32.Parse(tempstring.Substring(0, 1));
				int tempcol = System.Int32.Parse(tempstring.Substring(1, 1));
				//temptile = new Tile(pair.Value, pair.Key);
				if (pair.Value == "event_green")
				{
					//initialy had _eventitems and playerequpment++ in the eventitle creation however
					//that would mix up the linear advancement order thus do it within playevents.
					_eventitem = "empty";
				}
				else if (pair.Value == "event_red")
				{
					if (Random.Range(0, 2) == 0)
					{
						_eventitem = "scorpion";
					}
					else
					{
						_eventitem = "snake";
					}
				}

				// Populate the board with event tiles ??
				temptile = new EventTile(pair.Value, pair.Key, _eventitem);
				tileBoard[temprow, tempcol] = temptile;
			}
			//clear eventindex
			eventindex.Clear();
		}
		//check tileBoard exists
		if (tileBoard.Length != 0)
		{
			//check if next level is available exit = bottom of the grid 50-54
			for (int row = 3; row < 4; row++)
			{
				for (int col = 0; col < 3; col++)
				{
					//if tile within bottom row matches exit name
					if (tileBoard[row, col]._tileID == "tile_exit")
					{
						exit = row.ToString() + col.ToString();
					}
				}
			}
			//find entrance and set player location
			for (int row = 0; row < 1; row++)
			{
				for (int col = 0; col < 3; col++)
				{
					if (tileBoard[row, col]._tileID == "tile_entrance")
					{
						tileBoard[row, col]._isOccupied = true;
						playerLoc = row.ToString() + col.ToString();
						//draw player
						int pindex = 0;
						cellindex.TryGetValue (playerLoc, out pindex);
						movePlayer.DrawPlayer (pindex, gridPanels);
						int rand = Random.Range (0,movementClips.Length);
						audioSource.PlayOneShot (movementClips[rand], 1.0f);
					}
				}
			}
		}
	}

	/// <summary>
	/// Generates the game board including the number of green and red event tiles.
	/// </summary>
	public void GenerateBoard()
	{
		// Generate a number of green tiles.
		int green = 1;

		// Generate the number of red tiles
		int red = 1;

		if (gridPanels != null)
		{
			GameObject gridPanelsParent = gridPanels[0].transform.parent.gameObject;

			//exit tile location will be in the bottom right
			int downPanel = 11;
			//entrance tile location will be in the top left
			int upPanel = 0;

			//set exit tile
			gridPanels[downPanel].GetComponent<Image>().sprite = gridSprite[1] as Sprite;
			gridPanels[downPanel].GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
			//set entrance tile
			gridPanels[upPanel].GetComponent<Image>().sprite = gridSprite[2] as Sprite;
			gridPanels[upPanel].GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);

			//store event green tile into eventgreen list
			eventindex.Clear();

			//Draw all green tiles
			for (int i = 0; i < green; i++)
			{
				GameObject tempPanel = gridPanels[greenLocIdx];
				GameObject panelClone = Instantiate(tempPanel);
				panelClone.transform.SetParent(gridPanelsParent.transform);
				panelClone.tag = "eventTile";
				panelClone.transform.localPosition = tempPanel.transform.localPosition;
				panelClone.transform.localScale = new Vector3(1, 1, 1);
				panelClone.GetComponent<Image>().sprite = eventGreenLrg as Sprite;
				panelClone.GetComponent<Image>().color = new Color(255f, 255f, 255f, 150f);
				eventindex.Add(tempPanel.name, "event_green");

			}

			//Draw all red tiles
			for (int i = 0 + green; i < red + green; i++)
			{
				GameObject tempPanel = gridPanels[redLocIdx];
				GameObject panelClone = Instantiate(tempPanel);
				panelClone.transform.SetParent(gridPanelsParent.transform);
				panelClone.tag = "eventTile";
				panelClone.transform.localPosition = tempPanel.transform.localPosition;
				panelClone.transform.localScale = new Vector3(1, 1, 1);
				panelClone.GetComponent<Image>().sprite = eventEnemy [Random.Range (0, eventEnemy.Length)] as Sprite;
				panelClone.GetComponent<Image>().color = new Color(255f, 255f, 255f, 150f);

				//store event red tiles into eventred list
				eventindex.Add(tempPanel.name, "event_red");

				enemyCont.TutorialSetMovement (panelClone, "left");
			}
		}
	}

	private void FindGridPanels()
	{
		gridPanelsScript = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<GridPanelsTutorial> ();
		gridPanels = new GameObject[12];

		for (int i = 0; i < gridPanels.Length; i++)
		{
			gridPanels[i] = gridPanelsScript.GetGridPanel(i);
		}
	}

	#endregion

	#region Hand Functions

	/// <summary>
	/// Generate a new hand of tiles
	/// </summary>
	public void NewHand()
	{
		handTilesDrag = GameObject.FindGameObjectsWithTag("handDrag");
		if (handTilesDrag != null)
		{
			for (int i = 0; i < handTilesDrag.Length; i++)
			{
				Destroy (handTilesDrag[i]);
			}
		}
		InstantiateStamDownPanel ("-" + newHandCost, movePlayer.PlayerLocation);
		playerStamina += - newHandCost;

		// Generate hand with "up,down", "right,left", "up,down,left" and "up, right, left"
		GenerateHand (4, 3, 5, 10);
		feedHand++;
		emptyhand = false;

		stage++;
		UpdateStage ();
		UpdateUI();
		coinCont.UpdateCoins (-newHandCost, "newHandButton");

		newHandButton.interactable = false;

		handTile2.AddComponent<Draggable>();
	}

	public void GenerateHand(int idx0, int idx1, int idx2, int idx3)
	{       
		//approaching hand generation via grabbing each individual UI element and updating the sprite image and render...didnt work out 13/04
		//actaullyworking just rendered tiny and behind default image too...13/04
		//handTiles = GameObject.FindGameObjectsWithTag ("handDefault");
		//btmPanel = GameObject.FindGameObjectWithTag ("bottomPanel");
		//check for null
		if (handTiles != null)
		{
			int rand = Random.Range (0, dealingClips.Length);
			audioSource.PlayOneShot (dealingClips[rand], 1.0f);

			handTilesDrag = new GameObject[4];

			// SET TILE #1
			handTile0 = Instantiate (handTiles[0]);
			handTile0.transform.localScale = handTiles [0].transform.localScale;
			handTile0.transform.localPosition = handTileSource.localPosition;
			//set tag so handTiles above doesnt grab clones as well.
			handTile0.tag = "handDrag";
			//assign new object correct parents
			handTile0.transform.SetParent (btmPanel.transform, false);
			handTile0.GetComponent <Image>().sprite = tileSprite [idx0] as Sprite;
			handTile0.GetComponent <Image>().color = new Color(255f,255f,255f,255f);
			handTilesDrag [0] = handTile0;

			// SET TILE #2
			handTile1 = Instantiate (handTiles[1]);
			handTile1.transform.localScale = handTiles [1].transform.localScale;
			handTile1.transform.localPosition = handTileSource.localPosition;
			//set tag so handTiles above doesnt grab clones as well.
			handTile1.tag = "handDrag";
			//assign new object correct parents
			handTile1.transform.SetParent (btmPanel.transform, false);
			handTile1.GetComponent <Image>().sprite = tileSprite [idx1] as Sprite;
			handTile1.GetComponent <Image>().color = new Color(255f,255f,255f,255f);
			handTilesDrag [1] = handTile1;

			// SET TILE #3
			handTile2 = Instantiate (handTiles[2]);
			handTile2.transform.localScale = handTiles [2].transform.localScale;
			handTile2.transform.localPosition = handTileSource.localPosition;
			//set tag so handTiles above doesnt grab clones as well.
			handTile2.tag = "handDrag";
			//assign new object correct parents
			handTile2.transform.SetParent (btmPanel.transform, false);
			handTile2.GetComponent <Image>().sprite = tileSprite [idx2] as Sprite;
			handTile2.GetComponent <Image>().color = new Color(255f,255f,255f,255f);
			handTilesDrag [2] = handTile2;

			// SET TILE #4
			handTile3 = Instantiate (handTiles[3]);
			handTile3.transform.localScale = handTiles [3].transform.localScale;
			handTile3.transform.localPosition = handTileSource.localPosition;
			//set tag so handTiles above doesnt grab clones as well.
			handTile3.tag = "handDrag";
			//assign new object correct parents
			handTile3.transform.SetParent (btmPanel.transform, false);
			handTile3.GetComponent <Image>().sprite = tileSprite [idx3] as Sprite;
			handTile3.GetComponent <Image>().color = new Color(255f,255f,255f,255f);
			handTilesDrag [3] = handTile3;

			StartCoroutine (TweenDragTiles (handTilesDrag));
		}
	}

	IEnumerator TweenDragTiles (GameObject[] dragTiles)
	{
		int rand = Random.Range(0, dealingClips.Length);
		audioSource.PlayOneShot(dealingClips[rand]);

		for (int i = 0; i < dragTiles.Length; i++)
		{
			if (dragTiles[i] != null)
				dragTiles[i].GetComponent <HandTileTween>().enabled = true;
			yield return new WaitForSeconds (handTweenWait);
		}
	}

	#endregion

	#region UI Functions

	// ~UI related
	/// <summary>
	/// Updates Player Stamina text and Stamina bar.
	/// </summary>
	public void UpdateUI()
	{
		if (playerStamina > maxStamina )
		{
			playerStamina = maxStamina;
		}else if (playerStamina < 0)
		{
			playerStamina = 30;
		}
		GameObject tempObj = GameObject.FindGameObjectWithTag("PlayerStam");
		tempObj.GetComponent<Text>().text = playerStamina.ToString();
	}

	/// <summary>
	/// Enables pause panel UI.
	/// </summary>
	/// <param name="panel"></param>
	private void DisplayClickPanel (GameObject panel)
	{
		displayingEvent = true;
		panel.SetActive (true);
		PlayerPrefs.SetString ("Paused", "true");
		StartCoroutine (ClickToClose (panel));
	}

	/// <summary>
	/// Closes the pause panel UI.
	/// </summary>
	/// <param name="panel"></param>
	/// <returns></returns>
	IEnumerator ClickToClose (GameObject panel)
	{
		while (!Input.GetMouseButtonUp (0))
		{
			yield return null;
		}
		panel.SetActive (false);
		displayingEvent = false;
		PlayerPrefs.SetString ("Paused", "false");
	}

	private void InstantiateStamUpPanel (string newText, Vector3 panelPos)
	{
		GameObject stamUpPanel = (GameObject) Instantiate (stamUpPrefab);
		Transform stamUpTrans = stamUpPanel.GetComponent <Transform> ();
		stamUpTrans.SetParent (stamPopupsContainer, false);
		stamUpTrans.position = panelPos;
		Text stamUpText = stamUpTrans.Find ("StamUp").gameObject.GetComponent <Text> ();
		if (stamUpText != null)
			StartFade (stamUpText, newText);
		else
			Debug.Log ("Couldn't find StamUp text component");
	}

	private void InstantiateStamDownPanel (string newText, Vector3 panelPos)
	{
		GameObject stamDownPanel = (GameObject) Instantiate (stamDownPrefab);
		Transform stamDownTrans = stamDownPanel.GetComponent <Transform> ();
		stamDownTrans.SetParent (stamPopupsContainer, false);
		stamDownTrans.position = panelPos;
		Text stamDownText = stamDownTrans.Find ("StamDown").gameObject.GetComponent <Text> ();
		if (stamDownText != null)
			StartFade (stamDownText, newText);
		else
			Debug.Log ("Couldn't find StamDown text component");
	}

	private void StartFade (Text stamText, string newText)
	{
		if (faderRunning)
		{
			StopCoroutine ("FadeStamPopup");
		}
		Color newColor = stamText.color;
		newColor.a = 1;
		stamText.color = newColor;
		stamText.text = newText;

		StartCoroutine (FadeStamPopup (stamText, fadeTime));
	}

	IEnumerator FadeStamPopup (Text stamText, float time)
	{
		Transform stamTrans = stamText.transform;
		Vector3 stamInitPos = stamTrans.position;
		Vector3 stamFinalPos = new Vector3 (stamInitPos.x, stamInitPos.y + 0.5f, stamInitPos.z);
		faderRunning = true;
		float alpha = stamText.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
		{
			Color newColor = stamText.color;
			newColor.a = Mathf.Lerp (alpha,0,t);
			stamTrans.position = Vector3.Lerp (stamInitPos, stamFinalPos, t);
			stamText.color = newColor;
			yield return null;
		}
		Destroy (stamTrans.parent.gameObject);
		faderRunning = false;
	}

	#endregion

	#region Remove Tile Functions

	/// <summary>
	/// Toggles the ability to delete tiles.
	/// </summary>
	public void DeleteTileToggle()
	{
		deletingTile = !deletingTile;

		if (deletingTile)
		{
			Debug.Log("Selecting tile to delete.");
			newHandButton.interactable = false;
			removeTileButton.interactable = false;
			removeTileImage.sprite = backArrowSprite;

			NextArrow ();
			manageRemoveTileScript.DisplayOverlays ("33");
		}
		else
		{
			Debug.Log("No longer deleting tile.");
			removeTileButton.interactable = false;
			removeTileImage.sprite = pickaxeSprite;
			manageRemoveTileScript.HidePanelOverlays();
			stage++;
			UpdateStage ();
		}
	}

	/// <summary>
	/// Performs the deletion of a tile.
	/// This is done by creating a blank tile to replace the one being "deleted".
	/// </summary>
	private void DeleteTile()
	{
		string clickLoc;

		clickLoc = MouseLocation;

		// If mouse has been clicked
		if (Input.GetMouseButtonDown(0) && PlayerPrefs.GetString ("Paused") == "false")
		{
			if (clickLoc != "" && clickLoc == "22")
			{
				// row and column of tile that was clicked.
				int temprow = System.Int32.Parse(clickLoc.Substring(0, 1));
				int tempcol = System.Int32.Parse(clickLoc.Substring(1, 1));

				// Ensure only player-placed tiles can be deleted.
				if ( tileBoard[temprow, tempcol]._isEntrySet ) 
				{
					// Prevent deletion if the tile is an exit or entry.
					if (tileBoard[temprow, tempcol] != null && !tileBoard[temprow, tempcol]._tileID.Contains("exit") && !tileBoard[temprow, tempcol]._tileID.Contains("entrance") )
					{
						// Ensure player is not on the target tile.
						if (tileBoard[System.Int32.Parse(playerLoc.Substring(0, 1)), System.Int32.Parse(playerLoc.Substring(1, 1))] != tileBoard[temprow, tempcol])
						{

							int pIndex;                        
							Debug.Log("Removing tileBoard object at " + "[" + temprow + "," + tempcol + "]");
							Debug.Log("Tile ID: " + tileBoard[temprow,tempcol]._tileID);

							// The sprite for the tile is in a separate array for some reason.
							// It's also a one dimensional arraym, so we must retrieve the correct index.
							cellindex.TryGetValue(tileBoard[temprow, tempcol]._boardLocation, out pIndex);

							// Now we can delete the sprite for the tile.
							gridPanels[pIndex].GetComponent<Image>().sprite = null;
							gridPanels[pIndex].GetComponent<Image>().color = new Color(255f, 255f, 255f, 0f);

							Tile tileToDelete = tileBoard[temprow, tempcol];
							tileBoard[temprow, tempcol] = new Tile(0);

							// Preserve any events on the tile.
							if (tileToDelete._event != "")
							{
								Debug.LogWarning("Tile contains an event.");
								tileBoard[temprow, tempcol]._event = tileToDelete._event;
								tileBoard[temprow, tempcol]._eventItem = tileToDelete._eventItem;
								tileBoard[temprow, tempcol].combat = tileToDelete.combat;

								// Need to reset the sprite for the chest events (It stays small once the tile is removed).
								if (tileBoard[temprow,tempcol]._event == "green")
								{
									Debug.Log ("Attempting to update chest sprite at [" + temprow + "," + tempcol + "]");
									GameObject temp = GameObject.Find(temprow.ToString() + tempcol.ToString() + "(Clone)");
									if (temp != null) 
									{
										temp.GetComponent<Image> ().sprite = eventGreenLrg as Sprite;
									} 
									else 
									{
										Debug.Log(temprow.ToString() + tempcol.ToString() + "(Clone)");
										Debug.Log ("Error: could not find gameobject.");
									}
								}
							}

							// Toggle tile deletion, so player deletes only one tile per button press.
							DeleteTileToggle(); 
						}
					}
					else
					{
						Debug.Log("Invalid target: this tile is an entrance or exit!");
						Debug.Log(tileBoard[temprow,tempcol]._entry);
					}
				}
			}
		}
	}

	#endregion

	#region tools

	public int GetRow(string pstring)
	{
		int num = int.Parse(pstring.Substring(0, 1));
		return num;
	}

	public int GetCol(string pstring)
	{
		int num = int.Parse(pstring.Substring(1, 1));
		return num;
	}

	public Tile GetTile (int currow, int curcol)
	{
		return tileBoard[currow, curcol];
	}

	#endregion
}