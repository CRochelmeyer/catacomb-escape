using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

    private Direction _entry { get; set; }
    private string _tileID { get; set; }
    private bool _isOccupied { get; set; }
    private bool _isActive { get; set; }


    public Tile(Direction pEntry, string pID)
    {
        _entry = pEntry;
        _tileID = pID;
        _isOccupied = false;
        _isActive = true;
    }
    
}
