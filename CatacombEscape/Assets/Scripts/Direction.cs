using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Direction : MonoBehaviour {
    Dictionary<string, string> movedir = new Dictionary<string, string>();
    //pass by ref to Move() for tile updates
    public void Move(Tile pCurrent, Tile pNext , Tile[,] board)
    {
        //check pCurrent is where the player is...
        if (pCurrent._isOccupied == true)
        {
            string dir = MoveDirection(pCurrent._boardLocation, pNext._boardLocation);
            //check if move is valid
            if (dir != "invalid move")
            {
                //change isOccupied
                pCurrent._isOccupied = false;
                pNext._isOccupied = true;
                //call movement function
            }
            else
            {
                Debug.Log(dir);
            }
        }

            
    }
    //public string return directional string based on current index and next index
    public string MoveDirection (string pCurrent, string pNext)
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
