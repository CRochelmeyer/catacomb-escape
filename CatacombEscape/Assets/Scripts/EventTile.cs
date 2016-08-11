///
/// Is this class for the chest and enemy tiles?
///
using UnityEngine;
using System.Collections;

public class EventTile : Tile
{
    public EventTile(string pID, string pboardloc, string pType)
    {
        _eventItem = pType;
        _boardLocation = pboardloc;
        _tileID = pID;
        _isEntrySet = false;
        _isOccupied = false;
        _isActive = true;
        _isDummy = false;
        GenerateEvent();
    }
}
