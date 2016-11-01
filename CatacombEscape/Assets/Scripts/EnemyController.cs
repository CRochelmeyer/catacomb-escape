using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
	public GameLogic gameLogic;
	public TutorialLogic tutLogic;
	public GemController gemCont;
	public GameObject directionArrow;
	public Sprite enemyFront;
	public Sprite enemyLeft;
	public Sprite enemyRight;
	public GameObject enemySkeleton;

	/// <summary>
	/// Updates events. Deals damage etc.
	/// </summary>
	/// <param name="pcell"></param>
	public void PlayEvent(string pcell)
	{
		if (gameLogic.GetTile (System.Int32.Parse(pcell.Substring(0, 1)), System.Int32.Parse(pcell.Substring(1, 1)))._isActive)
		{

            Debug.LogWarning("["+Time.time+"] Calling PlayEvent");

            int temprow = System.Int32.Parse(pcell.Substring(0, 1));
			int tempcol = System.Int32.Parse(pcell.Substring(1, 1));

			Tile tempTile = gameLogic.GetTile (temprow, tempcol);
			GameObject tempObj = GameObject.Find(tempTile._boardLocation + "(Clone)");

			//play event = remove stamina and destroy the event clone...
			if (tempTile._event == "red")
			{
				gameLogic.DisplayEvent (tempTile, tempObj);
			}
			else if (tempTile._event == "green")
			{
				gameLogic.DisplayEvent (tempTile, tempObj);
			}

			if (tempObj != null)
			{
				//Debug.Log("destroy clone");
				Destroy(tempObj);
				tempTile._isActive = false;
			}

			gameLogic.CheckStamina();
			tempTile._event = ""; // Event wasn't being destroyed after it was triggered. This should solve that.
		}
	}

	/// <summary>
	/// Used in determining valid enemy movement directions.
	/// Checks a given direction, if the move is valid returns true.
	/// </summary>
	/// <returns>True if the move is valid.</returns>
	/// <param name="dir">Dir.</param>
	public bool CheckEnemyMove(string dir, int curRow, int curCol)
	{

		bool isValidMove = false;

		Tile currentTile = gameLogic.GetTile (curRow, curCol);
		Tile newTile; // the target tile.

		string curLocStr = curRow.ToString() + curCol.ToString();
		string newLocStr;

		switch (dir)
		{
		case "up":

			if (curRow - 1 >= 0)
			{
				if (gameLogic.GetTile (curRow - 1, curCol) == null)
				{
					Debug.Log("The tile [" + (curRow - 1).ToString() + curCol.ToString() + "] is null.");
				}
				newTile = gameLogic.GetTile (curRow - 1, curCol);
				newLocStr = (curRow - 1).ToString() + curCol.ToString();
			}
			else
			{
				return false;
			}
			break;

		case "down":

			if (curRow + 1 < 6)
			{
				newTile = gameLogic.GetTile (curRow + 1, curCol);
				newLocStr = (curRow + 1).ToString() + curCol.ToString();
			}
			else
			{
				return false;
			}
			break;

		case "left":

			if (curCol - 1 >= 0)
			{
				newTile = gameLogic.GetTile (curRow, curCol - 1);
				newLocStr = (curRow).ToString() + (curCol - 1).ToString();
			}
			else
			{
				return false;
			}
			break;

		case "right":
			if (curCol + 1 < 5)
			{
				newTile = gameLogic.GetTile (curRow, curCol + 1);
				newLocStr = (curRow).ToString() + (curCol + 1).ToString();
			}
			else
			{
				return false;
			}
			break;

		default:
			return false;
		}

		// Just some debugging code.
		// Used for printing tile info to the debug log.
		string debug = "# ";

		foreach (Tile tile in gameLogic.GetTileBoard())
		{
			if(tile._isEntrySet)
			{
				debug += tile._boardLocation + " ";

				if (tile._event != "")
				{
					debug += "[" + tile._event + "] ";
				}
			}
		}

		debug = "# ";

		foreach (Tile tile in gameLogic.GetTileBoard())
		{
			if (tile._event == " red" || tile._event == " green")
			{
				debug += tile._event;
			}
		}

		string debugStr = "[Trying move [" + currentTile._boardLocation + " -> " + newTile._boardLocation + "] (" + dir + ")"; // Rather than having a million debug messages, add messages to this string, and use one debug.log

		// Moving from a tile placed by the player.
		if (currentTile._isEntrySet)
		{
			debugStr += " [Enemy at [" + curLocStr + "] is checking move from a non-empty tile] ";

			// If the target is a tile as well. Prevent moving to entrance, exit, chest tiles and other enemies. 
			if (newTile._isEntrySet && (newTile._tileID != "tile_exit" && newTile._tileID != "tile_entrance") && (newTile._event != "green" && newTile._event != "red") && (newLocStr != gameLogic.GetPlayerLoc()))
			{

				string debugStr2 = "";
				foreach (string entry in newTile._entry)
				{
					debugStr2 += entry + " ";
				}
				//Debug.Log("Target tile ("+ newLocStr +") entries: ");

				if (gameLogic.CheckValidMovement (dir, currentTile, newTile))
				{
					debugStr += " [Target tile ("+ newLocStr +") was placed by player, and is a valid move] ";
					isValidMove = true;
				}
				else
				{
					debugStr += " [Target tile (" + newLocStr + ") was placed by player, but is not a valid move] ";
					isValidMove = false;
				}

			}
			else
			{
				// Target tile is empty
				if (!newTile._isEntrySet && currentTile.ValidMove(dir) && (newTile._event != "green" && newTile._event != "red"))
				{
					debugStr += " [Target tile (" + newLocStr + ") is empty, and current tile has relevant exit] ";
					isValidMove = true;
				}
				else
				{
					debugStr += " [Target tile (" + newLocStr + ") is empty, but current tile does not have relevent exit] ";

					debugStr += "Fucking why? " + "_isEntrySet? " + newTile._isEntrySet + ", Valid move? " + currentTile.ValidMove("dir") + ", An event? " + newTile._event;

					debugStr += " [Tile had ";
					foreach (string entry in currentTile._entry)
					{
						debugStr += entry + " ";
					}
					debugStr += "]";
				}
			}

		}
		// Moving from an empty tile
		else
		{
			debugStr += " [Enemy at [" + curLocStr + "] is checking move from an empty tile] ";
			// Target is not empty
			if (newTile._isEntrySet && (newTile._tileID != "tile_exit" && newTile._tileID != "tile_entrance") && (newTile._event != "green" && newTile._event != "red") && (newLocStr != gameLogic.GetPlayerLoc()))
			{
				if (newTile.ValidEntry(dir))
				{
					debugStr += " [Target tile (" + newLocStr + ") is player-placed, and current tile is empty] ";
					isValidMove = true;
				}
				else
				{
					debugStr += " [The target tile does not have the correct entry]";
				}
			}
			else if ((newTile._event == "green") || (newTile._event == "red"))
			{
				// If there is an event, then the move is invalid.
				debugStr += " [Target tile contains a "+ newTile._event + " event, and is thus an invalid move.]";
			}
			else if (newTile._tileID == null)
			{
				debugStr += " [Target tile (" + newLocStr + ") is empty, and current tile is also empty] ";
				isValidMove = true;
			}
		}
		return isValidMove;
	}

	/// <summary>
	/// Handles movement of the red event tiles
	/// </summary>
	public void MoveEvents()
	{
        Debug.LogWarning("[" + Time.time + "] Calling MoveEvents");

        string etloc = "";
		int currow = 0;
		int curcol = 0;

		GameObject[] eventTiles = GameObject.FindGameObjectsWithTag("eventTile");
		for (int i = 0; i < eventTiles.Length; i++)
		{
			// For red tiles only. Green tiles don't move.
			if (eventTiles[i].GetComponent<Image>().sprite.name.Substring(0, 5) == "enemy")
			{
				etloc = eventTiles[i].name.Substring(0, 2);
				System.Int32.TryParse(etloc.Substring(0, 1), out currow);
				System.Int32.TryParse(etloc.Substring(1, 1), out curcol);
				Tile currentTile = gameLogic.GetTile (currow, curcol);

				var moves = new List<string>();

				// Check for available moves
				if (CheckEnemyMove ("up", currow, curcol)) {
					moves.Add ("up");
					//Debug.Log ("["+ currow +"," + curcol +"] can move up.");
				}

				if (CheckEnemyMove ("down", currow, curcol)) {
					moves.Add ("down");
					//Debug.Log ("["+ currow +"," + curcol +"] can move down.");
				}

				if (CheckEnemyMove ("left", currow, curcol)) {
					moves.Add ("left");
					//Debug.Log ("["+ currow +"," + curcol +"] can move left.");
				}

				if (CheckEnemyMove ("right", currow, curcol)) {
					moves.Add ("right");
					//Debug.Log ("["+ currow +"," + curcol +"] can move right.");
				}
					
				// Moves an enemy, if it is able
				if (moves.Count != 0)
				{
					//Debug.Log("Enemy at [" + etloc + "] moving " + currentTile._nextMove);
					MoveEnemy (currentTile._nextMove, currow, curcol, i);
				}
				else
				{
					// The enemy can't move.
					// This else is here for giving the player a bonus if they've trapped an enemy.

					Debug.Log("Enemy at [" + currow.ToString() + curcol.ToString() + "] can no longer move.");
					KillStuckEnemy (eventTiles[i], currentTile);
				}
			}
		}
	}

	/// <summary>
	/// Moves an enemy tile, based upon a given string direction. (up, down, left, right)
	/// </summary>
	public void MoveEnemy(string dir, int curRow, int curCol, int enemyIndex)
	{
		string newLoc = "";
		int newRow = 0, newCol = 0;

		switch (dir)
		{
		case "up":
			newLoc = (curRow - 1).ToString() + curCol.ToString();
			newRow = curRow - 1;
			newCol = curCol;
			break;

		case "down":
			newLoc = (curRow + 1).ToString() + curCol.ToString();
			newRow = curRow + 1;
			newCol = curCol;
			break;

		case "left":
			newLoc = curRow.ToString() + (curCol - 1).ToString();
			newRow = curRow;
			newCol = curCol - 1;
			break;

		case "right":
			newLoc = curRow.ToString() + (curCol + 1).ToString();
			newRow = curRow;
			newCol = curCol + 1;
			break;
		}

		// Used to access gridpanels
		int panelkey = 0;

		Tile dummy = new Tile(0);
		GameObject[] eventTiles = GameObject.FindGameObjectsWithTag("eventTile");

		panelkey = gameLogic.TryGetCellIdxValue (newLoc);

		//update tileboard
		eventTiles[enemyIndex].transform.localPosition = gameLogic.GetGridPanelPositionByIdx (panelkey);

		//clone the current tile to dummy
		dummy.CloneTile (gameLogic.GetTile (curRow, curCol));

		//set current tile event that's moving to no event
		gameLogic.GetTile (curRow, curCol).ClearEvent();

		//flush dummy entry if it is set from the previous tile cloned
		if (dummy._isEntrySet)
		{
			dummy.FlushEntry();
		}

		//update the new board position with the dummy clone
		gameLogic.GetTile (newRow, newCol).UpdatePosition (dummy);

		//update objclone name to be used for destroying the game obj
		eventTiles[enemyIndex].name = newLoc + "(Clone)";

		// If enemy moves onto player, play event
		if (gameLogic.GetPlayerLoc() == newLoc)
		{
			PlayEvent(newLoc);
		}

		RecheckPlannedMoves();
	}

	/// <summary>
	/// Plans enemy moves so they can be displayed.
	/// </summary>
	public void PlanEnemyMoves()
	{
		GameObject[] eventTiles = GameObject.FindGameObjectsWithTag("eventTile");
		for (int i = 0; i < eventTiles.Length; i++) 
		{
			SetPlannedMovement (eventTiles [i]);
		}
	}

	public void RecheckPlannedMoves()
	{
		GameObject[] eventTiles = GameObject.FindGameObjectsWithTag("eventTile");
		for (int i = 0; i < eventTiles.Length; i++) 
		{
			string etloc = "";
			int currow = 0;
			int curcol = 0;

			etloc = eventTiles [i].name.Substring(0, 2);
			System.Int32.TryParse(etloc.Substring(0, 1), out currow);
			System.Int32.TryParse(etloc.Substring(1, 1), out curcol);

			string dir = gameLogic.GetTile (currow, curcol)._nextMove;
			if (!CheckEnemyMove (dir, currow, curcol))
				SetPlannedMovement (eventTiles [i]);
		}
	}

	public void SetPlannedMovement (GameObject eventTile)
	{
		string etloc = "";
		int currow = 0;
		int curcol = 0;

		// For red tiles only. Green tiles don't move.
		if (eventTile.GetComponent<Image> ().sprite.name.Substring(0, 5) == "enemy") 
		{
			etloc = eventTile.name.Substring(0, 2);
			System.Int32.TryParse(etloc.Substring(0, 1), out currow);
			System.Int32.TryParse(etloc.Substring(1, 1), out curcol);

			Tile enemyTile = gameLogic.GetTile (currow, curcol);

			List<string> moves = new List<string>();

			// Check for available moves
			if (CheckEnemyMove ("up", currow, curcol))
			{
				moves.Add ("up");
			}

			if (CheckEnemyMove ("down", currow, curcol))
			{
				moves.Add ("down");
			}

			if (CheckEnemyMove ("left", currow, curcol))
			{
				moves.Add ("left");
			}

			if (CheckEnemyMove ("right", currow, curcol))
			{
				moves.Add ("right");
			}

			// Select a random valid direction and set it to the enemy's next move.
			if (moves.Count != 0)
			{
				string moveName = moves [Random.Range(0, moves.Count)];
				GameObject arrow;

				if (eventTile.transform.childCount <= 0)
				{
					arrow = Instantiate (directionArrow);
					arrow.transform.SetParent (eventTile.transform);
					arrow.transform.localPosition = eventTile.transform.position;
					arrow.transform.localScale = new Vector3 (1, 1, 1);
				}
				else
				{
					arrow = eventTile.transform.GetChild (0).gameObject;
				}

				switch (moveName)
				{
				case "up":
					arrow.transform.rotation = Quaternion.identity;
					arrow.transform.Rotate (0, 0, 180);
					eventTile.GetComponent<Image> ().sprite = enemyFront;
					break;
				case "down":
					arrow.transform.rotation = Quaternion.identity;
					eventTile.GetComponent<Image> ().sprite = enemyFront;
					break;
				case "left":
					arrow.transform.rotation = Quaternion.identity;
					arrow.transform.Rotate (0, 0, 270);
					eventTile.GetComponent<Image> ().sprite = enemyLeft;
					break;
				case "right":
					arrow.transform.rotation = Quaternion.identity;
					arrow.transform.Rotate (0, 0, 90);
					eventTile.GetComponent<Image> ().sprite = enemyRight;
					break;
				}

				enemyTile._nextMove = moveName;
				//Debug.Log ("Enemy at [" + etloc + "] is planning to move " + moveName);
			}
		}
	}

	public void TutorialSetMovement (GameObject eventTile, string direction)
	{
		string etloc = "";
		int currow = 0;
		int curcol = 0;

		// For red tiles only. Green tiles don't move.
		if (eventTile.GetComponent<Image> ().sprite.name.Substring(0, 5) == "enemy") 
		{
			etloc = eventTile.name.Substring(0, 2);
			System.Int32.TryParse(etloc.Substring(0, 1), out currow);
			System.Int32.TryParse(etloc.Substring(1, 1), out curcol);

			Tile enemyTile = tutLogic.GetTile (currow, curcol);

			string moveName = direction;
			GameObject arrow;

			if (eventTile.transform.childCount <= 0)
			{
				arrow = Instantiate (directionArrow);
				arrow.transform.SetParent (eventTile.transform);
				arrow.transform.localPosition = eventTile.transform.position;
				arrow.transform.localScale = new Vector3 (1, 1, 1);
			}
			else
			{
				arrow = eventTile.transform.GetChild (0).gameObject;
			}

			switch (moveName)
			{
			case "up":
				arrow.transform.rotation = Quaternion.identity;
				arrow.transform.Rotate (0, 0, 180);
				eventTile.GetComponent<Image> ().sprite = enemyFront;
				break;
			case "down":
				arrow.transform.rotation = Quaternion.identity;
				eventTile.GetComponent<Image> ().sprite = enemyFront;
				break;
			case "left":
				arrow.transform.rotation = Quaternion.identity;
				arrow.transform.Rotate (0, 0, 270);
				eventTile.GetComponent<Image> ().sprite = enemyLeft;
				break;
			case "right":
				arrow.transform.rotation = Quaternion.identity;
				arrow.transform.Rotate (0, 0, 90);
				eventTile.GetComponent<Image> ().sprite = enemyRight;
				break;
			}

			enemyTile._nextMove = moveName;
		}
	}

	/// <summary>
	/// Checks all enemies to see if they can move.
	/// </summary>
	public void AreEnemiesStuck()
	{
		string etloc = "";
		int currow = 0;
		int curcol = 0;

		GameObject[] eventTiles = GameObject.FindGameObjectsWithTag("eventTile");

		for (int i = 0; i < eventTiles.Length; i++) 
		{

			// For red tiles only. Green tiles don't move.
			if (eventTiles [i].GetComponent<Image> ().sprite.name.Substring(0, 5) == "enemy") 
			{
				etloc = eventTiles[i].name.Substring(0, 2);

				System.Int32.TryParse (etloc.Substring (0, 1), out currow);
				System.Int32.TryParse (etloc.Substring (1, 1), out curcol);

				Tile enemyTile = gameLogic.GetTile (currow, curcol);

				GameObject enemyGameObj = GameObject.Find(enemyTile._boardLocation + "(Clone)");

				bool enemyCanMove = false;

				if (CheckEnemyMove ("up", currow, curcol))
				{
					enemyCanMove = true;
				}
				else if (CheckEnemyMove ("down", currow, curcol))
				{
					enemyCanMove = true;
				}
				else if (CheckEnemyMove ("left", currow, curcol))
				{
					enemyCanMove = true;
				}
				else if (CheckEnemyMove ("right", currow, curcol))
				{
					enemyCanMove = true;
				}

				// Add gem animation etc here
				if (!enemyCanMove) 
				{
					Debug.Log ("Enemy at [" + etloc + "] cannot move. Destroy it.");
					enemyTile.ClearEvent(); // Kill the enemy, clear tile fields

					if (enemyGameObj != null)
					{
						StartCoroutine (KillStuckEnemy (enemyGameObj, enemyTile));
					}
				}
			}
		}
	}

	IEnumerator KillStuckEnemy (GameObject enemyObj, Tile enemyTile)
	{
		//Debug.LogWarning ("Killing enemy: " + enemyObj.name + " | " + enemyTile.ToString());
		GameObject skeleton = Instantiate (enemySkeleton) as GameObject;
		skeleton.transform.SetParent (enemyObj.transform.parent, false);
		skeleton.transform.position = enemyObj.transform.position;
		Destroy(enemyObj);
		gemCont.AddGem (enemyObj.transform.position);
		yield return new WaitForSeconds (1.5f);
		//Debug.Log("destroy clone");
		Destroy(skeleton);
		enemyTile.ClearEvent(); // Clear all of the event related fields in the tile.
		gameLogic.IncrementGems ();
	}
}
