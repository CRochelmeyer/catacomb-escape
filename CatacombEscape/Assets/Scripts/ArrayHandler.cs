using UnityEngine;
using System.Collections;

public class ArrayHandler : MonoBehaviour
{
    //Maths for determining the quadrants of the grid
    //Grid x,y coordinates
    /*
    top left x : 0                  top right x : 568
    top left y : 816                top right y : 816

    bot left x : 0                  bot right x : 568
    bot left y : 138                bot right y : 138

    border padding = 5x 5y
    grid padding = 10x 10y

	y = 678
	x = 568

	

    */
    public string FindLocation(Vector2 pCoord)
    {
        //static grid dimensions for now
        int _col = 5;
        int _row = 6;
        //coordinates based of grid
        int _yMin = 138;
        int _yMax = 860;
        int _xMin = 0;
        int _xMax = 600;
        int _padd = 0;
        int _bpadd = 0;
        int _cellsizex = (_xMax - _xMin) / _col + _padd;
        int _cellsizey = (_yMax - _yMin) / _row +_padd;
        //return var
        string _cell = "";
        //cell 00 xy max - padding
       // Debug.Log("FindLocation :" + pCoord);
        //Debug.Log("cellsizex :" + _cellsizex);
        //Debug.Log("cellsizesy :" + _cellsizey);
        for (int row = 0; row < _row; row++)
        {
            for (int col = 0; col < _col; col++)
            {
                //Debug.Log("before ifcell = " + _cell);
                if ( (_cell == "") && (pCoord.x < (_cellsizex * (col + 1))) && (pCoord.y > (_yMax - (_cellsizey * (row + 1))) ) && (pCoord.y <= _yMax) && (pCoord.y >= _yMin) && (pCoord.x >= _xMin) && (pCoord.x <= _xMax) )
                {
                    //Debug.Log("ArrayHandler If :" + row + " " + col);
                    _cell = row.ToString() + col.ToString();
                    Debug.Log("cell = " + _cell);
                    //using break to end the loop prematurely once _cell has been located
                    break;
                }
            }
            //using break to end the loop prematurely once _cell has been located
            if (_cell != "")
            {
                break;
            }
        }
        Debug.Log("testing ArrayH cell size x :" + _cellsizex + " y: " + _cellsizey + "going into cell :"+_cell);
        return _cell;
    }
}
