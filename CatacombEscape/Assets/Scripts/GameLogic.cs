﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;


public class GameLogic : MonoBehaviour
{
    //boolean game conditions
    private bool gameover = false;
    private bool emptyhand = true;
    private bool nextlevel = false;
    private int playerStamina = 100;
	private string playerLoc="";
	private string destLoc = "";
	private string mouseLocation = "";
    private int playerEquip = 0;

    //dictionary to match cell strings of 00-04 10-14 to an index from 0-29
    Dictionary<string, int> cellindex = new Dictionary<string, int>();
    //dictionary to store event tile clone name and original grid panel location int
    Dictionary<string, string> eventindex = new Dictionary<string, string>();
    //dictionary for item desciptions
    Dictionary<int, string> equipmentindex = new Dictionary<int, string>();
    //sprite holders drag sprites via inspector


    public Sprite[] tileSprite;
    public Sprite[] gridSprite;
    public GameObject btmPanel;
    public GameObject[] handTiles;
	public string exit;
    //create tile gameboard 2d array
    public Tile[,] gameBoard = new Tile[5, 6];
    //initiate a static instance of gamelogic to be used globally...
    public static GameLogic instance = null;

    private int level = 1;
	private GridPanels gridPanelsScript;
	private Direction validMove;
	private PlayerMove movePlayer;
	private GameObject[] gridPanels;
    private Tile[,] tileBoard;
	private float tileplaced = 0;
	private float greencol =0;
	private float redavoid=0;
    private int redtiles;
    private int redstep;
    private float score;

	private AudioSource audioSource;
	public AudioClip startGameClip;
	public AudioClip[] placementClips;
	public AudioClip[] dealingClips;
	public AudioClip[] movementClips;
	public AudioClip[] greenTileClips;
	public AudioClip[] redTileClips;
	public AudioClip[] lvlCompClips;

    public Text equipment;
    public Text equipDesc;
	public Text stamUp;
	public Text stamDown;
	public float fadeTime;
	private bool faderRunning = false;
	public GameObject equipPanel;
	public GameObject snakePanel;
	public Text snakeStamDown;
	public GameObject snakeDefeatPanel;
	public Text snakeDefeatEquip;
	public GameObject scorpionPanel;
	public Text scorpStamDown;
	public GameObject scorpionDefeatPanel;
	public Text scorpDefeatEquip;
	//public Text breadStamUp;
	public GameObject statPanel; //this is what pops up at gameover
	public Text lvlNoCleared;
	public Text lvlPointTot;
	public Text greenCollected;
	public Text greenPointTot;
	public Text redAvoided;
	public Text redPointTot;
	public Text tileNoPlaced;
	public Text tileNoTot; //remember this value should be negative
	public Text pointTot;
	public GameObject newHighscore;
	// These can be accessed and set with 'string tot = pointTot.text;' and 'pointTot.text = "100"'

    //awake called behind start
    void Awake()
    {
		//Debug.Log("GameLogic awake");
        //refresh and initialse redstep per awake call
        redstep = 0;
		audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource> ();
		audioSource.PlayOneShot (startGameClip, 0.5f);
		
		GameObject mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		if (mainCamera != null)
			mainCamera.GetComponent<BackGroundMusic> ().ResetScript ();
		
		validMove = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<Direction> ();
		movePlayer = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<PlayerMove> ();
		TutorialBehaviour tutorialScript = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<TutorialBehaviour> ();

        //creating equipment index
        equipmentindex.Clear();
        int temp = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                string mod = "";
                if (i == 0)
                { mod = "Bronze"; }
                else if (i == 1)
                { mod = "Iron"; }
                else if (i == 3)
				{ mod = "Steel"; }
				else if (i == 4)
				{ mod = "Platinum"; }
                switch (j)
                {
                    case 0:
                        {
                            equipmentindex.Add(temp, mod + " Trowel");
                            break;
                        }
                    case 1:
                        {
                            equipmentindex.Add(temp, mod + " Trenching Hoe");
                            break;
                        }
                    case 2:
                        {
                            equipmentindex.Add(temp, mod + " Shovel");
                            break;
                        }
                    case 3:
                        {
                            equipmentindex.Add(temp, mod + " Pickaxe");
                            break;
                        }
                    case 4:
                        {
                            equipmentindex.Add(temp, mod + " Hand Drill");
                            break;
                        }
                    case 5:
                        {
                            equipmentindex.Add(temp, mod + " Automatic Drill");
                            break;
                        }
                }
                temp++;
            }
        }
        //fill cellindex dictionary
        //temp index int to fill dictonary
        cellindex.Clear();
        temp = 0;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 5; j++)
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
		//DontDestroyOnLoad(gameObject);

		FindGridPanels();

		PlayerPrefs.SetString ("Paused", "false");
		//PlayerPrefs.SetString ("GeneratedBoard", "false");

        InitGame(level);
        tileBoard = new Tile[6, 5];
        for (int i =0; i<6;i++)
        {
            for (int j =0; j<5;j++)
            {
                tileBoard[i, j] = new Tile(0);
            }
        }
        GenerateBoardLogic();
        //Debug.Log("End Awake");

		//Run tutorial each time game is launched from main menu
		if (PlayerPrefs.GetString ("PlayFromMenu") == "true")
		{
			tutorialScript.RunTutorial ();
			PlayerPrefs.SetString ("PlayFromMenu", "false");
		}
	}

	// Use this for initialization
	void Start()
	{
		//Debug.Log("GameLogic start");
    }

    // Update is called once per frame
    void Update()
	{
		if (PlayerPrefs.GetString ("Paused") != "true")
		{
	        //Debug.Log("Update");
	        /*
			if (PlayerPrefs.GetString("GeneratedBoard") == "false")
	        {
	            InitGame(level);
	            PlayerPrefs.SetString("GeneratedBoard", "true");
	        }*/

	        //check if handTiles has been filled
	        if (emptyhand == true)
	        {
	            //generate hand
	            GenerateHand();
	            emptyhand = false;
	        }

			//UpdateMouseLocation ();
	        UpdateUI();
	        PlayerClick();

			if (nextlevel && movePlayer.GetPlayerMoving == false)
	        {
	            nextlevel = false;
	            //PlayerPrefs.SetString("GeneratedBoard", "false");
	            NextLevel();
			}else if (gameover)
	        {
	            GameOverHS();
	            statPanel.SetActive (true);
	        }
		}
    }

    //init game method
    void InitGame(int pLevel)
    {
        //initialise player data
		UpdateUI();
		GameObject tempObj = GameObject.FindGameObjectWithTag("GameLevel");
		tempObj.GetComponent<Text>().text = "Lvl " + pLevel;
        //setup board with rng sprites
		GenerateBoard();
		//PlayerPrefs.SetString ("GeneratedBoard", "true");
		//generate hand 
    	//GenerateHand();
	}

	public string MouseLocation
	{
		get{UpdateMouseLocation(); 
			return mouseLocation; }
		set{ mouseLocation = value; }
	}
	
	public int GetLevel
	{
		get {return level;}
	}

	// Called by PlayerMove once the character animation is complete, to stop events from occuring before player has stopped on the destination tile
	public void SetPlayerLoc()
	{
        Debug.Log("set player loc start");
		playerLoc = destLoc;
        //play event for event tiles    
        if (tileBoard[System.Int32.Parse(playerLoc.Substring(0, 1)), System.Int32.Parse(playerLoc.Substring(1, 1))]._event != "")
		{
			PlayEvent(playerLoc);
		}

		//check if next level...
		if (playerLoc == exit) 
		{
			nextlevel = true;
			movePlayer.PlayerExits ();
		} else 
		{ //move events
			MoveEvents ();
			CheckStamina ();
		}
	}

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
	
    private void GameOverHS()
    {
        lvlNoCleared.text = (level-1).ToString();
        lvlPointTot.text = ((level-1) * 100).ToString();
        greenCollected.text = (greencol).ToString();
        greenPointTot.text = (greencol * 50).ToString();
        redAvoided.text = redavoid.ToString();
        redPointTot.text = (redavoid * 5).ToString();
        tileNoPlaced.text = (tileplaced).ToString();
        //highscore = (level * 100) + (greencol * 10) + (redavoid * 20) + (tileplaced * -.56);
        //pointTot.text = highscore.ToString("0");
		float temp = tileplaced * -0.56f;
        tileNoTot.text = (Mathf.Round(temp)).ToString();
		score = ((level-1) * 100) + (greencol * 10) + (redavoid * 20) + Mathf.Round(temp);
		if (score < 0)
			score = 0;
		pointTot.text = score.ToString();

		string hs = PlayerPrefs.GetString ("HighScore");
		bool newHS = false;
		if (hs != null && hs != "")
		{
			int highscore = int.Parse (hs);
			if (score > highscore)
			{
				newHS = true;
			}
		}else if (score > 0) //A score of 0 doesn't count as a high score the first time played
		{
			newHS = true;
		}

		if (newHS)
		{
			PlayerPrefs.SetString ("HighScore", score.ToString());
			PlayerPrefs.SetString ("LvlNoCleared", lvlNoCleared.text);
			PlayerPrefs.SetString ("LvlPointTot", lvlPointTot.text);
			PlayerPrefs.SetString ("GreenCollected", greenCollected.text);
			PlayerPrefs.SetString ("GreenPointTot", greenPointTot.text);
			PlayerPrefs.SetString ("RedAvoided", redAvoided.text);
			PlayerPrefs.SetString ("RedPointTot", redPointTot.text);
			PlayerPrefs.SetString ("TileNoPlaced", tileNoPlaced.text);
			PlayerPrefs.SetString ("TileNoTot", tileNoTot.text);
			newHighscore.SetActive (true);
		}
    }

    //try again button for the statpanel disable statpanel and destory gameObject and load the new scene.
    public void TryAgain()
    {
        //Debug.Log("Try again");
        statPanel.SetActive(false);
        Destroy(gameObject);
        PauseBehaviour pb = new PauseBehaviour();
        pb.LoadScene("main_game");
    }

	public void UpdateUI()
	{
		GameObject tempObj = GameObject.FindGameObjectWithTag("PlayerStam");
		tempObj.GetComponent<Text>().text = playerStamina + "/100";
		tempObj = GameObject.FindGameObjectWithTag("StamBar");
		tempObj.GetComponent<Slider>().value = playerStamina;
	}

	private void CheckStamina()
	{
		if (playerStamina <= 0)
		{
			//Debug.Log("GameOver " + gameover);
			gameover = true;
		}
        else if (playerStamina > 100 )
        {
            playerStamina = 100;
        }
	}

    public void MoveEvents()
    {
        //Debug.Log("move event");
        //event tile location
        string etloc = "";
        //new location
        string newloc = "";
        int panelkey = 0;
        int currow = 0;
        int curcol = 0;
        int newrow = 0;
        int newcol = 0;

        GameObject[] eventTiles = GameObject.FindGameObjectsWithTag("eventTile");
        for(int i =0; i<eventTiles.Length;i++)
        {
            List<string> pmoves = new List<string>();
            //Debug.Log("Crash for loop" + i);
            //for red tiles only
            if (eventTiles[i].GetComponent<Image>().sprite.name == "event_red")
            {
                //Debug.Log("moving et : " + i);
                //loop to generate the first appropriate new location
                etloc = eventTiles[i].name.Substring(0, 2);
                System.Int32.TryParse(etloc.Substring(0, 1), out currow);
                System.Int32.TryParse(etloc.Substring(1, 1), out curcol);
                //identify possible moves
                pmoves = PossibleEventTileMoves(currow, curcol);
                //Debug.Log(pmoves.Count);
                for (int j = 0; j < pmoves.Count; j++)
                {
                    Debug.Log(pmoves[j]);
                }
                //check if a returned list has "NoMoves"
                if (!pmoves.Contains("nomoves"))
                {
                    int rngmove = Random.Range(0, (pmoves.Count));
                    //Debug.Log("move rng " + rngmove);
                    //vertical movement
                    if (pmoves[rngmove] == "up" || pmoves[rngmove] == "down")
                    {
                        switch (pmoves[rngmove])
                        {
                            case "up":
                                {
                                    newrow = (currow - 1);
                                    break;
                                }
                            case "down":
                                {
                                    newrow = (currow + 1);
                                    break;
                                }
                        }
                        newloc = newrow.ToString() + curcol.ToString();
                        //Debug.Log("Move Vertical");
                        cellindex.TryGetValue(newloc, out panelkey);
                        //set new tileboard location with the event details of the moving event.
                        tileBoard[newrow, curcol].UpdatePosition(tileBoard[currow, curcol]);
                        //update tile's location to newloc
                        tileBoard[newrow, curcol]._boardLocation = newloc;
                        //flush/clear event data from the previous tile...to leave tile details such as entries if its a placed tile intact.
                        tileBoard[currow, curcol].ClearEvent();
                        //move the tile on board and update obj in the UI/game....
                        eventTiles[i].transform.localPosition = gridPanels[panelkey].transform.localPosition;
                        //update objclone name to be used for destroying the game obj
                        eventTiles[i].name = newloc + "(Clone)";
                    }
                    //move horizontal
                    else if (pmoves[rngmove] == "left" || pmoves[rngmove] == "right")
                    {
                        switch (pmoves[rngmove])
                        {
                            case "left":
                                {
                                    newcol = (curcol - 1);
                                    break;
                                }
                            case "right":
                                {
                                    newcol = (curcol + 1);
                                    break;
                                }
                        }
                        newloc = currow.ToString() + newcol.ToString();
                        //Debug.Log("Move horizontal");
                        cellindex.TryGetValue(newloc, out panelkey);
                        //Debug.Log(newcol + "newcol " + curcol + " curcol " + newrow + " newrow " + currow + " currow");
                        //update tileboard
                        //set new tileboard location with the event details of the moving event.
                        tileBoard[currow, newcol].UpdatePosition(tileBoard[currow, curcol]);
                        //update tile's boardlocation with new loc
                        tileBoard[currow, newcol]._boardLocation = newloc;
                        //flush/clear event data from the previous tile...to leave tile details such as entries if its a placed tile intact.
                        tileBoard[currow, curcol].ClearEvent();
                        //move thetile
                        eventTiles[i].transform.localPosition = gridPanels[panelkey].transform.localPosition;
                        //update objclone name to be used for destroying the game obj
                        eventTiles[i].name = newloc + "(Clone)";
                    }
                }
            }
        }
           
    }
    //new approach to moving tiles... find event_red tiles and check its possible moves and RNG based on possible moves to avoid conflicts for a tile unable to move....
    private List<string> PossibleEventTileMoves(int prow, int pcol)
    {
        string newloc = "";
        List<string> possiblemoves = new List<string>();
        //Debug.Log("possibleeventtilemoves");
        //check that down
        if (prow + 1 <= 5 && prow + 1 >= 0)
        {
            //Debug.Log("pmoves inside down");
            newloc = (prow + 1).ToString() + pcol.ToString();
            if (tileBoard[prow + 1, pcol]._event == "" && newloc != playerLoc)
            {
                //Debug.Log("add down");
                possiblemoves.Add("down");
            }
        }
        //check up
        if (prow - 1 <= 5 && prow - 1 >= 0)
        {
            newloc = (prow - 1).ToString() + pcol.ToString();
            //Debug.Log("pmoves inside up");
            if (tileBoard[prow - 1, pcol]._event == "" && newloc != playerLoc)
            {
                //Debug.Log("add up");
                possiblemoves.Add("up");
            }
        }
        //check left
        if (pcol - 1 <= 4 && pcol - 1 >= 0)
        {
            newloc = prow.ToString() + (pcol-1).ToString();
            //Debug.Log("pmoves inside left");
            if (tileBoard[prow, pcol - 1]._event == "" && newloc != playerLoc)
            {
                //Debug.Log("add left");
                possiblemoves.Add("left");
            }
        }
        //check right
        if (pcol + 1 <= 4 && pcol + 1 >= 0)
        {
            newloc = prow.ToString() + (pcol + 1).ToString();
            //Debug.Log("pmoves inside right");
            if (tileBoard[prow, pcol + 1]._event == "" && newloc != playerLoc)
            {
                //Debug.Log("add right");
                possiblemoves.Add("right");
            }
        }
        //else set NoMoves currently this is causing an issue sometimes checking ot see if bug occurs further with count<0 condition....
        else
        {
            if (possiblemoves.Count<0)
            {
                possiblemoves.Add("nomoves");
            }
        }
        return possiblemoves;
    }
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

    private void FindGridPanels()
	{
        gridPanelsScript = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<GridPanels> ();
		gridPanels = new GameObject[30];

		for (int i = 0; i < gridPanels.Length; i++)
		{
			gridPanels[i] = gridPanelsScript.GetGridPanel(i);
		}
        //Debug.Log("gridpanelsFound");
        
    }


    public void UpdateDrag( Tile ptile , string pcell)
    {
        //Debug.Log("updatedrag");
        //grab index based on pcell in cellindex dictionary
        int _gridIndex = 33;
        int _spriteIndex =0;
        cellindex.TryGetValue(pcell, out _gridIndex);
        //grab corresponding gridsprite[index] based on ptile
        for (int i = 0; i < tileSprite.Length; i++)
        {
            string temp1 = tileSprite[i].name.ToString();
            string temp2 = ptile._tileID.ToString();
            //Debug.Log(temp1 + "::::" + temp2);
            if (string.Compare(temp1, temp2) == 0 && _gridIndex != 33 )
            {
                _spriteIndex = i;
                gridPanels[_gridIndex].GetComponent<Image>().sprite = tileSprite[_spriteIndex] as Sprite;
                gridPanels[_gridIndex].GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
                //Debug.Log("checking logic event:" + tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))]._event);
                //update tileBoard
                //check tileBoard cell doesnt exist a event cell already
                if (tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))]._event != "")
                {
                    tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))].UpdateTile(ptile);
                }
                else
                {
                    tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))] = ptile;
                }
                //tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))].test();
				//decreaste stamina

				int rand = Random.Range (0,placementClips.Length);
				audioSource.PlayOneShot (placementClips[rand], 0.5f);
				playerStamina -= 2;
				StartFade (stamDown, "-2", fadeTime);
                //increment tileplaced
                tileplaced++;
				CheckStamina();
                break;
            }
        }
        //check if hand is empty (one remaining to be destroyed), draw it using tag handDrag
        handTiles = GameObject.FindGameObjectsWithTag("handDrag");
        //Debug.Log("empty hand bool?" + handTiles.Length);
        if (handTiles.Length == 1)
        {
            //assign emptyhand bool
            emptyhand = true;
        }
        CheckStamina();
        //Debug.Log("empty hand?" + emptyhand);
    }
    
    public void PlayerClick()
    {
        //check for right click
        if(Input.GetMouseButtonDown(0))
        {
            string clickLoc = "";

            //no longer using ArrayHandler switching to use panel instead
			clickLoc = GameObject.FindGameObjectWithTag("Scripts").GetComponent<ArrayHandler>().FindLocation(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            //clickLoc = this.GetClickLocation(Input.mousePosition);
            //Debug.Log(clickLoc);
			if (clickLoc != "")
			{
	            int temprow = System.Int32.Parse(clickLoc.Substring(0, 1));
	            int tempcol = System.Int32.Parse(clickLoc.Substring(1, 1));
	            //Debug.Log(tempcol);
	            //Debug.Log("mouseClick");
	            //Debug.Log("clickloc !+ " + clickLoc);
	            //Debug.Log(temprow + " " + tempcol);
	            //Debug.Log("test tileboard");
	            //Debug.Log(tileBoard[temprow,tempcol]._isEntrySet);
	            //Debug.Log(tileBoard[temprow,tempcol]._tileID);
	            //Debug.Log("test tileboard");

	            //int plocr = System.Int32.Parse(playerLoc.Substring(0,1));
	            //int plocc = System.Int32.Parse(playerLoc.Substring(1, 1));
	            if ((tileBoard[temprow, tempcol]._isEntrySet) && (playerLoc != "") )
	            {
	                //Debug.Log("clickloc 1: " + clickLoc);
	                //Debug.Log("clickLoc if");
	                if (validMove.MoveDirection(playerLoc, clickLoc) != "invalid move" && validMove.InRange(playerLoc, clickLoc) )
	                {
	                    //Debug.Log("not invalid move");
	                    //valid move
	                    //Debug.Log("clickloc 2: " + clickLoc);
	                    if (validMove.Move(playerLoc, clickLoc,ref tileBoard) )
						{
							int rand = Random.Range (0,movementClips.Length);
							audioSource.PlayOneShot (movementClips[rand], 1.0f);

							//Debug.Log("clickloc 3: " + clickLoc);
	                        int tempIndex = 0;
							cellindex.TryGetValue(clickLoc, out tempIndex);
							movePlayer.UpdatePlayer(gridPanels[tempIndex], validMove.MoveDirection(playerLoc,clickLoc));
	                        //update Playerloc
	                        destLoc = clickLoc;
	                        //Debug.Log("new player loc" + playerLoc + " :: " + clickLoc);
							playerStamina--;
							StartFade (stamDown, "-1", fadeTime);
	                        UpdateUI();
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
    
    public void NewHand()
    {
        handTiles = GameObject.FindGameObjectsWithTag("handDrag");
        if (handTiles != null)
        {
            for (int i =0; i<handTiles.Length;i++)
            {
                Destroy(handTiles[i]);
            }
        }
		StartFade (stamDown, "-4", fadeTime);
        playerStamina += -4;
        GenerateHand();
        CheckStamina();
    }

    public void PlayEvent(string pcell)
    {
        if (tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))]._isActive)
        {
            int temprow = System.Int32.Parse(pcell.Substring(0, 1));
            int tempcol = System.Int32.Parse(pcell.Substring(1, 1));
            //Debug.Log(tileBoard[temprow, tempcol]._tileID);
            //play event = remove stamina and destroy the event clone...
            if (tileBoard[temprow, tempcol]._event == "red")
            {
                stamUp.text = tileBoard[temprow, tempcol].combat.ToString() + playerEquip;
				stamUp.enabled = true;

				int combat = tileBoard[temprow, tempcol].combat;
				int damage = combat + playerEquip;
				
				if (damage > 0) //If player would gain stamina from enemy. Don't want no eating of strange creatures...
				{
					damage = 0;
				}

                switch (tileBoard[temprow, tempcol]._eventItem.ToLower())
                {
                case "snake":
					if (damage == 0)
					{
						snakeDefeatEquip.text = equipment.text;
						DisplayClickPanel (snakeDefeatPanel);
					}else
					{
	                    snakeStamDown.text = damage.ToString();
						DisplayClickPanel (snakePanel);
					}
                    break;
                case "scorpion":
					if (damage == 0)
					{
						scorpDefeatEquip.text = equipment.text;
						DisplayClickPanel (scorpionDefeatPanel);
					}else
					{
	                    scorpStamDown.text = damage.ToString();
						DisplayClickPanel (scorpionPanel);
					}
                    break;
				}

				if (damage != 0) //don't show stam popup if stamina is not reduced
				{
					string newText = damage.ToString();
					StartFade (stamDown, newText, fadeTime);
					playerStamina += damage;
				}

				int rand = Random.Range (0,redTileClips.Length);
				audioSource.PlayOneShot (redTileClips[rand]);

                redstep++;
            }
            else if (tileBoard[temprow, tempcol]._event == "green")
            {
				int equipRand = Random.Range(0, 10); //Chance of 0 to 9 (max (10) is exclusive)
				if (equipRand <= 1) //Chance of 0 or 1 which == 20% (decreased as testing flew through them)
                {
                    //check equipment index and identify new item
                    string item = ""; //old item
                    string itemb = ""; //new item
                    equipmentindex.TryGetValue(playerEquip, out item);
                    //iterate playerequip int and power
                    playerEquip++;
                    equipmentindex.TryGetValue(playerEquip, out itemb);
                    equipDesc.text = ("You've found a " + itemb + ". It feels more durable than your " + item + ".").ToString();
					DisplayClickPanel (equipPanel);
                    //update player equip text
					equipment.text = itemb;
                }
				string newText = "+" + tileBoard[temprow, tempcol].combat.ToString();
				StartFade (stamUp, newText, fadeTime);
                playerStamina += tileBoard[temprow, tempcol].combat;
				
				int rand = Random.Range (0,greenTileClips.Length);
				audioSource.PlayOneShot (greenTileClips[rand]);

                //increment the greens taken
                greencol++;
            }
            GameObject tempObj = GameObject.Find(tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))]._boardLocation + "(Clone)");

            if (tempObj != null)
            {
                Debug.Log("destroy clone");
                Destroy(tempObj);
                tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))]._isActive = false;
            }
            CheckStamina();
        }
    }

	private void DisplayClickPanel (GameObject panel)
	{
		panel.SetActive (true);
		PlayerPrefs.SetString ("Paused", "true");
		StartCoroutine (ClickToClose (panel));
	}

	IEnumerator ClickToClose (GameObject panel)
	{
		while (!Input.GetMouseButtonUp (0))
		{
			yield return null;
		}
		panel.SetActive (false);
		PlayerPrefs.SetString ("Paused", "false");
	}

	private void StartFade (Text stamText, string newText, float time)
	{
		if (faderRunning)
		{
			StopCoroutine ("FadeStamPopup");
		}
		Color newColor = stamText.color;
		newColor.a = 1;
		stamText.color = newColor;
		stamText.text = newText;

		StartCoroutine (FadeStamPopup (stamText, time));
	}

	IEnumerator FadeStamPopup (Text stamText, float time)
    {
		faderRunning = true;
		float alpha = stamText.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
		{
			Color newColor = stamText.color;
			newColor.a = Mathf.Lerp (alpha,0,t);
			stamText.color = newColor;
			yield return null;
		}
		faderRunning = false;
    }
	// not working: parse this!

    public void GenerateBoardLogic()
    {
        //Debug.Log("Tileboard generated for level " + level);
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
                    //Debug.Log(temp + " temp");
                    if (gridPanels[temp].GetComponent<Image>().sprite != null)
                    {
                        temptile = new Tile(gridPanels[temp].GetComponent<Image>().sprite.name.ToString(), row.ToString() + col.ToString());
                        tileBoard[row, col] = temptile;
                        //Debug.Log("tileboard Added " + row+col);
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
                temptile = new EventTile(pair.Value, pair.Key, _eventitem);
                tileBoard[temprow, tempcol] = temptile;
                //using list of tiles for events due to movements 
                //tileEvents.Add(temptile);
                //temptile.test();
            }
            //clear eventindex
            eventindex.Clear();
        }
        //check tileBoard exists
        if (tileBoard.Length != 0)
        {
            //Debug.Log("find exit entrance");
            //check if next level is available exit = bottom of the grid 50-54
            for (int row = 5; row < 6; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    //if tile within bottom row matches exit name
                    if (tileBoard[row, col]._tileID == "tile_exit")
                    {
                        tileBoard[row, col]._event = "exit";
                        //Debug.Log("found tile exit");
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
                        tileBoard[row, col]._event = "entry";
                        playerLoc = row.ToString() + col.ToString();
                        //draw player
                        int pindex = 0;
                        cellindex.TryGetValue(playerLoc, out pindex);
                        movePlayer.DrawPlayer(pindex, gridPanels);
                    }
                }
            }
        }
    }

    public void GenerateHand()
    {       
        //approaching hand generation via grabbing each individual UI element and updating the sprite image and render...didnt work out 13/04
        //actaullyworking just rendered tiny and behind default image too...13/04
        handTiles = GameObject.FindGameObjectsWithTag("handDefault");
		//Debug.Log("handtiles length : " + handTiles.Length);
		btmPanel = GameObject.FindGameObjectWithTag("bottomPanel");
		//check for null
		if (handTiles != null)
		{
			int rand = Random.Range (0,dealingClips.Length);
			audioSource.PlayOneShot (dealingClips[rand], 1.0f);

			for (int i = 0; i < handTiles.Length; i++)
			{
				// Debug.Log(handTiles[i]);
				//set background image as sprite using spriterenderer had sizing/scale issues...
				// handTiles[i].GetComponent<Image>().sprite = tileSprite[Random.Range(0, tileSprite.Length)] as Sprite;
				//clone object 
				//cloning object successful below but doesnt display properly and off propotions.
				//GameObject newObject = Instantiate(handTiles[i],handTiles[i].transform.position,handTiles[i].transform.localRotation) as GameObject;
				GameObject newObject = Instantiate(handTiles[i]);
				newObject.transform.localScale = handTiles[i].transform.localScale;
				newObject.transform.localPosition = handTiles[i].transform.localPosition;
				//set tag so handTiles above doesnt grab clones as well.
				newObject.tag = "handDrag";
				//assign new object correct parents
				newObject.transform.SetParent(btmPanel.transform, false);
				//use handdefaults to instantiate objects with rng sprite below and add script....
				newObject.GetComponent<Image>().sprite = tileSprite[Random.Range(0, tileSprite.Length)] as Sprite;
				newObject.GetComponent<Image>().color = new Color(255f,255f,255f,255f);
				newObject.AddComponent<Draggable>();
				//above method with bool set to false solved instantiating flipped object....   
				//newObject.transform.parent = handTiles[i].transform.parent;
				
				//Debug.Log(handTiles[i].transform.parent + "   ");
				//test[i] = GameObject.Instantiate(handTiles[i], handTiles[i].transform.position, Quaternion.identity);
				// Debug.Log("trying to find component" + handTiles[i] + " :: " + i + " :::" + handTiles.Length + "l");
				//Debug.Log("tilesprite[] " + tileSprite[Random.Range(0, tileSprite.Length)]);
			}
		}
		//trying new method of grabbing BottomPanel panel and adding child comp...
		// BtmPanel = GameObject.FindGameObjectWithTag("bottomPanel");
		//Debug.Log("generatingHand");
	}
	
	public void NextLevel()
	{
		int rand = Random.Range (0,lvlCompClips.Length);
		audioSource.PlayOneShot (lvlCompClips[rand], 0.5f);
		//System.Threading.Thread.Sleep (2000);

        emptyhand = true;
		//Debug.Log("Previous Level " + level);
		level++;
        //initilaise tileBoard list a new
        tileBoard = new Tile[6, 5];
		//degenerate handtiles
		handTiles = GameObject.FindGameObjectsWithTag("handDrag");
		for (int i = 0; i < handTiles.Length; i++)
		{
			Destroy(handTiles[i]);
		}

		//degenerate eventTiles and calculate red tiles avoided
		GameObject[] eventTiles = GameObject.FindGameObjectsWithTag("eventTile");
		for (int i = 0; i < eventTiles.Length; i++)
		{
            redavoid += redtiles - redstep;
			Destroy(eventTiles[i]);
		}

		//degenerate grid tiles
		//gridPanels = GameObject.FindGameObjectsWithTag("GridPanel");
		for (int i=0; i<gridPanels.Length; i++)
		{
			gridPanels[i].GetComponent<Image>().sprite = null;
			gridPanels[i].GetComponent<Image>().color = new Color(255f,255f,255f,0f);
		}

        //PlayerPrefs.SetString ("GeneratedBoard", "false");
		//initialise 
		//Debug.Log("New Level " + level);
		this.Awake();
        for (int row = 0; row <6; row++)
        {
            for (int col = 0; col <5; col++)
            {
                //this.tileBoard[row, col].test();
            }
        }
		CheckStamina ();
	}

    public void GenerateBoard()
    {
        //generate number of green tiles
        //random of 1 to 3 tiles inclusive
        int green = Random.Range(1, 4);
        //Debug.Log("green " + green);

        //generate number of red tiles
        //random of 1 to 3 tiles inclusive
        int red = Random.Range(1, 4);
        //store the rng red into redtiles to use for scoring
        redtiles = red;

        //Debug.Log("red " + red);

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
            //set entrance tile
            gridPanels[upPanel].GetComponent<Image>().sprite = gridSprite[2] as Sprite;
            gridPanels[upPanel].GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);

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
                //Debug.Log("Random" + i + "= " + randomPanels[i]);
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
                panelClone.GetComponent<Image>().sprite = gridSprite[3] as Sprite;
                panelClone.GetComponent<Image>().color = new Color(255f, 255f, 255f, 150f);
                //Debug.Log("event greens :" + panelClone.GetComponent<Image>().sprite.name.ToString() + " ::: " + tempPanel.name);
                eventindex.Add(tempPanel.name, panelClone.GetComponent<Image>().sprite.name.ToString());

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
                panelClone.GetComponent<Image>().sprite = gridSprite[4] as Sprite;
                panelClone.GetComponent<Image>().color = new Color(255f, 255f, 255f, 150f);
                //store event red tiles into eventred list
                eventindex.Add(tempPanel.name, panelClone.GetComponent<Image>().sprite.name.ToString());

            }
        }
    }
}