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


public class GameLogic : MonoBehaviour
{
	#region gameVariables
	[Header("Game Variables")]
	public int standardStamina;
	public int maxStamina;
	public int maxRedNo;
	public int minRedNo;
	public int redLvlInc; // After X levels, enemy number increases
	public int initRedDmg;
	public int maxGreenNo;
	public int minGreenNo;
	public int greenLvlDecr; // After X levels, loot number decreases
	public int greenAmt;
	public int newHandCost;
	public int removeTileCost;
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
	public Text gameScore;
	public Text diamondAmount;
	// Game Over panels and components
	public GameObject statPanel;
	public Text lvlNoClearedText;
	public Text greenCollectedText;
	public Text redAvoidedText;
	public Text tileNoPlacedText;
	public Text pointTot;
	public GameObject newHighscore;
	public Animator characterDeath;
	// Pause components
	public Text pauseLvlText;
	public Text pauseGreenText;
	public Text pauseRedText;
	public Text pauseTileText;
	public Text pauseTotText;
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
	public GameObject removeTilePopUp;
	public ManageRemoveTile manageRemoveTileScript;
	private bool showRemoveTilePop = true;
    #endregion

    //boolean game conditions
    public bool mouseClicked{ get; set; }
	private bool gameover = false;
	private bool emptyhand = true;
	private bool nextlevel = false;
	private bool exiting = false;
	private int playerStamina;
	private int diamonds;
	private string playerLoc="";
	private string destLoc = "";
	private string mouseLocation = "";
	private bool deletingTile = false;

	//dictionary to match cell strings of 00-04 10-14 to an index from 0-29
	Dictionary<string, int> cellindex = new Dictionary<string, int>();
	//dictionary to store event tile clone name and original grid panel location int
	Dictionary<string, string> eventindex = new Dictionary<string, string>();
	//sprite holders drag sprites via inspector

	private GameObject btmPanel;
	private GameObject[] handTiles;
	private string entrance;
	private string exit;
	//initiate a static instance of gamelogic to be used globally...
	private static GameLogic instance = null;

	private int level = 1;
	private GridPanels gridPanelsScript;
	private Direction validMove;
	private PlayerMove movePlayer;
	private PathFinder Pathing;
	private CoinController coinCont;
	private GameObject[] gridPanels;
	private Tile[,] tileBoard;

	private int tileplaced = 0;
	private int greencol = 0;
	private int redavoid = 0;
	private int redtiles = 0;
	private int redstep;

	// Scoring variables
	private float score;
	private int lvlNoCleared;
	private int greenCollected;
	private int redAvoided;
	private int tileNoPlaced;

	// Init, update, awake etc.
	#region General Game Functions

	//awake called behind start
	void Awake()
	{
        //initialising mouseCLicked false
        mouseClicked = false;
		if (PlayerPrefs.HasKey ("Diamonds"))
		{
			diamonds = PlayerPrefs.GetInt ("Diamonds");
		}
		else
		{
			diamonds = 0;
		}

		PlayerPrefs.SetString ("TutorialScene", "false");

		//refresh and initialse redstep per awake call
		redstep = 0;
		audioSource.PlayOneShot (startGameClip, 0.5f);

		GameObject mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		if (mainCamera != null)
			mainCamera.GetComponent<BackGroundMusic>().ResetScript();

		Pathing = GameObject.FindGameObjectWithTag("Scripts").GetComponent<PathFinder>(); //PathFinder.cs
		validMove = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<Direction> ();  // Direction.cs
		movePlayer = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<PlayerMove> ();
		coinCont = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<CoinController> ();
		//TutorialBehaviour tutorialScript = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<TutorialBehaviour> ();

		playerStamina = standardStamina;
		UpdateUI();

		InitGame(level);
	}

	/// <summary>
	/// Initialise the game, and generate the board.
	/// </summary>
	/// <param name="pLevel"></param>
	void InitGame (int pLevel)
	{
		int temp = 0;

		//fill cellindex dictionary
		//temp index int to fill dictonary
		cellindex.Clear();
		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				cellindex.Add (i.ToString() + j.ToString(), temp);
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
			Destroy (gameObject);
		}

		FindGridPanels();

		PlayerPrefs.SetString ("Paused", "false");

		GameObject tempObj = GameObject.FindGameObjectWithTag ("GameLevel");
		tempObj.GetComponent<Text>().text = "Lvl " + pLevel;

		//setup board with rng sprites
		GenerateBoard();

		tileBoard = new Tile[6, 5];
		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				tileBoard[i, j] = new Tile(0);
			}
		}

		GenerateBoardLogic();
	}

	/// <summary>
	/// Generates a hand if empty, handles level progression (or game over)
	/// Called once per frame.
	/// </summary>
	void Update()
	{
		if (PlayerPrefs.GetString("Paused") != "true")
		{
			//check if handTiles has been filled
			if (emptyhand == true)
			{
				//generate hand
				GenerateHand();
				emptyhand = false;
			}

			if (deletingTile)
			{
				// Needs to run in update to check for the mouse click.
				DeleteTile();
			}
			else if (!exiting)
			{
                if (mouseClicked == false)
                {
                    PlayerClick();
                }
			}

			if (nextlevel)
			{
				GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "Level" + level, level);
				nextlevel = false;
				exiting = false;
				NextLevel();
			}
			else if (gameover)
			{
				GameOverHS();
				statPanel.SetActive(true);
			}
		}
	}

	private int CalculateDecrement()
	{
		int decrement = 0;
		int mod = level % greenLvlDecr; // find if level is a factor of green lvl decrement
		if (mod == 0)
		{
			decrement = level / greenLvlDecr;
		}
		return decrement;
	}

	private int CalculateIncrement()
	{
		int increment = 0;
		int mod = level % redLvlInc; // find if level is a factor of red lvl increment
		if (mod == 0)
		{
			increment = level / redLvlInc;
		}
		return increment;
	}

	/// <summary>
	/// Updates the gamestate (game over) when stamina reaches 0. 
	/// </summary>
	private void CheckStamina()
	{
		if (playerStamina <= 0)
		{
			gameover = true;
		}
	}

	public bool DeletingTile
	{
		get {return deletingTile;}
	}

	#endregion

	#region Player Functions

	/// <summary>
	/// Called by PlayerMove once the character animation is complete, to stop events from occuring before player has stopped on the destination tile.
	/// </summary>
	public void SetPlayerLoc()
	{
		playerLoc = destLoc;
		//play event for event tiles    
		if (tileBoard[System.Int32.Parse(playerLoc.Substring(0, 1)), System.Int32.Parse(playerLoc.Substring(1, 1))]._event != "")
		{
			PlayEvent(playerLoc);

			// Testing to see if this fixes some issues relating to tile removal.
			// Seems to have worked, for now...
			tileBoard [System.Int32.Parse (playerLoc.Substring (0, 1)), System.Int32.Parse (playerLoc.Substring (1, 1))]._event = "";
		}

		//check if next level...
		if (playerLoc == exit)
		{
			int rand = Random.Range(0, movementClips.Length);
			int pindex = 0;
			exiting = true;
			audioSource.PlayOneShot(movementClips[rand], 1.0f);

			// Award diamond to player
			diamonds++;
			UpdateUI();

			// Move player out of level
			cellindex.TryGetValue(playerLoc, out pindex);
			movePlayer.PlayerExits(gridPanels[pindex]);
		}
		else
		{
			//move events
			MoveEvents();
			CheckStamina();
		}
	}

	//overload for above with a passed in player location
	public void SetPlayerLoc(string loc)
	{
		Debug.Log("checking setpL calls");
		//udate occupied tile
		validMove.UpdateOccupiedTile(tileBoard[GetRow(playerLoc),GetCol(playerLoc)], tileBoard[GetRow(loc),GetCol(loc)]);
		int rand = Random.Range(0, movementClips.Length);
		audioSource.PlayOneShot(movementClips[rand], 1.0f);
		// update the player's location
		Debug.Log("PlayerClick destLoc :" + destLoc);
		playerStamina--;
		InstantiateStamDownPanel("-1", movePlayer.PlayerLocation);
		UpdateUI();
        Debug.Log("set player loc start" + loc);
		playerLoc = loc;
		//play event for event tiles    
		if (tileBoard[System.Int32.Parse(playerLoc.Substring(0, 1)), System.Int32.Parse(playerLoc.Substring(1, 1))]._event != "")
		{
			PlayEvent(playerLoc);
		}

		//check if next level...
		if (playerLoc == exit)
		{
			int randNum = Random.Range(0, movementClips.Length);
			int pindex = 0;
			exiting = true;
			audioSource.PlayOneShot(movementClips[randNum], 1.0f);

			// Award diamond to player
			diamonds++;
			UpdateUI();

			// Move player out of level
			cellindex.TryGetValue(playerLoc, out pindex);
			movePlayer.PlayerExits(gridPanels[pindex]);
		}
		//move events
		MoveEvents();
		CheckStamina();
	}

	// Initialise player data
	public void InitPlayer()
	{
		if (playerStamina < standardStamina)
		{
			int stamUpAmt = standardStamina - playerStamina;
			string stamUpStr = stamUpAmt.ToString();
			coinCont.UpdateCoins (stamUpAmt, playerLoc);
			Vector3 playerPos = GetGridPanelPosition (playerLoc);
			InstantiateStamUpPanel ("+" + stamUpStr, playerPos);
			playerStamina = standardStamina;
			UpdateUI();
		}
	}

	#endregion

	#region Mouse Functions

	/// <summary>
	/// Updates mouse location and returns it.
	/// </summary>
	public string MouseLocation
	{
		get{UpdateMouseLocation(); 
			return mouseLocation; }
		set{ mouseLocation = value; }
	}

	/// <summary>
	/// Searches tiles for the mouse.
	/// </summary>
	private void UpdateMouseLocation()
	{
		bool foundMouse = false;
		int i = 0;
		while (!foundMouse && i < 30)
		{
			foundMouse = gridPanels[i].GetComponent<Panel>().MouseOverPanel();
			i++;
		}
	}

	/// <summary>
	/// Gets location of mouse click.
	/// Not sure if used or not.
	/// </summary>
	/// <param name="pv3"></param>
	/// <returns>Click Location</returns>
	private string GetClickLocation(Vector3 pv3)
	{
		string location = "invalid location";
		bool foundMouse = false;
		int i = 0;
		while (!foundMouse && i < 30)
		{
			location = gridPanels[i].GetComponent<Panel>().MouseClickPanel(pv3);
			if (location != "invalid location")
			{
				foundMouse = true;
			}
			i++;
		}
		return location;
	}

	/// <summary>
	/// After tile has completed its dragging action, place pTile in location pCell
	/// </summary>
	/// <param name="ptile">Ptile.</param>
	/// <param name="pcell">Pcell.</param>
	public void UpdateDrag(Tile ptile, string pcell)
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

			if (string.Compare(temp1, temp2) == 0 && _gridIndex != 33)
			{
				_spriteIndex = i;
				gridPanels[_gridIndex].GetComponent<Image>().sprite = tileSprite[_spriteIndex] as Sprite;
				gridPanels[_gridIndex].GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);

				// If the target cell contains an event.
				if (tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))]._event != "") 
				{
					tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))].UpdateTile(ptile);
					if (tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))]._event == "green")
					{
						// Chest sprite is replaced with smaller sprite once a tile has been placed over the top of it.
						GameObject temp = GameObject.Find(tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))]._boardLocation + "(Clone)");
						temp.GetComponent<Image>().sprite = eventGreenSml as Sprite;
					}
					else if (tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))]._event == "red")
					{
						// Enemy sprite is also updated once a tile has been placed on top of it.
						GameObject temp = GameObject.Find(tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))]._boardLocation + "(Clone)");
						temp.GetComponent<Image>().sprite = eventEnemy[Random.Range(0, eventEnemy.Length)] as Sprite;
					}
				}
				else
				{
					tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))] = ptile;
				}

				GameObject tempObj = GameObject.Find(tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))]._boardLocation);

				//Debug.Log("tempObj = " + pcell.Substring(0,1) + " , " + pcell.Substring(1, 1));

				//decrease stamina
				int rand = Random.Range(0, placementClips.Length);
				audioSource.PlayOneShot(placementClips[rand], 0.5f);
				playerStamina -= 2;
				InstantiateStamDownPanel("-2", tempObj.transform.position);
				//increment tileplaced
				tileplaced++;
				UpdateUI();
				coinCont.UpdateCoins (-2, tempObj.name);
				CheckStamina();
				break;
			}
		}
		//check if hand is empty (one remaining to be destroyed), draw it using tag handDrag
		handTiles = GameObject.FindGameObjectsWithTag("handDrag");

		if (handTiles.Length == 1)
		{
			//assign emptyhand bool
			emptyhand = true;
		}
		CheckStamina();
	}

	/// <summary>
	/// Handles player movement when there is a mouse click (or finger press)
	/// </summary>
	public void PlayerClick()
	{
		//check for right click
		if (Input.GetMouseButtonDown(0))
		{
            string clickLoc = "";

			clickLoc = MouseLocation;

			if (clickLoc != "")
			{
				int temprow = System.Int32.Parse(clickLoc.Substring(0, 1));
				int tempcol = System.Int32.Parse(clickLoc.Substring(1, 1));

				// _isEntrySet doesn't mean entry or exit tile. It seems to be true for all tiles.
				if ((tileBoard[temprow, tempcol]._isEntrySet) && (playerLoc != ""))
				{
                    mouseClicked = true;
					if (validMove.MoveDirection(playerLoc, clickLoc) != "invalid move" && validMove.InRange(playerLoc, clickLoc))
					{
						if (validMove.Move(playerLoc, clickLoc, ref tileBoard))
						{
							int rand = Random.Range(0, movementClips.Length);
							audioSource.PlayOneShot(movementClips[rand], 1.0f);

							int tempIndex = 0;
							cellindex.TryGetValue(clickLoc, out tempIndex);
							movePlayer.UpdatePlayer(gridPanels[tempIndex], validMove.MoveDirection(playerLoc, clickLoc));

							// update the player's location
							destLoc = clickLoc;

							playerStamina--;
							InstantiateStamDownPanel("-1", movePlayer.PlayerLocation);
							UpdateUI();
							coinCont.UpdateCoins (-1, playerLoc);
						}
					}
                    else if (validMove.InRange(playerLoc, clickLoc)) // In range, but invalid move.
                    {
                        mouseClicked = false;
                        Debug.Log("Invalid player move");
                    }
                    else // If the click was out of range.Use pathfinding.
                    {
						List<string> path = Pathing.PathFind(tileBoard, playerLoc, clickLoc);
						if ((!path.Contains("invalid") || !path.Contains("Invalid")) && path.Count > 1 )
						{
                            //Debug.Log("Valid Path!");
                            destLoc = clickLoc;
							movePlayer.UpdatePlayer(gridPanels, path, tileBoard);
						}
                        else
                        {
                            mouseClicked = false;
                            Debug.Log("Path is invalid");
                        }
					}
                    mouseClicked = false;
                    Debug.Log("Invalid player move");
                }
				else
				{
                    mouseClicked = false;
					Debug.Log("Invalid player move");
				}
			}
            Debug.Log("mouseClicked: "+ mouseClicked);
        }
	}

	#endregion

	#region Level Functions

	public bool SetNextLevel
	{
		set { nextlevel = value; }
	}

	/// <summary>
	/// Returns the current level.
	/// </summary>
	public int GetLevel
	{
		get { return level; }
	}

	/// <summary>
	/// Destroys GameObjects in current level.
	/// Generation of next level occurs by using Awake(). This also recreates a lot of other objects. Perhaps this can be optimised better?
	/// </summary>
	public void NextLevel()
	{
		//int rand = Random.Range (0,lvlCompClips.Length);
		//audioSource.PlayOneShot (lvlCompClips[rand], 1);

		emptyhand = true;
		level++;

		//initilaise tileBoard list a new
		tileBoard = new Tile[6, 5];

		// Destroy handtiles
		handTiles = GameObject.FindGameObjectsWithTag("handDrag");
		for (int i = 0; i < handTiles.Length; i++)
		{
			Destroy(handTiles[i]);
		}

		// Destroy eventTiles and calculate red tiles avoided
		GameObject[] eventTiles = GameObject.FindGameObjectsWithTag("eventTile");
		for (int i = 0; i < eventTiles.Length; i++)
		{
			redavoid += redtiles - redstep;
			Destroy(eventTiles[i]);
		}

		// Destroy grid tiles
		for (int i = 0; i < gridPanels.Length; i++)
		{
			gridPanels[i].GetComponent<Image>().sprite = null;
			gridPanels[i].GetComponent<Image>().color = new Color(255f, 255f, 255f, 0f);
		}

		// Generate the next level.
		InitGame(level);

	}


	#endregion

	#region Score Functions

	/// <summary>
	/// Displays Highscore
	/// </summary>
	private void GameOverHS()
	{
		CalculateScore();

		lvlNoClearedText.text = lvlNoCleared.ToString();
		greenCollectedText.text = greenCollected.ToString();
		redAvoidedText.text = redAvoided.ToString();
		tileNoPlacedText.text = tileNoPlaced.ToString();
		pointTot.text = score.ToString();

		string hs = PlayerPrefs.GetString("HighScore");
		bool newHS = false;

		if (hs != null && hs != "")
		{
			int highscore = int.Parse(hs);
			if (score > highscore)
			{
				newHS = true;
			}
		}
		else if (score > 0) //A score of 0 doesn't count as a high score the first time played
		{
			newHS = true;
		}

		if (newHS)
		{
			PlayerPrefs.SetString("HighScore", score.ToString());
			PlayerPrefs.SetString("LvlNoCleared", lvlNoClearedText.text);
			PlayerPrefs.SetString("GreenCollected", greenCollectedText.text);
			PlayerPrefs.SetString("RedAvoided", redAvoidedText.text);
			PlayerPrefs.SetString("TileNoPlaced", tileNoPlacedText.text);
			newHighscore.SetActive(true);
		}

		PlayerPrefs.SetInt ("Diamonds", diamonds);
	}

	/// <summary>
	/// Displays score to pause menu
	/// </summary>
	public void PauseScore()
	{
		CalculateScore();

		pauseLvlText.text = lvlNoCleared.ToString();
		pauseGreenText.text = greenCollected.ToString();
		pauseRedText.text = redAvoided.ToString();
		pauseTileText.text = tileNoPlaced.ToString();
		pauseTotText.text = score.ToString();
	}

	/// <summary>
	/// Calculates Highscore.
	/// </summary>
	public void CalculateScore()
	{
		lvlNoCleared = (level - 1);
		int lvlPointTot = (lvlNoCleared * 100);
		greenCollected = greencol;
		int greenPointTot = (greenCollected * 50);
		redAvoided = redavoid;
		int redPointTot = (redAvoided * 5);
		tileNoPlaced = tileplaced;

		int tilePlacedTot = tileNoPlaced * -5;
		score = lvlPointTot + greenPointTot + redPointTot + tilePlacedTot;
		if (score < 0)
			score = 0;
	}

	#endregion

	// Events are the enemies
	#region Event Functions

	/// <summary>
	/// Used in determining valid enemy movement directions.
	/// Checks a given direction, if the move is valid returns true.
	/// </summary>
	/// <returns>True if the move is valid.</returns>
	/// <param name="dir">Dir.</param>
	public bool CheckEnemyMove(string dir, int curRow, int curCol)
	{

		bool isValidMove = false;

		Tile currentTile = tileBoard [curRow, curCol];
		Tile newTile; // the target tile.

		string curLocStr = curRow.ToString() + curCol.ToString();
		string newLocStr;

		switch (dir)
		{
		case "up":

			if (curRow - 1 >= 0)
			{
				if (tileBoard[curRow - 1, curCol] == null)
				{
					Debug.Log("The tile [" + (curRow - 1).ToString() + curCol.ToString() + "] is null.");
				}
				newTile = tileBoard[curRow - 1, curCol];
				newLocStr = (curRow - 1).ToString() + curCol.ToString();
			}
			else
			{
				return false;
			}
			break;

		case "down":

			if (curRow + 1 < 6)
			{
				newTile = tileBoard[curRow + 1, curCol];
				newLocStr = (curRow + 1).ToString() + curCol.ToString();
			}
			else
			{
				return false;
			}
			break;

		case "left":

			if (curCol - 1 >= 0)
			{
				newTile = tileBoard[curRow, curCol - 1];
				newLocStr = (curRow).ToString() + (curCol - 1).ToString();
			}
			else
			{
				return false;
			}
			break;

		case "right":
			if (curCol + 1 < 5)
			{
				newTile = tileBoard[curRow, curCol + 1];
				newLocStr = (curRow).ToString() + (curCol + 1).ToString();
			}
			else
			{
				return false;
			}
			break;

		default:
			return false;
		}

		// Just some debugging code.
		// Used for printing tile info to the debug log.
		string debug = "# ";

		foreach (Tile tile in tileBoard)
		{
			if(tile._isEntrySet)
			{
				debug += tile._boardLocation + " ";

				if (tile._event != "")
				{
					debug += "[" + tile._event + "] ";
				}
			}
		}

		//Debug.Log("_isEntrySet Tiles: " + debug);

		debug = "# ";

		foreach (Tile tile in tileBoard)
		{
			if (tile._event == " red" || tile._event == " green")
			{
				debug += tile._event;
			}
		}

		string debugStr = "[Trying move [" + currentTile._boardLocation + " -> " + newTile._boardLocation + "] (" + dir + ")"; // Rather than having a million debug messages, add messages to this string, and use one debug.log

		// Moving from a tile placed by the player.
		if (currentTile._isEntrySet)
		{
			debugStr += " [Enemy at [" + curLocStr + "] is checking move from a non-empty tile] ";

			// If the target is a tile as well. Prevent moving to entrance, exit, chest tiles and other enemies. 
			if (newTile._isEntrySet && (newTile._tileID != "tile_exit" && newTile._tileID != "tile_entrance") && (newTile._event != "green" && newTile._event != "red") && (newLocStr != playerLoc))
			{

				string debugStr2 = "";
				foreach (string entry in newTile._entry)
				{
					debugStr2 += entry + " ";
				}
				//Debug.Log("Target tile ("+ newLocStr +") entries: ");

				if (validMove.ValidMovement(dir, currentTile, newTile))
				{
					debugStr += " [Target tile ("+ newLocStr +") was placed by player, and is a valid move] ";
					isValidMove = true;
				}
				else
				{
					debugStr += " [Target tile (" + newLocStr + ") was placed by player, but is not a valid move] ";
					isValidMove = false;
				}

			}
			else
			{
				// Target tile is empty
				if (!newTile._isEntrySet && currentTile.ValidMove(dir) && (newTile._event != "green" && newTile._event != "red"))
				{
					debugStr += " [Target tile (" + newLocStr + ") is empty, and current tile has relevant exit] ";
					isValidMove = true;
				}
				else
				{
					debugStr += " [Target tile (" + newLocStr + ") is empty, but current tile does not have relevent exit] ";

					debugStr += "Fucking why? " + "_isEntrySet? " + newTile._isEntrySet + ", Valid move? " + currentTile.ValidMove("dir") + ", An event? " + newTile._event;

					debugStr += " [Tile had ";
					foreach (string entry in currentTile._entry)
					{
						debugStr += entry + " ";
					}
					debugStr += "]";
				}
			}

		}
		// Moving from an empty tile
		else
		{
			debugStr += " [Enemy at [" + curLocStr + "] is checking move from an empty tile] ";
			// Target is not empty
			if (newTile._isEntrySet && (newTile._tileID != "tile_exit" && newTile._tileID != "tile_entrance") && (newTile._event != "green" && newTile._event != "red") && (newLocStr != playerLoc))
			{
				if (newTile.ValidEntry(dir))
				{
					debugStr += " [Target tile (" + newLocStr + ") is player-placed, and current tile is empty] ";
					isValidMove = true;
				}
				else
				{
					debugStr += " [The target tile does not have the correct entry]";
				}
			}
			else if ((newTile._event == "green") || (newTile._event == "red"))
			{
				// If there is an event, then the move is invalid.
				debugStr += " [Target tile contains a "+ newTile._event + " event, and is thus an invalid move.]";
			}
			else if (newTile._tileID == null)
			{
				debugStr += " [Target tile (" + newLocStr + ") is empty, and current tile is also empty] ";
				isValidMove = true;
			}
		}


		//debugStr += " Target tile event: " + newTile._event;


		//Debug.Log(debugStr);

		return isValidMove;
	}

	/// <summary>
	/// Moves an enemy tile, based upon a given string direction. (up, down, left, right)
	/// </summary>
	public void MoveEnemy(string dir, int curRow, int curCol, int enemyIndex)
	{
		string newLoc = "";
		int newRow = 0, newCol = 0;

		switch (dir)
		{
		case "up":
			newLoc = (curRow - 1).ToString() + curCol.ToString();
			newRow = curRow - 1;
			newCol = curCol;
			break;

		case "down":
			newLoc = (curRow + 1).ToString() + curCol.ToString();
			newRow = curRow + 1;
			newCol = curCol;
			break;

		case "left":
			newLoc = curRow.ToString() + (curCol - 1).ToString();
			newRow = curRow;
			newCol = curCol - 1;
			break;

		case "right":
			newLoc = curRow.ToString() + (curCol + 1).ToString();
			newRow = curRow;
			newCol = curCol + 1;
			break;
		}

		// Used to access gridpanels
		int panelkey = 0;

		Tile dummy = new Tile(0);
		GameObject[] eventTiles = GameObject.FindGameObjectsWithTag("eventTile");

		cellindex.TryGetValue(newLoc, out panelkey);

		//update tileboard
		eventTiles[enemyIndex].transform.localPosition = gridPanels[panelkey].transform.localPosition;

		//clone the current tile to dummy
		dummy.CloneTile(tileBoard[curRow, curCol]);

		//set current tile event that's moving to no event
		tileBoard[curRow, curCol].ClearEvent();

		//flush dummy entry if it is set from the previous tile cloned
		if (dummy._isEntrySet)
		{
			dummy.FlushEntry();
		}

		//update the new board position with the dummy clone
		tileBoard[newRow, newCol].UpdatePosition(dummy);

		//update objclone name to be used for destroying the game obj
		eventTiles[enemyIndex].name = newLoc + "(Clone)";
	}

	/// <summary>
	/// Handles movement of the red event tiles
	/// I'm going to re-write this to create a list of available moves, and then choose from one of them. ~ Nick
	/// </summary>
	public void MoveEvents()
	{
		string etloc = "";
		int currow = 0;
		int curcol = 0;

		GameObject[] eventTiles = GameObject.FindGameObjectsWithTag("eventTile");
		for (int i = 0; i < eventTiles.Length; i++)
		{
			// For red tiles only. Green tiles don't move.
			if (eventTiles[i].GetComponent<Image>().sprite.name == "enemyCharA")
			{
				etloc = eventTiles[i].name.Substring(0, 2);
				//Debug.Log("etloc: " + etloc);
				System.Int32.TryParse(etloc.Substring(0, 1), out currow);
				System.Int32.TryParse(etloc.Substring(1, 1), out curcol);

				var moves = new List<string>();

				//Debug.Log("###################################################");
				//Debug.Log("Checking moves for Enemy at [" + etloc + "]");
				//Debug.Log("###################################################");

				// Check for available moves
				if (CheckEnemyMove ("up", currow, curcol)) {
					moves.Add ("up");
					//Debug.Log ("["+ currow +"," + curcol +"] can move up.");
				}

				if (CheckEnemyMove ("down", currow, curcol)) {
					moves.Add ("down");
					//Debug.Log ("["+ currow +"," + curcol +"] can move down.");
				}

				if (CheckEnemyMove ("left", currow, curcol)) {
					moves.Add ("left");
					//Debug.Log ("["+ currow +"," + curcol +"] can move left.");
				}

				if (CheckEnemyMove ("right", currow, curcol)) {
					moves.Add ("right");
					//Debug.Log ("["+ currow +"," + curcol +"] can move right.");
				}


				//Debug.Log("Available moves for enemy " + etloc + ": ");

				foreach (string move in moves)
				{
					//Debug.Log(move.ToString());
				}

				// Here's hoping this works lol
				// Moves an enemy, if it is able
				if (moves.Count != 0)
				{
					int newMove = Random.Range(0,moves.Count);
					//Debug.Log("Selecting number from 1 to " + moves.Count);
					//Debug.Log("Enemy at [" + currow.ToString() + curcol.ToString() + "] moving " + moves[newMove].ToString());

					MoveEnemy(moves[newMove], currow, curcol, i);
				}
				else
				{
					// The enemy can't move.
					// This else is here for giving the player a bonus if they've trapped an enemy.
					// May not actually be needed.
					Debug.Log("Enemy at [" + currow.ToString() + curcol.ToString() + "] can no longer move.");
				}
			}
		}
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
				DisplayClickPanel(enemyPanel);

				if (damage != 0) //don't show stam popup if stamina is not reduced
				{
					string newText = damage.ToString();
					InstantiateStamDownPanel(newText, tempObj.transform.position);
					playerStamina += damage;
					UpdateUI();
					coinCont.UpdateCoins (damage, tempTile._boardLocation);
				}

				int rand = Random.Range(0, redTileClips.Length);
				audioSource.PlayOneShot(redTileClips[rand]);

				redstep++;
			}
			else if (tempTile._event == "green")
			{
				string newText = "+" + tempTile.combat.ToString();
				InstantiateStamUpPanel(newText, tempObj.transform.position);
				playerStamina += tempTile.combat;
				coinCont.UpdateCoins (tempTile.combat, tempTile._boardLocation);

				int rand = Random.Range(0, greenTileClips.Length);
				audioSource.PlayOneShot(greenTileClips[rand]);

				//increment the greens taken
				greencol++;
				UpdateUI();
			}

			if (tempObj != null)
			{
				Debug.Log("destroy clone");
				Destroy(tempObj);
				tempTile._isActive = false;

			}
			CheckStamina();
			tempTile._event = ""; // Event wasn't being destroyed after it was triggered. This should solve that.
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
		//check if tileboard is empty
		if (tileBoard.Length != 0)
		{
			//generate tileboard...
			for (int row = 0; row < 6; row++)
			{
				for (int col = 0; col < 5; col++)
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
			for (int row = 5; row < 6; row++)
			{
				for (int col = 0; col < 5; col++)
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
				for (int col = 0; col < 5; col++)
				{
					if (tileBoard[row, col]._tileID == "tile_entrance")
					{
						tileBoard[row, col]._isOccupied = true;
						playerLoc = row.ToString() + col.ToString();
						//draw player
						int pindex = 0;
						cellindex.TryGetValue(playerLoc, out pindex);
						movePlayer.DrawPlayer(pindex, gridPanels);
						int rand = Random.Range(0, movementClips.Length);
						audioSource.PlayOneShot(movementClips[rand], 1.0f);
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
		int greenDecrement = CalculateDecrement();
		int green = Random.Range(minGreenNo, maxGreenNo + 1) - greenDecrement;
		if (green < 3)
			green = 3;

		// Generate the number of red tiles
		int redIncrement = CalculateIncrement();
		int red = Random.Range(minRedNo, maxRedNo + 1) + redIncrement;

		//store the rng red into redtiles to use for scoring
		redtiles = red;
		redstep = 0;

		if (gridPanels != null)
		{
			GameObject gridPanelsParent = gridPanels[0].transform.parent.gameObject;

			//exit tile location will be somewhere in the bottom row
			int downPanel = Random.Range(25, 30);
			//entrance tile location will be somewhere in the top row
			int upPanel = Random.Range(0, 5);

			int[] randomPanels = new int[green + red];
			//Debug.Log("green + red = " + randomPanels.Length);

			//initialise location of all event panels to be 30 (default)
			for (int i = 0; i < randomPanels.Length; i++)
				randomPanels[i] = 30;

			//set exit tile
			gridPanels[downPanel].GetComponent<Image>().sprite = gridSprite[1] as Sprite;
			gridPanels[downPanel].GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
			exit = gridPanels[downPanel].name;
			//set entrance tile
			gridPanels[upPanel].GetComponent<Image>().sprite = gridSprite[2] as Sprite;
			gridPanels[upPanel].GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
			entrance = gridPanels[upPanel].name;

			//set random panel numbers for red and green tile placement
			for (int i = 0; i < randomPanels.Length; i++)
			{
				//check that a successful location is chosen (no longer 30)
				while (randomPanels[i] == 30)
				{
					int randNo = Random.Range(0, 30);
					//check that location is not same as entrance or exit tile
					if (randNo != downPanel && randNo != upPanel)
					{
						randomPanels[i] = randNo;
						for (int j = 0; j < i; j++)
						{
							//check that location is not already taken
							if (randNo == randomPanels[j])
							{
								randomPanels[i] = 30;
								break;
							}
							else if (j == i - 1)
								randomPanels[i] = randNo;
						}

					}
				}
			}
			//store event green tile into eventgreen list
			eventindex.Clear();

			//Draw all green tiles
			for (int i = 0; i < green; i++)
			{
				GameObject tempPanel = gridPanels[randomPanels[i]];
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
				GameObject tempPanel = gridPanels[randomPanels[i]];
				GameObject panelClone = Instantiate(tempPanel);
				panelClone.transform.SetParent(gridPanelsParent.transform);
				panelClone.tag = "eventTile";
				panelClone.transform.localPosition = tempPanel.transform.localPosition;
				panelClone.transform.localScale = new Vector3(1, 1, 1);
				panelClone.GetComponent<Image>().sprite = eventEnemy[Random.Range(0, eventEnemy.Length)] as Sprite;
				panelClone.GetComponent<Image>().color = new Color(255f, 255f, 255f, 150f);

				//store event red tiles into eventred list
				eventindex.Add(tempPanel.name, "event_red");

			}
		}
	}

	private void FindGridPanels()
	{
		gridPanelsScript = GameObject.FindGameObjectWithTag("Scripts").GetComponent<GridPanels>();
		gridPanels = new GameObject[30];

		for (int i = 0; i < gridPanels.Length; i++)
		{
			gridPanels[i] = gridPanelsScript.GetGridPanel(i);
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
	#endregion

	#region Hand Functions

	/// <summary>
	/// Generate a new hand of tiles
	/// </summary>
	public void NewHand()
	{
		handTiles = GameObject.FindGameObjectsWithTag("handDrag");
		if (handTiles != null)
		{
			for (int i = 0; i < handTiles.Length; i++)
			{
				Destroy(handTiles[i]);
			}
		}
		InstantiateStamDownPanel("-" + newHandCost, newHandButton.gameObject.transform.position);
		playerStamina += -newHandCost;
		GenerateHand();
		UpdateUI();
		coinCont.UpdateCoins (-newHandCost, "newHandButton");
		CheckStamina();
	}

	public void GenerateHand()
	{
		//approaching hand generation via grabbing each individual UI element and updating the sprite image and render...didnt work out 13/04
		//actaullyworking just rendered tiny and behind default image too...13/04
		handTiles = GameObject.FindGameObjectsWithTag("handDefault");
		btmPanel = GameObject.FindGameObjectWithTag("bottomPanel");
		//check for null
		if (handTiles != null)
		{
			int rand = Random.Range(0, dealingClips.Length);
			audioSource.PlayOneShot(dealingClips[rand], 1.0f);

			List<int> spriteIndex = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

			for (int i = 0; i < handTiles.Length; i++)
			{
				GameObject newObject = Instantiate(handTiles[i]);
				newObject.transform.localScale = handTiles[i].transform.localScale;
				newObject.transform.localPosition = handTiles[i].transform.localPosition;
				//set tag so handTiles above doesnt grab clones as well.
				newObject.tag = "handDrag";
				//assign new object correct parents
				newObject.transform.SetParent(btmPanel.transform, false);
				//use handdefaults to instantiate objects with rng sprite below and add script....
				int randIndex = Random.Range(0, spriteIndex.Count);
				int index = spriteIndex[randIndex];
				newObject.GetComponent<Image>().sprite = tileSprite[index] as Sprite;
				newObject.GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
				newObject.AddComponent<Draggable>();
				//above method with bool set to false solved instantiating flipped object....

				//remove index from list if it does not represent the crossways tile (10th element)
				if (index != 10)
				{
					spriteIndex.RemoveAt(randIndex);
				}
			}
		}
	}

	#endregion

	#region UI Functions

	/// <summary>
	/// Updates Player Stamina text and Stamina bar.
	/// </summary>
	public void UpdateUI()
	{
		if (playerStamina > maxStamina)
		{
			playerStamina = maxStamina;
		}
		else if (playerStamina < 0)
		{
			playerStamina = 0;
		}
		GameObject tempObj = GameObject.FindGameObjectWithTag("PlayerStam");
		tempObj.GetComponent<Text>().text = playerStamina.ToString();

		CalculateScore ();
		gameScore.text = score.ToString();
		diamondAmount.text = diamonds.ToString();
	}

	/// <summary>
	/// Try again button for the statpanel disable statpanel and destory gameObject and load the new scene.
	/// </summary>
	public void TryAgain()
	{
		//Debug.Log("Try again");
		statPanel.SetActive(false);
		Destroy(gameObject);
		PauseBehaviour pb = new PauseBehaviour();
		pb.LoadScene("main_game");
	}

	/// <summary>
	/// Enables pause panel UI.
	/// </summary>
	/// <param name="panel"></param>
	private void DisplayClickPanel (GameObject panel)
	{
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
		do
		{
			yield return null;
		}while (!Input.GetMouseButtonUp (0));
		panel.SetActive (false);
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

	/// <summary>
	/// Finds the tile names (ie. "00", "03") of the entrance, exit and player locations
	/// </summary>
	/// <returns>A string of tile names.</returns>
	private string[] FindInvalidRemoveTiles()
	{
		List<string> list = new List<string> ();

		for (int i = 0; i < 30; i++)
		{
			string name = gridPanels[i].name;
			int temprow = System.Int32.Parse(name.Substring(0, 1));
			int tempcol = System.Int32.Parse(name.Substring(1, 1));

			if (tileBoard[temprow, tempcol]._isEntrySet == false)
			{
				list.Add (name);
			}
			else if (name == entrance || name == playerLoc || name == exit)
			{
				list.Add (name);
			}
		}

		string[] array = new string[list.Count];

		for (int i = 0; i < list.Count; i++)
		{
			array[i] = list[i];
		}

		/*
		if (entrance == playerLoc) // only need two elements in the array
		{
			array = new string[2] {entrance, exit};
		}
		else
		{
			array = new string[3] {entrance, playerLoc, exit};
		}*/

		return array;
	}

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

			string[] invalidTileNames = FindInvalidRemoveTiles();
			manageRemoveTileScript.DisplayOverlays (invalidTileNames);

			if (showRemoveTilePop)
			{
				DisplayClickPanel (removeTilePopUp);
				showRemoveTilePop = false;
			}
		}
		else
		{
			Debug.Log("No longer deleting tile.");
			newHandButton.interactable = true;
			manageRemoveTileScript.HidePanelOverlays();
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
			if (clickLoc != "")
			{
				// row and column of tile that was clicked.
				int temprow = System.Int32.Parse(clickLoc.Substring(0, 1));
				int tempcol = System.Int32.Parse(clickLoc.Substring(1, 1));

				// Prevent deletion if the tile is an exit or entry.
				if (tileBoard[temprow, tempcol] != null && !tileBoard[temprow, tempcol]._tileID.Contains("exit") && !tileBoard[temprow, tempcol]._tileID.Contains("entrance") && tileBoard[temprow, tempcol]._isEntrySet == true)
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

						InstantiateStamDownPanel("-" + removeTileCost, GetGridPanelPosition (clickLoc));
						playerStamina += -removeTileCost;
						UpdateUI();
						coinCont.UpdateCoins (-removeTileCost, clickLoc);
						CheckStamina();

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
	#endregion


}