using UnityEngine;
using System.Collections.Generic;

public class Tile  {

    public List<string> _entry { get; set; }
    public string _tileID {get; set;}
    public bool _isOccupied { get; set; }
    public bool _isActive { get; set; }


    public Tile(List<string> pEntry, string pID)
    {
        Debug.Log("tile 2param");
        _entry = pEntry;
        _tileID = pID.ToLower();
        _isOccupied = false;
        _isActive = true;
    }
    //constructor to automatically generate _entry based on id
    public Tile(string pID)
    {
        Debug.Log("tile 1 param");
        _tileID = pID.ToLower();
        _isActive = true;
        _isOccupied = false;
        //initiate _entry
        _entry = new List<string>();
        generateEntry();

    }
    //move validator
    public void ValidMove(string pMove)
    {
        switch (pMove.ToUpper())
        {
            case "UP":
                {
                    _entry.Contains("UP");
                    Debug.Log("Possible up");
                    break;
                }
            case "RIGHT":
                {
                    _entry.Contains("RIGHT");
                    Debug.Log("Possible right");
                    break;
                }
            case "DOWN":
                {
                    _entry.Contains("DOWN");
                    Debug.Log("Possible down");
                    break;
                }
            case "LEFT":
                {
                    _entry.Contains("LEFT");
                    Debug.Log("Possible left");
                    break;
                }
        }
    }

    private void generateEntry()
    {
        Debug.Log("start entry");
        //int to hold start and end of indexof(_) 
        int con_start = 0;
        int con_end = 0;
        //temp string holder for storing directions
        string dir = "";
        //loop to grab and and add to _entry of directional pathways based on _tileID
        //check if con_end isat the last _ location for the condition
        //this allows us to grab everything inbetween _ and jump out leaving the 1 out
        while (con_end != _tileID.LastIndexOf("_"))
        {
             Debug.Log("start loop ");
            //grab index of _ from con_end/con_start indexes initially 0
            //+1 to make sure the new start isnt looking at the old end_
            Debug.Log("before start "+con_start);
            Debug.Log("before end "+con_end);
            con_start = this._tileID.IndexOf("_", con_end);
            //+1 the search for index so it doesnt look at the same _ that it starts from
            con_end = this._tileID.IndexOf("_", con_start + 1);
            Debug.Log("after start " + con_start);
            Debug.Log("after end " + con_end);
            Debug.Log("string length " + this._tileID.Length);
            //+1 to start from after the first _ 
            //2nd param is a counter thus con_end - con_start and -1 to start before 2nd _
            dir = this._tileID.Substring((con_start + 1), (con_end - con_start) - 1);
            //check if dir is up right down left    
            if (dir == "up" || dir == "right" || dir == "down" || dir == "left")
            {
                this._entry.Add(dir);
                Debug.Log("good " + dir);
            }
            else
            {
                Debug.Log("bad " + dir);
            }
            Debug.Log("lastindex" + _tileID.LastIndexOf("_"));
        }
        //grab the final entry
        //using con_end and string.length
        dir = this._tileID.Substring(con_end + 1, (this._tileID.Length - con_end - 1) );
        if (dir == "up" || dir == "right" || dir == "down" || dir == "left")
        {
            this._entry.Add(dir);
            Debug.Log("good " + dir);
        }
        else
        {
            Debug.Log("bad " + dir);
        }

    }
    public void test()
    {
        Debug.Log("entry : " +_entry );
        Debug.Log("_tileID : " + _tileID);
        Debug.Log("isOccupied : " + _isOccupied);
        Debug.Log("isActive : " + _isActive);
        for(int i =0; i< _entry.Count; i++)
        {
            Debug.Log(_entry[i]);
        }
    }

}
