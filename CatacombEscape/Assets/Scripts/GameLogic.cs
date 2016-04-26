using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLogic : MonoBehaviour
{
    int interval = 1;
    float nextTime = 0;
    //sprite holders drag sprites via inspector
    public Sprite[] tileSprite;
    public Sprite[] gridSprite;
    public GameObject btmPanel;
    public GameObject[] handTiles;
    public GameObject boardHolder;
    //create tile gameboard 2d array
    public Tile[,] gameBoard = new Tile[5, 6];
    //initiate a static instance of gamelogic to be used globally...
    public static GameLogic instance = null;

    private int level = 1;
	private GridPanels gridPanelsScript;
	private GameObject[] gridPanels;

    //awake called behind start
    void Awake()
    {
        Debug.Log("GameLogic awake");
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

		PlayerPrefs.SetString ("Paused", "true");

        InitGame(level);
	}

	// Use this for initialization
	void Start()
	{
		Debug.Log("GameLogic start");
	}
	
	// Update is called once per frame
	void Update()
	{
		//do this every second
		/*if (Time.time >= nextTime)
        {
            Debug.Log("mouse x: " + Input.mousePosition.x + " mouse y: " + Input.mousePosition.y);
            nextTime += interval;
        }*/
	}

    //init game method
    void InitGame(int pLevel)
    {
        //setup board with rng sprites
        GenerateBoard(pLevel);
	}

	void FindGridPanels()
	{
		gridPanelsScript = GameObject.FindGameObjectWithTag ("GridPanelsScript").GetComponent<GridPanels> ();
		gridPanels = new GameObject[30];

		for (int i = 0; i < gridPanels.Length; i++)
		{
			gridPanels[i] = gridPanelsScript.GetGridPanel(i);
		}

		/*
		GameObject[] tempPanels;
		GameObject gridPanel = GameObject.FindGameObjectWithTag("GridPanel");

		tempPanels =


		var objects = new Array();
		RigidBody[] bodies;
		void Example()
		{
			bodies = GetComponentsInChildren<RigidBody>();
			foreach(RigidBody body in bodies) {
				objects.Add(body.gameObject);
			}
		}*/
	}

	public void TestPassing(string pImageId, float px, float py)
	{
		Debug.Log("image id " + pImageId + " x y " + px + " " + py);
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
		//degenerate handtiles
		handTiles = GameObject.FindGameObjectsWithTag("handDrag");
		for (int i = 0; i < handTiles.Length; i++)
		{
			Destroy(handTiles[i]);
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
	}

    public void GenerateBoard(int level)
    {
        //simple alg for #of green/red tiles based on level
        //using int to truncate .decimals
        //int green = (int) (level * 1.5f);
		int green = Random.Range(1, 4);
        Debug.Log("green " + green);
        //int red = (int)(level * .5);
		int red = Random.Range(1, 4);
        Debug.Log("red " + red);
        //grab grid panels
        //gridPanels = GameObject.FindGameObjectsWithTag("GridPanel");
        if (gridPanels != null)
        {
           /* for (int i = 0; i < gridPanels.Length; i++)
            {
                Debug.Log("gridpanels I : " + gridPanels[i] + " ::: " + i + " spritename : "+gridPanels[5].GetComponent<Image>().sprite);
            }*/
			int downPanel = Random.Range(25, 30);
			int upPanel = Random.Range(0, 5);
			int[] randomPanels = new int[green+red];
			Debug.Log("green + red = " + randomPanels.Length);

			for (int i=0; i< randomPanels.Length; i++)
				randomPanels[i] = 30;
			
			//set ladder up tile
            gridPanels[downPanel].GetComponent<Image>().sprite = gridSprite[1] as Sprite;
			gridPanels[downPanel].GetComponent<Image>().color = new Color(255f,255f,255f,255f);
			//set ladder down tile
			gridPanels[upPanel].GetComponent<Image>().sprite = gridSprite[2] as Sprite;
			gridPanels[upPanel].GetComponent<Image>().color = new Color(255f,255f,255f,255f);

			//set random panel numbers for red and green tile placement
			for (int i=0; i< randomPanels.Length; i++)
			{
				while( randomPanels[i] == 30 )
				{
					int randNo = Random.Range(0, 30);
					if (randNo != downPanel && randNo != upPanel)
					{
						randomPanels[i] = randNo;
						for (int j=0; j<i; j++)
						{
							if (randNo == randomPanels[j] )
							{
								randomPanels[i] = 30;
								break;
							}else if (j == i-1)
								randomPanels[i] = randNo;
						}

					}
				}
				Debug.Log("Random" + i + "= " + randomPanels[i]);
			}

			//Draw all green tiles
            for (int i =0; i< green; i++)
			{
				gridPanels[randomPanels[i]].GetComponent<Image>().color = new Color(255f,255f,255f,150f);
				gridPanels[randomPanels[i]].GetComponent<Image>().sprite = gridSprite[3] as Sprite;
            }
			//Draw all red tiles
            for (int i = 0+green; i < red+green; i++)
            {
            	//if (gridPanels[Random.Range(2, gridPanels.Length)].GetComponent<Image>().sprite)
				//{
					gridPanels[randomPanels[i]].GetComponent<Image>().color = new Color(255f,255f,255f,150f);
					gridPanels[randomPanels[i]].GetComponent<Image>().sprite = gridSprite[4] as Sprite;
				//}
            }
            
        }

    }
}
