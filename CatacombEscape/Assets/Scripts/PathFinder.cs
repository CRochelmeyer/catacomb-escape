using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
	private Direction ValidMove;

	private List<Tile> walkableTiles = new List<Tile>();

	private Tile[,] grid;

	private string startTile;

	private string endTile;

	private string currentTile;

	private List<string> closedList = new List<string>();

	private List<string> openList = new List<string>();

	private List<string> Path = new List<string>();

	public List<string> PathFind(Tile[,] pboard, string pplayerloc, string pclickloc)
	{
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
				Debug.Log("reached the end");
				break;
			}
			this.openList.Remove(this.currentTile);
			this.closedList.Add(this.currentTile);
			List<string> adjacentTiles = this.GetAdjacentTiles(this.currentTile);
				using (List<string>.Enumerator enumerator = adjacentTiles.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						if (!this.openList.Contains(current) && !this.closedList.Contains(current))
						{
							this.openList.Add(current);
							Tile tile = this.grid[this.GetRow(current), this.GetCol(current)];
							tile.cost = this.grid[this.GetRow(this.currentTile), this.GetCol(this.currentTile)].cost + 1;
							tile.heuristic = this.ManhattanDistance(current);
							tile.total = tile.cost + tile.heuristic;
						}
					}
				}
		}
		this.GetPath();
		return this.Path;
	}

	public void GetPath()
	{
		bool flag = false;
		this.currentTile = this.endTile;
		while (!flag)
		{
			List<string> adjacentTiles = this.GetAdjacentTiles(this.currentTile);
			foreach(string current in adjacentTiles)
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
		Debug.Log("GetAdjTiles");
		List<string> list = new List<string>();
		string text = (this.GetRow(pcurrent) + 1).ToString();
		Debug.Log("change down " + text);
		if (int.Parse(text) >= 0 && int.Parse(text) <= 5)
		{
			string text2 = (text + pcurrent.Substring(1, 1) );
			Debug.Log(text2 + "below adjTile");
			if (this.GetRow(text2) >= 0 && this.GetRow(text2) <= 5 && this.grid[this.GetRow(text2), this.GetCol(text2)]._isEntrySet && this.ValidMove.ValidMovement("down", this.grid[this.GetRow(pcurrent), this.GetCol(pcurrent)], this.grid[this.GetRow(text2), this.GetCol(text2)]))
			{
				list.Add(text2);
				Debug.Log("Down success");
			}
		}
		text = (this.GetCol(pcurrent) - 1).ToString();
		Debug.Log("change left " + text);
		if (int.Parse(text) >= 0 && int.Parse(text) <=4)
		{
			string text2 = pcurrent.Substring(0, 1) + text;
			Debug.Log(text2 + "left adjTile");
			if (this.GetCol(text2) >= 0 && this.GetCol(text2) <= 4 && this.grid[this.GetRow(text2), this.GetCol(text2)]._isEntrySet && this.ValidMove.ValidMovement("left", this.grid[this.GetRow(pcurrent), this.GetCol(pcurrent)], this.grid[this.GetRow(text2), this.GetCol(text2)]))
			{
				list.Add(text2);
				Debug.Log("left success");
			}
		}
		text = (this.GetCol(pcurrent) + 1).ToString();
		Debug.Log("change right " + text);
		if (int.Parse(text) >= 0 && int.Parse(text) <=4 )
		{
			string text2 = pcurrent.Substring(0, 1) + text;
			Debug.Log(text2 + "right adjTile");
			if (this.GetCol(text2) >= 0 && this.GetCol(text2) <= 4 && this.grid[this.GetRow(text2), this.GetCol(text2)]._isEntrySet && this.ValidMove.ValidMovement("right", this.grid[this.GetRow(pcurrent), this.GetCol(pcurrent)], this.grid[this.GetRow(text2), this.GetCol(text2)]))
			{
				list.Add(text2);
				Debug.Log("right success");
			}
		}
		text = (this.GetRow(pcurrent) - 1).ToString();
		Debug.Log("change up " + text);
		if (int.Parse(text) >= 0 && int.Parse(text) <= 5)
		{
			string text2 = text + pcurrent.Substring(1, 1);
			Debug.Log(text2 + "up adjTile");
		    Debug.Log("within range above");
			if (this.grid[this.GetRow(text2), this.GetCol(text2)]._isEntrySet && this.ValidMove.ValidMovement("up", this.grid[this.GetRow(pcurrent), this.GetCol(pcurrent)], this.grid[this.GetRow(text2), this.GetCol(text2)]))
			{
				list.Add(text2);
				Debug.Log("Up success");
			}
		}
		return list;
	}

	public int GetRow(string pstring)
	{
		int num = int.Parse(pstring.Substring(0, 1));
		Debug.Log(string.Concat(new object[]
		{
			"get Row ",
			pstring,
			" :: ",
			num
		}));
		return num;
	}

	public int GetCol(string pstring)
	{
		int num = int.Parse(pstring.Substring(1, 1));
		Debug.Log(string.Concat(new object[]
		{
			"get Col ",
			pstring,
			" :: ",
			num
		}));
		return num;
	}
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               