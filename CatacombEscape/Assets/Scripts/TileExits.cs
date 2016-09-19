using UnityEngine;
using System.Collections;

public class TileExits : MonoBehaviour
{
	public bool exitUp;
	public bool exitRight;
	public bool exitDown;
	public bool exitLeft;

	/// <summary>
	/// Returns positive if this tile has an upwards exit.
	/// </summary>
	/// <value><c>true</c> if this instance can exit up; otherwise, <c>false</c>.</value>
	public bool CanExitUp
	{
		get {return exitUp;}
	}

	/// <summary>
	/// Returns positive if this tile has a right exit.
	/// </summary>
	/// <value><c>true</c> if this instance can exit right; otherwise, <c>false</c>.</value>
	public bool CanExitRight
	{
		get {return exitRight;}
	}

	/// <summary>
	/// Returns positive if this tile has a downwards exit.
	/// </summary>
	/// <value><c>true</c> if this instance can exit down; otherwise, <c>false</c>.</value>
	public bool CanExitDown
	{
		get {return exitDown;}
	}

	/// <summary>
	/// Returns positive if this tile has a left exit.
	/// </summary>
	/// <value><c>true</c> if this instance can exit left; otherwise, <c>false</c>.</value>
	public bool CanExitLeft
	{
		get {return exitLeft;}
	}
}
