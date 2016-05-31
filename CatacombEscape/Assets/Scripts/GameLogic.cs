﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


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

    //dictionary to match cell strings of 00-04 10-14 to an index from 0-29
    Dictionary<string, int> cellindex = new Dictionary<string, int>();
    //dictionary to store event tile clone name and original grid panel location int
    Dictionary<string, string> eventindex = new Dictionary<string, string>();
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

	private AudioSource audioSource;
	public AudioClip startGameClip;
	public AudioClip[] placementClips;
	public AudioClip[] dealingClips;
	public AudioClip[] movementClips;
	public AudioClip[] lvlCompClips;

	public Text playerStamUp;
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

		audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource> ();
		audioSource.PlayOneShot (startGameClip, 0.5f);
		
		GameObject mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		if (mainCamera != null)
			mainCamera.GetComponent<BackGroundMusic> ().ResetScript ();
		
		validMove = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<Direction> ();
		movePlayer = GameObject.FindGameObjectWithTag ("Scripts").GetComponent<PlayerMove> ();

        //fill cellindex dictionary
        //temp index int to fill dictonary
        cellindex.Clear();
        int temp = 0;
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
            Debug.Log("emptyhand get hand");
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

        if (gameover)
        {
	        Debug.Log("Game Over");
	        Destroy(gameObject);
	        Application.LoadLevel("Menu");
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

    public void PlayEvent(string pcell)
    {
        if(tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))]._isActive)
        {
            //play event = remove stamina and destroy the event clone...
            playerStamina += tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))].combat;
			GameObject tempObj = GameObject.Find (tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))]._boardLocation + "(Clone)");

			if (tempObj != null)
			{
				Debug.Log (tempObj.transform.name);
				Destroy(tempObj);
				tileBoard[System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1))]._isActive = false;
			}
		}
    }

	// DO not have a function named the same as a type (PlayerMove script!). Also validmove.Move returns a bool... Don't leave something that doesn't work/do anyting still implemented
	/*
    public void PlayerMove(string pNext)
    {
        validMove.Move(playerLoc, pNext ,ref tileBoard);
    }*/

    public void UpdateUI()
    {
		GameObject tempObj = GameObject.FindGameObjectWithTag("PlayerStam");
		tempObj.GetComponent<Text>().text = playerStamina + "/100";
        tempObj = GameObject.FindGameObjectWithTag("StamBar");
		tempObj.GetComponent<Slider>().value = playerStamina;
        if (playerStamina <= 0)
        {
            Debug.Log("GameOver " + gameover);
            gameover = true;
        }
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
            //add eventgreen eventred
            foreach (KeyValuePair<string, string> pair in eventindex)
            {
                string tempstring = pair.Key.ToString();
                int temprow = System.Int32.Parse(tempstring.Substring(0, 1));
                int tempcol = System.Int32.Parse(tempstring.Substring(1, 1));
                temptile = new Tile(pair.Value, pair.Key);
                tileBoard[temprow, tempcol] = temptile;
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
						Debug.Log("Found entrance and set player");
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

		//degenerate eventTiles
		GameObject[] eventTiles = GameObject.FindGameObjectsWithTag("eventTile");
		for (int i = 0; i < eventTiles.Length; i++)
		{
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
                //store event green tile into eventgreen list
                eventindex.Clear();
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