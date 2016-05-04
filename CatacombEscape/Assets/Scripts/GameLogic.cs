using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class GameLogic : MonoBehaviour
{
    //boolean game conditions
    private bool gameover = false;
    private bool emptyhand = false;
    private bool nextlevel = false;


    //dictionary to match cell strings of 00-04 10-14 to an index from 0-29
    Dictionary<string, int> cellindex = new Dictionary<string, int>();
    //dictionary to store event tile clone name and original grid panel location int
    Dictionary<string, string> eventindex = new Dictionary<string, string>();
    //sprite holders drag sprites via inspector
    public Sprite[] tileSprite;
    public Sprite[] gridSprite;
    public GameObject btmPanel;
    public GameObject[] handTiles;
    public GameObject tempObj;
    //create tile gameboard 2d array
    public Tile[,] gameBoard = new Tile[5, 6];
    //initiate a static instance of gamelogic to be used globally...
    public static GameLogic instance = null;
    private int level = 1;
	private GridPanels gridPanelsScript;
	private GameObject[] gridPanels;
    private Tile[,] tileBoard;

    //awake called behind start
    void Awake()
    {
        Debug.Log("GameLogic awake");
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
        DontDestroyOnLoad(gameObject);

		FindGridPanels();

		PlayerPrefs.SetString ("Paused", "false");

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
	}

	// Use this for initialization
	void Start()
	{
		Debug.Log("GameLogic start");
        Debug.Log(" show arrays");
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                this.tileBoard[row, col].test();
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        //check if hand is empty, draw it using tag handDrag
        handTiles = GameObject.FindGameObjectsWithTag("handDrag");
        if (handTiles.Length == 0)
        {
            //assign emptyhand bool
            emptyhand = true;
        }
        //check if handTiles has been filled
        if (emptyhand)
        {
            //generate hand
            GenerateHand();
        }
            //check if next level...
            if (nextlevel)
            {
                NextLevel();
            }

        
    }
    //init game method
    void InitGame(int pLevel)
    {
        //setup board with rng sprites
        GenerateBoard(pLevel);
        //generate hand 
        GenerateHand();
	}

	void FindGridPanels()
	{
        gridPanelsScript = GameObject.FindGameObjectWithTag ("GridPanelsScript").GetComponent<GridPanels> ();
		gridPanels = new GameObject[30];

		for (int i = 0; i < gridPanels.Length; i++)
		{
			gridPanels[i] = gridPanelsScript.GetGridPanel(i);
            //Debug.Log(gridPanels[i].name); all gridpanels are moved it 
		}
        Debug.Log("gridpanelsFound");
        
    }

    public void UpdateDrag(Tile ptile , string pcell)
    {
        //grab index based on pcell in cellindex dictionary
        int _gridIndex =0;
        int _spriteIndex =0;
        cellindex.TryGetValue(pcell, out _gridIndex);
        //Debug.Log("_index :" + _gridIndex);
        //Debug.Log("tile id :" + ptile._tileID.ToString());
        //grab corresponding gridsprite[index] based on ptile
        for (int i = 0; i < tileSprite.Length; i++)
        {
            string temp1 = tileSprite[i].name.ToString();
            string temp2 = ptile._tileID.ToString();
            if (string.Compare(temp1, temp2) == 0 )
            {
                _spriteIndex = i;
                gridPanels[_gridIndex].GetComponent<Image>().sprite = tileSprite[_spriteIndex] as Sprite;
                gridPanels[_gridIndex].GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
                break;
            }
        }
    }

	public void GenerateBoardLogic()
	{
        Debug.Log("tileboard generated ::" + level);
        Tile temptile;
        int temp = 0;
        //check if tileboard is empty
        if (tileBoard.Length != 0)
        {
            //generate tileboard...
            Debug.Log("tileboard gen if");
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    cellindex.TryGetValue(row.ToString() + col.ToString(), out temp);
                    if (gridPanels[temp].GetComponent<Image>().sprite != null)
                    {
                        temptile = new Tile(gridPanels[temp].GetComponent<Image>().sprite.name.ToString(), row.ToString() + col.ToString());
                        tileBoard[row, col] = temptile;
                        Debug.Log("tileboard Added " + row+col);
                    }
                }
            }
            //add eventgreen eventred
            foreach (KeyValuePair<string, string> pair in eventindex)
            {
                string tempstring = pair.Key.ToString();
                Debug.Log(tempstring);
                int temprow = System.Int32.Parse(tempstring.Substring(0, 1));
                int tempcol = System.Int32.Parse(tempstring.Substring(1, 1));
                temptile = new Tile(pair.Value, pair.Key);
                tileBoard[temprow, tempcol] = temptile;
                Debug.Log("added event tiles");
                Debug.Log("temp row " + temprow + " tempcol " + tempcol);
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
                    if (tileBoard[row, col]._tileID == "tile_entrance_exit")
                    {
                        Debug.Log("found tile exit");
                        if (tileBoard[row, col]._isOccupied)
                        {
                            Debug.Log("next levle set to true");
                            //nextlevel bool = true;
                            nextlevel = true;
                        }
                    }
                }
            }
            //find entrance and set player location
            for (int row =0; row<1; row++)
            {
                for (int col = 0; col<5; col++)
                {
                    if (tileBoard[row,col]._boardLocation == "tile_entrance_exit")
                    {
                        tileBoard[row, col]._isOccupied = true;
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
		Debug.Log("generatingHand");
	}
	
	public void NextLevel()
	{
		Debug.Log("Previous Level " + level);
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
		//initialise 
		Debug.Log("New Level " + level);
		this.Awake();
        for (int row = 0; row <6; row++)
        {
            for (int col = 0; col <5; col++)
            {
                this.tileBoard[row, col].test();
            }
        }
	}

    public void GenerateBoard(int level)
    {
        //generate number of green tiles
        //random of 1 to 3 tiles inclusive
        int green = Random.Range(1, 4);
        Debug.Log("green " + green);

        //generate number of red tiles
        //random of 1 to 3 tiles inclusive
        int red = Random.Range(1, 4);
        Debug.Log("red " + red);

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