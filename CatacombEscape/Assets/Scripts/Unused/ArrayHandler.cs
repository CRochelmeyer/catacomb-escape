using UnityEngine;
using System.Collections;

public class ArrayHandler : MonoBehaviour
{
    public string FindLocation(Vector2 pCoord)
    {
		/*
        //static grid dimensions for now
        int _col = 5;
        int _row = 6;
        //coordinates based of grid
        int _yMin = 138;
        int _yMax = 860;
        int _xMin = 0;
        int _xMax = 600;
        int _padd = 0;
        //int _bpadd = 0;
        int _cellsizex = (_xMax - _xMin) / _col + _padd;
        int _cellsizey = (_yMax - _yMin) / _row +_padd;
        //return var
        string _cell = "";
        //cell 00 xy max - padding
        //Debug.Log("FindLocation :" + pCoord);
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
        }*/

		GameLogic gameLogic = GameObject.FindObjectOfType<GameLogic> ();
		//Debug.Log (gameLogic.MouseLocation);
		string _cell = gameLogic.MouseLocation;

        return _cell;
    }
}
