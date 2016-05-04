using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Direction : MonoBehaviour {
    Dictionary<string, string> oppositedir = new Dictionary<string, string>();

    //pass by ref to Move() for tile updates
    public void Move(Tile pCurrent, Tile pNext, Tile[,] pboard)
    {
        //check pCurrent is where the player is...
        if (pCurrent._isOccupied == true)
        {
            string dir = MoveDirection(pCurrent._boardLocation, pNext._boardLocation);
            //check if move is valid
            if (dir != "invalid move")
            {
                //check if direction movement is valid
                if (pCurrent.ValidMove(dir) && pNext.ValidEntry(dir))
                {
                    //change isOccupied
                    pCurrent._isOccupied = false;
                    pNext._isOccupied = true;
                    //call movement function
                }
            }
            else
            {
                Debug.Log(dir);
            }
        }
    }
    //directional check for placing tiles no tile required just cell locations...and board...should be reversed order for ValidEntry
    public bool Move(string pCurrent , string pNext , Tile[,] pboard)
    {
        Debug.Log("move string params");
        //current row/col
        int currow = System.Int32.Parse(pCurrent.Substring(0, 1));
        int curcol = System.Int32.Parse(pCurrent.Substring(1, 1));
        //next row/col
        int nextrow = System.Int32.Parse(pNext.Substring(0, 1));
        int nextcol = System.Int32.Parse(pNext.Substring(1, 1));
        bool move = false;
        string dir = "";
        dir = MoveDirection(pCurrent, pNext);
        Debug.Log("Dir =" + dir);
        Debug.Log(pboard[nextrow, nextcol].ValidEntry(dir));
        if ( pboard[nextrow, nextcol].ValidEntry(dir))
        { move = true; }

        return move;
    }
    //Placement for checking valid tile placements of tiles
    public bool ValidPlacement(string pCurrent, Tile pNext )
    {
        bool placement = false;
        string dir = "";
        dir = MoveDirection(pCurrent, pNext._boardLocation);
        Debug.Log("direction :" +dir);
        if (dir != "")
        {
           if( pNext.ValidEntry(dir))
            {
                placement = true;
            }
        }
        return placement;
    }
    //public string return directional string based on current index and next index
    public string MoveDirection (string pCurrent, string pNext )
    {
        string dir = "";
        int tempmove = 0;
        //current row/col
        int currow = System.Int32.Parse(pCurrent.Substring(0,1));
        int curcol = System.Int32.Parse(pCurrent.Substring(1, 1));
        //next row/col
        int nextrow = System.Int32.Parse(pNext.Substring(0, 1));
        int nextcol = System.Int32.Parse(pNext.Substring(1, 1));
        //check row are equal
        if ( (currow - nextrow) == 0)
        {
            //movements sideways check col
            tempmove = curcol - nextcol;
            switch (tempmove)
            {
                case -1:
                    {
                        dir = "right";
                        break;
                    }
                case 1:
                    {
                        dir = "left";
                        break;
                    }
                default:
                    {
                        dir = "invalid move";
                        break;
                    }
            }
        }
        else
        {
            tempmove = currow - nextrow;
            switch(tempmove)
            {
                case -1:
                    {
                        dir = "down";
                        break;
                    }
                case 1:
                    {
                        dir = "up";
                        break;
                    }
                default:
                    {
                        dir = "invalid move";
                        break;
                    }
            }
        }
        return dir;
    }
}
