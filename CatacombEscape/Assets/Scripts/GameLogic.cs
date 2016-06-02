using UnityEngine;
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

    public Text Equipment;
    public Text equipDesc;
	public GameObject stamUpContainer;
	public Text playerStamUp;
	public GameObject stamDownContainer;
	public Text playerStamDown;
	public GameObject equipPanel;
	public GameObject snakePanel;
	public Text snakeStamDown;
	public GameObject scorpionPanel;
	public Text scorpStamDown;
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
	// These can be accessed and set with 'string tot = pointTot.text;' and 'pointTot.text = "100"'

    //awake called behind start
    void Awake()
    {
		Debug.Log("GameLogic awake");
        //refresh and initialse redstep per awake call
        redstep = 0;
		audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource> ();
		audioSource.PlayOneShot (startGameClip, 0.5f);
		
		GameObject mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		if (mainCamera != null)
			mainCamera.GetComponent<BackGroundMusic> ().ResetScript ();
		
		validMove = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<Direction> ();
		movePlayer = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<PlayerMove> ();
        //creating equipment index
        equipmentindex.Clear();
        int temp = 0;
        for (int i = 0; i < 3; i++)
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
	}

	// Use this for initialization
	void Start()
	{
		Debug.Log("GameLogic start");
    }

    // Update is called once per frame
    void Update()
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

        if (nextlevel)
        {
            nextlevel = false;
            //PlayerPrefs.SetString("GeneratedBoard", "false");
            NextLevel();
        }
        CheckStamina();
        if (gameover)
        {
            Debug.Log("Gameover");
            GameOverHS();
            statPanel.SetActive(true);
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

	// Called by PlayerMove once the character animation is complete, to stop events from occuring before player has stopped on the destination tile
	public void SetPlayerLoc()
	{
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
		}
        //move events
        MoveEvents();
        CheckStamina ();
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
        greenPointTot.text = (greencol * 10).ToString();
        redAvoided.text = redavoid.ToString();
        redPointTot.text = (redavoid * 20).ToString();
        tileNoPlaced.text = (tileplaced).ToString();
        tileNoTot.text = (tileplaced * -0.56).ToString();
        //highscore = (level * 100) + (greencol * 10) + (redavoid * 20) + (tileplaced * -.56);
        //pointTot.text = highscore.ToString("0");
		float temp = tileplaced * -0.56f;
        tileNoTot.text = (Mathf.Round(temp)).ToString();
		score = ((level-1) * 100) + (greencol * 10) + (redavoid * 20) + Mathf.Round(temp);
		if (score < 0)
			score = 0;
		pointTot.text = score.ToString();

		string hs = PlayerPrefs.GetString("HighScore");
		if (hs != null && hs != "")
		{
			int highscore = int.Parse (hs);
			if (score > highscore)
			{
				PlayerPrefs.SetString("HighScore", score.ToString());
				// Claire -> make text object to notify player of new highscore
			}
		}else if (score > 0) //A score of 0 doesn't count as a high score the first time played
		{
			PlayerPrefs.SetString("HighScore", score.ToString());
		}
    }
    //try again button for the statpanel disable statpanel and destory gameObject and load the new scene.
    public void TryAgain()
    {
        Debug.Log("Try again");
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
			Debug.Log("GameOver " + gameover);
			gameover = true;
		}
        else if (playerStamina > 100 )
        {
            playerStamina = 100;
        }
	}
    public void MoveEvents()
    {
        int move = Random.Range(0, 2);
        Debug.Log("move event");
        string etloc = "";
        string newloc = "";
        int panelkey = 0;
        int currow = 0;
        int curcol = 0;
        int newrow = 0;
        int newcol = 0;
        bool vertmove = false;
        bool horimove = false;

        GameObject[] eventTiles = GameObject.FindGameObjectsWithTag("eventTile");
        for(int i =0; i<eventTiles.Length;i++)
        {
            //for red tiles only
            if (eventTiles[i].GetComponent<Image>().sprite.name == "event_red")
            {
                etloc = eventTiles[i].name.Substring(0, 2);
                System.Int32.TryParse(etloc.Substring(0, 1), out currow);
                System.Int32.TryParse(etloc.Substring(1, 1), out curcol);
                newrow = currow + (Random.Range(-1, 1));
                newcol = curcol + (Random.Range(-1, 1));
                if (newrow <=5 && newrow >=0 && newrow != currow)
                {
                    vertmove = true;
                }
                if (newcol <= 4 && newcol >=0 && newcol != curcol)
                {
                    horimove = true;
                }
                Debug.Log(newrow + " : " +currow + " @@@@ " + newcol + " : " + curcol);
                //if new row is within bounds and not the same as current move tile 
                if (vertmove && (move == 0) )
                {
                    //check the new location is not already an event or the player/exits
                    Tile dummy = new Tile(0);
                    Debug.Log("Move Vertical");
                    newloc = newrow.ToString() + curcol.ToString();
                    cellindex.TryGetValue(newloc, out panelkey);
                    Debug.Log(newcol + "newcol " + curcol + " curcol " + newrow + " newrow " + currow + " currow");
                    if ((tileBoard[newrow, curcol]._tileID != "tile_exit") && (tileBoard[newrow, curcol]._tileID != "tile_entrance")  && (newloc != playerLoc) && (tileBoard[newrow, curcol]._event != "green") && (tileBoard[newrow, curcol]._event != "red"))
                    {
                        //move the tile
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
                        tileBoard[newrow, curcol].UpdatePosition(dummy);
                        //update objclone name to be used for destroying the game obj
                        eventTiles[i].name = newloc + "(Clone)";
                    }
                }
                //else if horizontal check
                else if (horimove && (move ==1))
                {
                    Tile dummy = new Tile(0);
                    Debug.Log("Move horizontal");
                    newloc = currow.ToString() + newcol.ToString();
                    cellindex.TryGetValue(newloc, out panelkey);
                    Debug.Log(newcol + "newcol " + curcol + " curcol " + newrow + " newrow " + currow + " currow");
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
                //else do nothing
            }
        }
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

    public bool ValidDrag( Tile ptile, string pcell)
    {
        int rowmove = 0;
        int colmove = 0;
        bool ValidDrag = false;
        Debug.Log("Valid Drag");
        //Debug.Log("Player Loc: " + playerLoc);
        rowmove = Mathf.Abs((System.Int32.Parse(pcell.Substring(0, 1))) - (System.Int32.Parse(playerLoc.Substring(0, 1))));
        colmove = Mathf.Abs((System.Int32.Parse(pcell.Substring(1, 1))) - (System.Int32.Parse(playerLoc.Substring(1, 1))));
        //Debug.Log("rowmove :" + rowmove);
        //Debug.Log("colmove :" + colmove);
        //check that the drag to is within range of player location 1 unit vertically or horizontally if so ValidPlacement to see if a clear path is available
        //if the difference between both cell's row is abs 1 moving vertically
        if (rowmove == 1)
        {
            //if the diff between both cells col is abs 0 to move vertically
            if (colmove == 0 )
            {
                //check if path is available
                if (validMove.ValidPlacement(playerLoc, ptile) )
                {
                    //Debug.Log("valid vert movement");
                    ValidDrag = true;
                }
            }
        }
        //moving horizontally
        else if (rowmove == 0)
        {
            if (colmove == 1)
            {
                //check if path is available
                if (validMove.ValidPlacement(playerLoc,ptile) )
                {
                    //Debug.Log("Valid hori movement");
                    ValidDrag = true;
                }
            }
        }
        return ValidDrag;
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
        playerStamDown.text = "-4";
        StartCoroutine(StamPopup(stamDownContainer));
        playerStamina += -4;
        GenerateHand();
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
                playerStamUp.text = tileBoard[temprow, tempcol].combat.ToString() + playerEquip;
                playerStamUp.enabled = true;
                switch (tileBoard[temprow, tempcol]._eventItem.ToLower())
                {
                    case "snake":
                        {
                            snakeStamDown.text = tileBoard[temprow, tempcol].combat.ToString();
                            StartCoroutine(eventWait(snakePanel));
                            playerStamDown.text = tileBoard[temprow, tempcol].combat.ToString();
                            StartCoroutine(StamPopup(stamDownContainer));
                            playerStamina += tileBoard[temprow, tempcol].combat + playerEquip;
                            break;
                        }
                    case "scorpion":
                        {
                            scorpStamDown.text = tileBoard[temprow, tempcol].combat.ToString();
                            StartCoroutine(eventWait(scorpionPanel));
                            playerStamDown.text = tileBoard[temprow, tempcol].combat.ToString();
                            StartCoroutine(StamPopup(stamDownContainer));
                            playerStamina += tileBoard[temprow, tempcol].combat + playerEquip;
                            break;
                        }
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
                    string item = "";
                    string itemb = "";
                    equipmentindex.TryGetValue(playerEquip, out item);
                    //iterate playerequip int and power
                    playerEquip++;
                    equipmentindex.TryGetValue(playerEquip, out itemb);
                    equipDesc.text = ("You've found a " + itemb + ". It feels more durable than your " + item + ".").ToString();
                    StartCoroutine(eventWait(equipPanel,3));
                    //update player equip text
                    Equipment.text = itemb;
                }
                playerStamUp.text = tileBoard[temprow, tempcol].combat.ToString();
                StartCoroutine(StamPopup(stamUpContainer));
                playerStamina += tileBoard[temprow, tempcol].combat;
                CheckStamina();
				
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
        }
    }
    IEnumerator eventWait(GameObject pType)
    {
        pType.SetActive(true);
        yield return new WaitForSeconds(2f);
        pType.SetActive(false);
    }
    IEnumerator eventWait(GameObject pType, int pt)
    {
        pType.SetActive(true);
        yield return new WaitForSeconds(pt);
        pType.SetActive(false);
    }
    IEnumerator StamPopup(GameObject pType)
    {
        pType.SetActive(true);
        yield return new WaitForSeconds(4f);
        pType.SetActive(false);
    }

    public void GenerateBoardLogic()
    {
        Debug.Log("Tileboard generated for level " + level);
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
		Debug.Log("New Level " + level);
		this.Awake();
        for (int row = 0; row <6; row++)
        {
            for (int col = 0; col <5; col++)
            {
                //this.tileBoard[row, col].test();
            }
        }
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