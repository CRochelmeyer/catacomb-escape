using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Direction : MonoBehaviour
{
    //Dictionary<string, string> oppositedir = new Dictionary<string, string>();

    //current row/col
    int currow = 0;
    int curcol = 0;
    //next row/col
    int nextrow = 0;
    int nextcol = 0;

    //pass by ref to Move() for tile updates
    public bool Move(Tile pCurrent, Tile pNext, ref Tile[,] pboard ,int crow,int ccol, int nrow, int ncol)
    {
        bool valid = false;
        //check pCurrent is where the player is...and the move to location is within range...
        if (pCurrent._isOccupied == true)
        {
            //check is within movement range
            if (InRange(pCurrent._boardLocation, pNext._boardLocation))
            {
                //Debug.Log("inRange");
                string dir = MoveDirection(pCurrent._boardLocation, pNext._boardLocation);
                //check if move is valid
                if (dir != "invalid move")
                {
                    //Debug.Log("Dir in move check :" + dir);
                    //check if direction movement is valid
                    if ( ValidMovement(dir,pCurrent,pNext) )
                    {
                        //Debug.Log("valid move/entry");  
                        //change isOccupied
                        pboard[crow, ccol]._isOccupied = false;
                        //Debug.Log(" new occupied " + pNext._boardLocation);
                        pboard[nrow, ncol]._isOccupied = true;
                        Debug.Log("Valid move updated");
                        valid = true;
                        //call movement function
                    }
                }

            }
        }
        return valid;
    }

    //overloading above method with string string tile[,]
    public bool Move(string pCurrent , string pNext ,ref Tile[,] pboard)
    {
        //Debug.Log("move string params");
        //Debug.Log("pCur " + pCurrent + "pNext " + pNext);
        //current row/col
        currow = System.Int32.Parse(pCurrent.Substring(0, 1));
        curcol = System.Int32.Parse(pCurrent.Substring(1, 1));
        //next row/col
        nextrow = System.Int32.Parse(pNext.Substring(0, 1));
        nextcol = System.Int32.Parse(pNext.Substring(1, 1));
        return this.Move( pboard[currow, curcol] , pboard[nextrow, nextcol] , ref pboard,currow,curcol,nextrow,nextcol);
    }

    //Placement for checking valid tile placements of tiles
    public bool ValidPlacement(string pCurrent, Tile pNext )
    {
        bool placement = false;
        string dir = "";
        dir = MoveDirection(pCurrent, pNext._boardLocation);
        //Debug.Log("direction :" +dir);
        if (dir != "")
        {
           if( pNext.ValidEntry(dir) )
            {
                placement = true;
            }
        }
        return placement;
    }

    //public string return directional string based on current index and next index
    public string MoveDirection (string pCurrent, string pNext )
    {
        string dir = "invalid move";
        int tempmove = 0;
        //current row/col
        //Debug.Log("moveDirection " + pCurrent + ":: " + pNext);
        currow = System.Int32.Parse(pCurrent.Substring(0,1));
        curcol = System.Int32.Parse(pCurrent.Substring(1, 1));
        //next row/col
        if(pNext != null && pNext != "")
        {
            nextrow = System.Int32.Parse(pNext.Substring(0, 1));
            nextcol = System.Int32.Parse(pNext.Substring(1, 1));
        }
        //check row are equal resulting in horizontal movement
        if ( Mathf.Abs(currow - nextrow) == 0)
        {
            dir = "invalid move";
            //movements sideways check col
            tempmove = (curcol - nextcol);
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
        //else check if its vertical movement
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
        //Debug.Log("Movedirections:::directions " + dir);
        return dir;
    }

    public bool InRange(string pCurrent, string pNext)
    {
        bool validmove = false;
        int rowmove = 0;
        int colmove = 0;
        currow = System.Int32.Parse(pCurrent.Substring(0, 1));
        curcol = System.Int32.Parse(pCurrent.Substring(1, 1));
        nextrow = System.Int32.Parse(pNext.Substring(0, 1));
        nextcol = System.Int32.Parse(pNext.Substring(1, 1));
        rowmove = Mathf.Abs(currow - nextrow);
        colmove = Mathf.Abs(curcol - nextcol);

        //if abs of rowmove = 1 then moving vertically...
        if (rowmove == 1)
        {
            //check that it isnt also moving horizontally.
            if (colmove == 0)
            {
                validmove = true;
            }
        }
        //moving horizontally
        else if (rowmove == 0)
        {
            //and that it is moving horizontall one unit
            if (colmove == 1)
            {
                validmove = true;
            }
        }
        return validmove;
    }

    public bool ValidMovement(string pdir,Tile pCurrent, Tile pNext)
    {
        //Debug.Log("pCurrent VM ::start");
        //pCurrent.test();
        //Debug.Log("pCurrent VM ::end");
        //Debug.Log("pNext VM ::start");
        //pNext.test();
        //Debug.Log("pNext VM ::end");
        //Debug.Log("ValidMovement :" + pCurrent.ValidMove(pdir) +" ::: "+pNext.ValidEntry(pdir));

        bool valid = false;
        if ( (pCurrent.ValidMove(pdir)) && (pNext.ValidEntry(pdir)) )
        {
            valid = true;
        }
        return valid;
    }
}
