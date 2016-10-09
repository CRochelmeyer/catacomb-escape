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
	public int discardCost;
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

	private AudioSource audioSource;
	#region audioClips
	[Header("Audio Clips")]
	public AudioClip startGameClip;
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
	#endregion

	#region stamPanels
	[Header("Stamina Panels")]
	public Transform stamPopupsContainer;
	public GameObject stamUpPrefab;
	public GameObject stamDownPrefab;
	#endregion

	#region tutorialPanels
	[Header("Tutorial Panels")]
	public GameObject stage1a;
	public GameObject stage1b;
	public GameObject stage2a;
	public GameObject stage2b;
	public GameObject stage3a;
	public GameObject stage3b;
	public GameObject stage4a;
	public GameObject stage4b;
	public GameObject stage5a;
	public GameObject stage5b;
	public GameObject stage6a;
	public GameObject stage6b;
	public GameObject stage7a;
	public GameObject stage7b;
	public GameObject stage8a;
	public GameObject stage8b;
	#endregion

	//boolean game conditions
	private bool emptyhand = true;
	private bool nextlevel = false;
	private int playerStamina;
	private string playerLoc="";
	private string destLoc = "";
	private string mouseLocation = "";

	//tutorial conditions
	private GameObject handTile0 = null;
	private GameObject handTile1 = null;
	private GameObject handTile2 = null;
	private GameObject handTile3 = null;
	private int feedHand = 0;
	private string[] placementLocations = new string[] {"10", "11", "21", "20", "22"};
	private int placementIndex = 0;
	public Button discardButton;
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
	private CoinController coinCont;
	private GameObject[] gridPanels;
	private Tile[,] tileBoard;

	//awake called behind start
	void Awake()
	{
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

		validMove = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<Direction> ();
		movePlayer = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<PlayerMove> ();
		coinCont = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<CoinController> ();

		playerStamina = standardStamina;
		UpdateUI();

		discardButton.interactable = false;

		// Stage 1: Tile 1
		// Stage 2: Tile 2
		// Stage 3: Tile 3
		// Stage 4: Tile 4
		// Stage 5: Move to chest
		// Stage 6: Discard hand
		// Stage 7: Tile 5
		// Stage 8: Move to exit

		InitLevel(level);

		stage1a.SetActive (true);
		stage1b.SetActive (true);
	}

	// Update is called once per frame
	void Update()
	{
		switch (stage)
		{
		case 2:
			stage1a.SetActive (false);
			stage1b.SetActive (false);
			stage2a.SetActive (true);
			stage2b.SetActive (true);
			break;
		case 3:
			stage2a.SetActive (false);
			stage2b.SetActive (false);
			stage3a.SetActive (true);
			stage3b.SetActive (true);
			break;
		case 4:
			stage3a.SetActive (false);
			stage3b.SetActive (false);
			stage4a.SetActive (true);
			stage4b.SetActive (true);
			break;
		case 5:
			stage4a.SetActive (false);
			stage4b.SetActive (false);
			stage5a.SetActive (true);
			stage5b.SetActive (true);
			break;
		case 6:
			stage5a.SetActive (false);
			stage5b.SetActive (false);
			stage6a.SetActive (true);
			//stage6b.SetActive (true);
			discardButton.interactable = true;
			break;
		case 7:
			stage6a.SetActive (false);
			//stage6b.SetActive (false);
			stage7a.SetActive (true);
			//stage7b.SetActive (true);
			break;
		case 8:
			stage7a.SetActive (false);
			//stage7b.SetActive (false);
			stage8a.SetActive (true);
			//stage8b.SetActive (true);
			break;
		}


		if (PlayerPrefs.GetString ("Paused") != "true")
		{
			//check if handTiles has been filled
			if (emptyhand == true)
			{
				if (feedHand == 0)
				{
					// Generate hand with "right,left", "down,left", "up,right,down" and "up, right, left"
					GenerateHand (3, 0, 8, 9);
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

			PlayerClick();

			if (nextlevel)
			{
				PlayerPrefs.SetString ("Paused", "true");
				tutCompletePanel.SetActive (true);
			}
		}
	}

	public bool SetNextLevel
	{
		set {nextlevel = value;}
	}

	private void SetNextDraggable()
	{
		switch (placementIndex)
		{
		case 0:
			handTile2.AddComponent<Draggable>();
			break;
		case 3:
			handTile0.AddComponent<Draggable>();
			break;
		case 2:
			handTile3.AddComponent<Draggable>();
			break;
		case 1:
			handTile1.AddComponent<Draggable>();
			break;
		}
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

		//initialise player data
		if (playerStamina < standardStamina)
		{
			int stamUpAmt = standardStamina - playerStamina;
			string stamUpStr = stamUpAmt.ToString();
			coinCont.UpdateCoins (stamUpAmt, playerLoc);
			Vector3 playerPos = GetGridPanelPosition (playerLoc);
			InstantiateStamUpPanel (stamUpStr, playerPos);
			playerStamina = standardStamina;
		}

		//setup board with rng sprites
		GenerateBoard();

		tileBoard = new Tile[4, 3];
		for (int i =0; i<4;i++)
		{
			for (int j =0; j<3;j++)
			{
				tileBoard[i, j] = new Tile(0);
			}
		}

		GenerateBoardLogic();
	}

	// ~Mouse related method
	public string MouseLocation
	{
		get{UpdateMouseLocation(); 
			return mouseLocation; }
		set{ mouseLocation = value; }
	}

	// ~Level related method
	public int GetLevel
	{
		get {return level;}
	}

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


	//
	//
	//	Enemies movements will be set
	//
	//


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
			if (eventTiles[i].GetComponent<Image>().sprite.name == "enemyCharA")
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
		while (!foundMouse && i < 12)
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

	private void FindGridPanels()
	{
		gridPanelsScript = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<GridPanelsTutorial> ();
		gridPanels = new GameObject[12];

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
					}
					else if (tileBoard [System.Int32.Parse (pcell.Substring (0, 1)), System.Int32.Parse (pcell.Substring (1, 1))]._event == "red")
					{
						GameObject temp = GameObject.Find (tileBoard [System.Int32.Parse (pcell.Substring (0, 1)), System.Int32.Parse (pcell.Substring (1, 1))]._boardLocation + "(Clone)");
						temp.GetComponent <Image>().sprite = eventEnemy [Random.Range (0, eventEnemy.Length)] as Sprite; 
					}
				}
				else
				{
					tileBoard [System.Int32.Parse (pcell.Substring (0, 1)), System.Int32.Parse (pcell.Substring (1, 1))] = ptile;
				}

				GameObject tempObj = GameObject.Find (tileBoard [System.Int32.Parse (pcell.Substring (0, 1)), System.Int32.Parse (pcell.Substring (1, 1))]._boardLocation);

				//decrease stamina
				int rand = Random.Range (0,placementClips.Length);
				audioSource.PlayOneShot (placementClips[rand], 0.5f);
				playerStamina -= 2;
				InstantiateStamDownPanel ("-2", tempObj.transform.position);

				stage ++;
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
		//check for right click
		if(Input.GetMouseButtonDown(0))
		{
			string clickLoc = "";			

			clickLoc = MouseLocation;

			if (clickLoc != "")
			{
				int temprow = System.Int32.Parse(clickLoc.Substring(0, 1));
				int tempcol = System.Int32.Parse(clickLoc.Substring(1, 1));

				if ((tileBoard[temprow, tempcol]._isEntrySet) && (playerLoc != "") )
				{
					if (validMove.MoveDirection(playerLoc, clickLoc) != "invalid move" && validMove.InRange(playerLoc, clickLoc) )
					{
						if (validMove.Move(playerLoc, clickLoc,ref tileBoard) )
						{
							int rand = Random.Range (0,movementClips.Length);
							audioSource.PlayOneShot (movementClips[rand], 1.0f);

							int tempIndex = 0;
							cellindex.TryGetValue(clickLoc, out tempIndex);
							movePlayer.UpdatePlayer(gridPanels[tempIndex], validMove.MoveDirection(playerLoc,clickLoc));

							// update the player's location
							destLoc = clickLoc;

							playerStamina--;
							InstantiateStamDownPanel ("-1", movePlayer.PlayerLocation);
							UpdateUI();
							coinCont.UpdateCoins (-1, playerLoc);
						}
					}
				}
				else
				{
					Debug.Log("Invalid player move");
				}
			}
		}
	}

	/// <summary>
	/// Generate a new hand of tiles
	/// </summary>
	public void NewHand()
	{
		handTilesDrag = GameObject.FindGameObjectsWithTag("handDrag");
		if (handTilesDrag != null)
		{
			for (int i =0; i<handTilesDrag.Length;i++)
			{
				Destroy(handTilesDrag[i]);
			}
		}
		InstantiateStamDownPanel ("-" + discardCost, movePlayer.PlayerLocation);
		playerStamina += - discardCost;

		// Generate hand with "up,down", "right,left", "up,down,left" and "up, right, left"
		GenerateHand (4, 3, 5, 9);
		feedHand++;
		emptyhand = false;

		stage++;
		UpdateUI();
		coinCont.UpdateCoins (-discardCost, "newHandButton");

		discardButton.interactable = false;

		handTile2.AddComponent<Draggable>();
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
		while (!Input.GetMouseButtonUp (0))
		{
			yield return null;
		}
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

			// SET TILE #1
			handTile0 = Instantiate (handTiles[0]);
			handTile0.transform.localScale = handTiles [0].transform.localScale;
			handTile0.transform.localPosition = handTiles [0].transform.localPosition;
			//set tag so handTiles above doesnt grab clones as well.
			handTile0.tag = "handDrag";
			//assign new object correct parents
			handTile0.transform.SetParent (btmPanel.transform, false);
			handTile0.GetComponent <Image>().sprite = tileSprite [idx0] as Sprite;
			handTile0.GetComponent <Image>().color = new Color(255f,255f,255f,255f);

			// SET TILE #2
			handTile1 = Instantiate (handTiles[1]);
			handTile1.transform.localScale = handTiles [1].transform.localScale;
			handTile1.transform.localPosition = handTiles [1].transform.localPosition;
			//set tag so handTiles above doesnt grab clones as well.
			handTile1.tag = "handDrag";
			//assign new object correct parents
			handTile1.transform.SetParent (btmPanel.transform, false);
			handTile1.GetComponent <Image>().sprite = tileSprite [idx1] as Sprite;
			handTile1.GetComponent <Image>().color = new Color(255f,255f,255f,255f);

			// SET TILE #3
			handTile2 = Instantiate (handTiles[2]);
			handTile2.transform.localScale = handTiles [2].transform.localScale;
			handTile2.transform.localPosition = handTiles [2].transform.localPosition;
			//set tag so handTiles above doesnt grab clones as well.
			handTile2.tag = "handDrag";
			//assign new object correct parents
			handTile2.transform.SetParent (btmPanel.transform, false);
			handTile2.GetComponent <Image>().sprite = tileSprite [idx2] as Sprite;
			handTile2.GetComponent <Image>().color = new Color(255f,255f,255f,255f);

			// SET TILE #4
			handTile3 = Instantiate (handTiles[3]);
			handTile3.transform.localScale = handTiles [3].transform.localScale;
			handTile3.transform.localPosition = handTiles [3].transform.localPosition;
			//set tag so handTiles above doesnt grab clones as well.
			handTile3.tag = "handDrag";
			//assign new object correct parents
			handTile3.transform.SetParent (btmPanel.transform, false);
			handTile3.GetComponent <Image>().sprite = tileSprite [idx3] as Sprite;
			handTile3.GetComponent <Image>().color = new Color(255f,255f,255f,255f);

			handTilesDrag = GameObject.FindGameObjectsWithTag("handDrag");
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

			}
		}
	}
}