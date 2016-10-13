using System;
using System.Collections.Generic;
using UnityEngine;

// TODO
// Prevent path from containing an exit tile unless it's the last tile.
// Fix endless loops.

public class PathFinder : MonoBehaviour
{
    private Direction ValidMove;
    private Tile[,] grid;
    private string startTile;
    private string endTile;
    private string currentTile;
    private List<string> closedList = new List<string>();
    private List<string> openList = new List<string>();
    private List<string> Path = new List<string>();

	/// <summary>
	/// Finds a path, returns a list of tile locations.
	/// </summary>
	/// <returns>The find.</returns>
	/// <param name="pboard">Pboard.</param>
	/// <param name="pplayerloc">Pplayerloc.</param>
	/// <param name="pclickloc">Pclickloc.</param>
    public List<string> PathFind(Tile[,] pboard, string pplayerloc, string pclickloc)
    {
        bool adjFound = false;
        this.closedList.Clear();
        this.openList.Clear();
        this.Path.Clear();
        this.ValidMove = GameObject.FindGameObjectWithTag("Scripts").GetComponent<Direction>();
        this.startTile = pplayerloc;
        this.endTile = pclickloc;
        this.grid = pboard;
        this.openList.Add(this.startTile);

        while (this.openList.Count != 0)
        {
            this.currentTile = this.GetTileWithLowestTotal(this.openList);

            if (this.currentTile == this.endTile)
            {
                break;
            }

            this.openList.Remove(this.currentTile);
            this.closedList.Add(this.currentTile);
            List<string> adjacentTiles = this.GetAdjacentTiles(this.currentTile);
            
			//check adjacentTiles exist
            if (!adjacentTiles.Contains("Invalid") && !adjacentTiles.Contains("invalid"))
            {
                foreach (string current in adjacentTiles)
                {
                    if (!this.openList.Contains(current) && !this.closedList.Contains(current))
                    {
                        this.openList.Add(current);
                        Tile tile = this.grid[this.GetRow(current), this.GetCol(current)];
                        tile.cost = this.grid[this.GetRow(this.currentTile), this.GetCol(this.currentTile)].cost + 1;
                        tile.heuristic = this.ManhattanDistance(current);
                        tile.total = tile.cost + tile.heuristic;
                        adjFound = true;
                    }
                }
            }
        }

        if (adjFound)
        {
            this.GetPath();
        }
        else if (adjFound == false)
        {
            Path.Add("invalid");
        }
        return this.Path;
    }

    /// <summary>
    /// Adds tiles to the member vairable "Path"
    /// </summary>
    public void GetPath()
    {
        bool flag = false;
        Path.Add(endTile);
        this.currentTile = this.endTile;

        int count = 0; // Used in preventing an endless loop.

        while (!flag)
        {

            count++;
            // 120 should be more than enough loops to determine any viable path.
            if (count > 120)
            {
                Debug.Log("Valid path not found (Getpath loop timed out.)");
                
                // Delete the path, since there were so many loops, and the path is therefore invalid.
                Path.Clear();

                break;
            }
            List<string> adjacentTiles = this.GetAdjacentTiles(this.currentTile);
            if (!adjacentTiles.Contains("Invalid") || !adjacentTiles.Contains("invalid"))
            {
                foreach (string current in adjacentTiles)
                {
                    if (current == this.startTile)
                    {
                        flag = true;
                    }

                    if ((this.closedList.Contains(current) || this.openList.Contains(current)) && this.grid[this.GetRow(current), this.GetCol(current)].cost <= this.grid[this.GetRow(this.currentTile), this.GetCol(this.currentTile)].cost
            && this.grid[this.GetRow(current), this.GetCol(current)].cost > 0)
                    {
                        this.currentTile = current;
                        this.Path.Add(current);
                        break;
                    }
                }
            }
        }
        this.Path.Add(startTile);
        this.Path.Reverse();
    }

    public int ManhattanDistance(string adjTile)
    {
        return Math.Abs(this.GetRow(this.endTile) - this.GetRow(adjTile)) + Math.Abs(this.GetCol(this.endTile) - this.GetCol(adjTile));
    }

    public string GetTileWithLowestTotal(List<string> openList)
    {
        int num = 2147483647;
        string result = string.Empty;

        using (List<string>.Enumerator enumerator = openList.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                string current = enumerator.Current;

                if (this.grid[this.GetRow(current), this.GetCol(current)]._isEntrySet && this.grid[this.GetRow(current), this.GetCol(current)].total <= num)
                {
                    num = this.grid[this.GetRow(current), this.GetCol(current)].total;
                    result = current;
                }
            }
        }
        return result;
    }

    public List<string> GetAdjacentTiles(string pcurrent)
    {
        bool adjFound = false;
        List<string> list = new List<string>();
        string text = (this.GetRow(pcurrent) + 1).ToString();
       // Debug.Log("change down " + text);

        if (int.Parse(text) >= 0 && int.Parse(text) <= 5)
        {
            string text2 = (text + pcurrent.Substring(1, 1));
            if (this.GetRow(text2) >= 0 && this.GetRow(text2) <= 5 && this.grid[this.GetRow(text2), this.GetCol(text2)]._isEntrySet && this.ValidMove.ValidMovement("down", this.grid[this.GetRow(pcurrent), this.GetCol(pcurrent)], this.grid[this.GetRow(text2), this.GetCol(text2)]))
            {
                list.Add(text2);
                adjFound = true;
            }
        }
        text = (this.GetCol(pcurrent) - 1).ToString();
        //Debug.Log("change left " + text);

        if (int.Parse(text) >= 0 && int.Parse(text) <= 4)
        {
            string text2 = pcurrent.Substring(0, 1) + text;
            if (this.GetCol(text2) >= 0 && this.GetCol(text2) <= 4 && this.grid[this.GetRow(text2), this.GetCol(text2)]._isEntrySet && this.ValidMove.ValidMovement("left", this.grid[this.GetRow(pcurrent), this.GetCol(pcurrent)], this.grid[this.GetRow(text2), this.GetCol(text2)]))
            {
                list.Add(text2);
                adjFound = true;
            }
        }
        text = (this.GetCol(pcurrent) + 1).ToString();
        //Debug.Log("change right " + text);

        if (int.Parse(text) >= 0 && int.Parse(text) <= 4)
        {
            string text2 = pcurrent.Substring(0, 1) + text;
            if (this.GetCol(text2) >= 0 && this.GetCol(text2) <= 4 && this.grid[this.GetRow(text2), this.GetCol(text2)]._isEntrySet && this.ValidMove.ValidMovement("right", this.grid[this.GetRow(pcurrent), this.GetCol(pcurrent)], this.grid[this.GetRow(text2), this.GetCol(text2)]))
            {
                list.Add(text2);
                adjFound = true;
            }
        }
        text = (this.GetRow(pcurrent) - 1).ToString();
        //Debug.Log("change up " + text);

        if (int.Parse(text) >= 0 && int.Parse(text) <= 5)
        {
            string text2 = text + pcurrent.Substring(1, 1);

            if (this.grid[this.GetRow(text2), this.GetCol(text2)]._isEntrySet && this.ValidMove.ValidMovement("up", this.grid[this.GetRow(pcurrent), this.GetCol(pcurrent)], this.grid[this.GetRow(text2), this.GetCol(text2)]))
            {
                list.Add(text2);
                adjFound = true;
            }
        }

        if (adjFound)
        {
            return list;
        }
        else
        {
            list.Add("Invalid");
            return list;
        }
    }

    public int GetRow(string pstring)
    {
        int num = int.Parse(pstring.Substring(0, 1));
        return num;
    }

    public int GetCol(string pstring)
    {
        int num = int.Parse(pstring.Substring(1, 1));
        return num;
    }
}