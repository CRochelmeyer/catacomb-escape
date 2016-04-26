using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

    private List<string> _entry { get; set; }
    private string _tileID { get; set; }
    private bool _isOccupied { get; set; }
    private bool _isActive { get; set; }


    public Tile(List<string> pEntry, string pID)
    {
        _entry = pEntry;
        _tileID = pID;
        _isOccupied = false;
        _isActive = true;
    }
    //move validator
    public void ValidMove(string pMove)
    {
        switch(pMove.ToUpper())
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
    public void test()
    {
        Debug.Log("entry : " +_entry );
        Debug.Log("_tileID : " + _tileID);
        Debug.Log("isOccupied : " + _isOccupied);
        Debug.Log("isActive : " + _isActive);
        ValidMove("up");
    }
}
