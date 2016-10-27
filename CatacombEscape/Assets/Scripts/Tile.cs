using UnityEngine;
using System.Collections.Generic;

public class Tile 
{
    public List<string> _entry { get; set; }
    public string _eventItem { get; set; }
    public string _tileID {get; set;}
    public bool _isOccupied { get; set; }
    public bool _isActive { get; set; }
	public string _nextMove = "";				// Used to show the enemy's movement direction to the player.
	public bool _isEntrySet { get; set; }	// Is used to check if tile is player-placed
    public bool _isDummy { get; set; }
    public string _event { get; set; }
    public int combat { get; set; }
    public string _boardLocation { get; set; }
    public int heuristic = 0;
    public int total = 0;
    public int cost = 0;

	protected Tile() { _isEntrySet = false; }

    public Tile(List<string> pEntry, string pID , string pboardloc)
    {
        //Debug.Log("tile 3param");
        _entry = pEntry;
        _tileID = pID.ToLower();
        _isOccupied = false;
        _isActive = true;
        _isEntrySet = true;
        _event = "";
        _isDummy = false;
        _boardLocation = pboardloc;
    }

    //constructor to automatically generate _entry based on id
    public Tile(string pID , string pboardloc)
    {
        //Debug.Log("tile 1 param");
        _tileID = pID.ToLower();
        _isActive = true;
        _isDummy = false;
        _isOccupied = false;
        _event = "";
        _boardLocation = pboardloc;
        //initiate _entry
        _entry = new List<string>();

        if (_tileID.Contains("event") )
        {
            //Debug.Log("If gen event");
            _isEntrySet = false;
            GenerateEvent();
        }
        else
        {
            //Debug.Log("else gen entry");
            GenerateEntry();
        }

    }

    public Tile(int pint)
    {
        //dummy constructor
        _isActive = false;
        _isDummy = true;
        _event = "";
    }
    //flush entry to move array around
    public void FlushEntry()
    {
        if (_entry != null)
        {
            _entry.Clear();
        }
    }
    //change tile positions for events
    public void UpdatePosition(Tile pTile)
    {
        _eventItem = pTile._eventItem;
        combat = pTile.combat;
        _event = pTile._event;
        _isActive = pTile._isActive;

    }
    public void ClearEvent()
    {

		if (_tileID == "event_red"){
			_tileID = null;
		}
        _event = "";
        _eventItem = "";
		_nextMove = "";
		_isActive = false;
        combat = 0;

    }
    //move validator
    public bool ValidMove(string pMove)
    {
        bool move = false;
        switch (pMove)
        {
            case "up":
                {
                    if (_entry.Contains("up"))
                    {
                        move = true;
                    }
                    break;
                }
            case "right":
                {
                    if (_entry.Contains("right"))
                    {
                        move = true;
                    }
                    break;
                }
            case "down":
                {
                    if (_entry.Contains("down"))
                    {
                        move = true;
                    }
                    break;
                }
            case "left":
                {
                    if (_entry.Contains("left"))
                    {
                        move = true;
                    }
                    break;
                }
        }
        return move;
    }

    /// <summary>
    /// Ensure entry tile is valid
    /// Is an entryTile always guaranteed to have at least one valid entry?
    /// </summary>
    /// <param name="pEntry"></param>
    /// <returns></returns>
    public bool ValidEntry(string pEntry)
    {
        bool entry = false;
        if (_isEntrySet)
        {
            switch (pEntry.ToLower())
            {
                case "up":
                    {
                        if (_entry.Contains("down"))
                        {
                            entry = true;
                        }
                        break;
                    }
                case "left":
                    {
                        if(_entry.Contains("right"))
                        {
                            entry = true;
                        }
                        break;
                    }
                case "down":
                    {
                        if (_entry.Contains("up"))
                        {
                            entry = true;
                        }
                        break;
                    }
                case "right":
                    {
                        if (_entry.Contains("left"))
                        {
                            entry = true;
                        }
                        break;
                    }
            }
        }
        //Debug.Log("return valid entry "+entry);
        return entry;
    }

    /// <summary>
    /// Creates the tile the player starts the level on.
    /// </summary>
    private void GenerateEntry()
    {
        //Debug.Log("start entry for " + _tileID);
        //int to hold start and end of indexof(_) 
        int con_start = 0;
        int con_end = 0;
        //temp string holder for storing directions
        string dir = "";
        //check if the tile being create is the entry/exit tile...
        if (this._tileID == "tile_entrance" || this._tileID == "tile_exit"  )
        {
            _entry.Add("up");
            _entry.Add("right");
            _entry.Add("down");
            _entry.Add("left");
            _isEntrySet = true;
        }
        //loop to grab and and add to _entry of directional pathways based on _tileID
        //check if con_end is at the last _ location for the condition
        //this allows us to grab everything inbetween _ and jump out leaving the 1 out
        else
        {
            while (con_end != _tileID.LastIndexOf("_"))
            {
                con_start = this._tileID.IndexOf("_", con_end);
                //+1 the search for index so it doesnt look at the same _ that it starts from
                con_end = this._tileID.IndexOf("_", con_start + 1);

                //+1 to start from after the first _ 
                //2nd param is a counter thus con_end - con_start and -1 to start before 2nd _
                dir = this._tileID.Substring((con_start + 1), (con_end - con_start) - 1);

                //check if dir is up right down left    
                if (dir == "up" || dir == "right" || dir == "down" || dir == "left")
                {
                    this._entry.Add(dir);
                    _isEntrySet = true;
                }
                else
                {
                    //Debug.Log("bad " + dir);
                    _isEntrySet = false;
                }
            }

            //grab the final entry
            //using con_end and string.length
            dir = this._tileID.Substring(con_end + 1, (this._tileID.Length - con_end - 1));
            if (dir == "up" || dir == "right" || dir == "down" || dir == "left")
            {
                this._entry.Add(dir);
                //Debug.Log("good " + dir);
                _isEntrySet = true;
            }
            else 
            {
                //Debug.Log("bad " + dir);
                _isEntrySet = false;
            }
        }
    }

    /// <summary>
    /// Create events (Chests, enemies)
    /// </summary>
    public void GenerateEvent()
    {
		TutorialLogic tutorialLogic = null;
		GameLogic gameLogic = null;

		//Debug.Log("Generate Event");
		if (PlayerPrefs.GetString ("TutorialScene") == "true")
			tutorialLogic = GameObject.FindObjectOfType<TutorialLogic> ();
		else
			gameLogic = GameObject.FindObjectOfType<GameLogic> ();
		
        int con_start;
        string dir = "";
        con_start = this._tileID.IndexOf("_");
        //Debug.Log("after start " + con_start);
        //Debug.Log("after end " + con_end);
        //Debug.Log("string length " + this._tileID.Length);
        //+1 to start from after the first _ 
        //2nd param is a counter thus con_end - con_start and -1 to start before 2nd _
        dir = this._tileID.Substring((con_start + 1));
        //Debug.Log("event dir " + dir);

        if (dir == "green")
        {
            _event = "green";
			if (PlayerPrefs.GetString ("TutorialScene") == "true")
				combat = tutorialLogic.greenAmt;
			else
				combat = gameLogic.greenAmt;
        }
        else if (dir == "red")
        {
            _event = "red";
			if (PlayerPrefs.GetString ("TutorialScene") == "true")
				combat = -(tutorialLogic.initRedDmg) - 1; //enemy strength increments by 1 each level
			else
				combat = -(gameLogic.initRedDmg) - gameLogic.GetLevel; //enemy strength increments by 1 each level
        }
    }

    //primary used for generating event tiles updating event tiles...
    public void UpdateTile(Tile pTile)
    {
        //Debug.Log("UpdateTile");
        _tileID = pTile._tileID;
        _isActive = pTile._isActive;
        _entry = new List<string>();
        _isDummy = pTile._isDummy;
        _isOccupied = pTile._isOccupied;
        _boardLocation = pTile._boardLocation;
        GenerateEntry();
    }

    /// <summary>
    /// Replicates a target tile
    /// </summary>
    /// <param name="pTileb"></param>
    public void CloneTile(Tile pTileb)
    {
        this.combat = pTileb.combat;
        this._boardLocation = pTileb._boardLocation;
        this._event = pTileb._event;
        this._eventItem = pTileb._eventItem;
        this._isDummy = pTileb._isDummy;
        this._isActive = pTileb._isActive;
        this._tileID = pTileb._tileID;
    }

    /// <summary>
    /// Is not currently used
    /// </summary>
    public void test()
    {
        //if (_isDummy == false)
        //{
            Debug.Log(" start of test_tileID : " + _tileID);
            Debug.Log("isOccupied : " + _isOccupied);
            Debug.Log("isActive : " + _isActive);
            Debug.Log("is set :" + _isEntrySet);
            Debug.Log("is event: " + _event);
            Debug.Log("is eventitem: " + _eventItem);
            Debug.Log("is combat: " + combat);
            Debug.Log("is location " + _boardLocation);
            if (_entry != null)
            {
                for (int i = 0; i < _entry.Count; i++)
                {
                    Debug.Log(_entry[i]);
                }
                Debug.Log("end of test is in loc :" + _boardLocation);
            }
        //}
    }

    public void OnMouseOver()
    {
        Debug.Log("Over a tile.");
    }

    public void toString(string info)
    {
        Debug.Log(info +" | >>> Tile ID: " + _tileID + " >>> Location: " + _boardLocation + ">>> Is active? " + _isActive);
    }
}