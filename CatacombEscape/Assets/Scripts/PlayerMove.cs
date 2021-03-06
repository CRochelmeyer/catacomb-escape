﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles moving the player character.
/// </summary>
public class PlayerMove : MonoBehaviour
{
	public GameObject player;
	public int playerDisp = 4; // The y amount the player's center is away from the center of a tile
    //multispeed @default 145
    public float multispeed { get; set; }
	public Animator animator;
	private GameObject tempobj;
	private Rigidbody2D rb2d;
	private string pDirection = "";
	private Vector3 targetPosition;
	private Vector3 initialPosition;
    private bool coinUpdated = false;
    CoinController coinCont;
    private string coinString="";
    public float speed = 0.01f;
	private float startTime;
	private float journeyLength;
	private bool moving = false;
	private bool enteringLevel = true;
	private bool exitingLevel = false;
    private bool crRunning = false;
    //dictionary to match cell strings of 00-04 10-14 to an index from 0-29
    Dictionary<string, int> cellindex = new Dictionary<string, int>();
    private Direction moveDir;
    private float panelHeight;

	private GameLogic gameLogic = null;
	private TutorialLogic tutorialLogic = null;

	void Start ()
	{
        //multispeed @default 145
        multispeed = 145;
        coinCont = GameObject.FindGameObjectWithTag("Scripts").GetComponent<CoinController>();
        if (PlayerPrefs.GetString ("TutorialScene") == "true")
		{
			tutorialLogic = GameObject.FindObjectOfType<TutorialLogic> ();

			cellindex.Clear();
			int temp = 0;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					cellindex.Add(i.ToString() + j.ToString(), temp);
					temp++;
				}
			}
		}
		else
		{
			gameLogic = GameObject.FindObjectOfType<GameLogic> ();

			cellindex.Clear();
			int temp = 0;
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					cellindex.Add(i.ToString() + j.ToString(), temp);
					temp++;
				}
			}
		}
		        
        moveDir = GameObject.FindGameObjectWithTag("Scripts").GetComponent<Direction>();
    }

	public void FixedUpdate()
	{
		if (moving)
		{
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			
			if (pDirection != "" && pDirection != "invalid move")
			{
                SetAnimation(pDirection);
				player.transform.localPosition = Vector3.Lerp (initialPosition, targetPosition, fracJourney);

				if (player.transform.localPosition == targetPosition)
				{
					animator.SetInteger ("Direction", 0); // Set animation to idle
					moving = false;
					pDirection = "";

					if (enteringLevel)
					{
						enteringLevel = false;

						if (PlayerPrefs.GetString ("TutorialScene") == "true")
							tutorialLogic.InitPlayer();
						else
							gameLogic.InitPlayer();
					}
					else if (exitingLevel)
					{
						exitingLevel = false;
						enteringLevel = true;

						if (PlayerPrefs.GetString ("TutorialScene") == "true")
							tutorialLogic.SetNextLevel = true;
						else
						{
							gameLogic.SetNextLevel = true;
							//gameLogic.EnableTransition ();
						}
					}
					else
					{
						if (PlayerPrefs.GetString ("TutorialScene") == "true")
							tutorialLogic.SetPlayerLoc ();
						else
                        {
                            gameLogic.SetPlayerLoc();
                            gameLogic.mouseClicked = false;
                        }
					}
				}
			}
		}
	}

	public Vector3 PlayerLocation
	{
		get {return player.transform.position;}
	}
	
    /// <summary>
    /// When the level starts, this sets the player above the grid and sets
	/// the target location as the entrance tile
    /// </summary>
    /// <param name="pLoc"></param>
    /// <param name="pGrid"></param>
	public void DrawPlayer(int pLoc, GameObject[] pGrid)
	{
		GameObject startPanel = pGrid [pLoc];
		panelHeight = startPanel.GetComponent <RectTransform> ().rect.height;
		Vector3 tempPos = startPanel.transform.localPosition;

		if (PlayerPrefs.GetString ("TutorialScene") == "true")
		{
			tempPos = new Vector3 (tempPos.x, tempPos.y - playerDisp, tempPos.z);
			player.transform.localPosition = tempPos;
			enteringLevel = false;
		}else
		{
			tempPos = new Vector3 (tempPos.x, tempPos.y - playerDisp + panelHeight, tempPos.z);
			player.transform.localPosition = tempPos;
			UpdatePlayer (startPanel, "down");
		}
    }

    /// <summary>
	/// When tile is clicked to move the player, this updates the target location and performs the movement animation
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="pdir"></param>
    public void UpdatePlayer (GameObject panel, string pdir)
    {
		pDirection = pdir;
		Vector3 tempPos = panel.transform.localPosition;
		tempPos = new Vector3 (tempPos.x, tempPos.y - playerDisp, tempPos.z);
		targetPosition = tempPos;
		initialPosition = player.transform.localPosition;
		journeyLength = Vector3.Distance(initialPosition, targetPosition);
		startTime = Time.time;
		moving = true;
	}

    /// <summary>
    /// overload for the above method taking in multiple paths
    /// </summary>
    public void UpdatePlayer(GameObject[] panel, List<string> path , Tile[,] pboard)
    {
		
        if (path.Count > 1)
        {
            StopCoroutine(MovePath(panel, path, pboard));
            StartCoroutine(MovePath(panel, path, pboard));
        }
        else
        {
            Debug.Log("Path not long enough. Not starting co-routine.");
			if (PlayerPrefs.GetString ("TutorialScene") == "true")
				tutorialLogic.mouseClicked = false;
			else
				gameLogic.mouseClicked = false;
        }
        
    }

    /// <summary>
    /// First coroutine that handles data is passed thru from gameLogic, which than runs a loop to call a second coroutine
    /// **came to this solution due to functions/loops not waiting fro the end of a coroutine or lerp before beginning, this method could possible be polished/refined for performance.
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    IEnumerator MovePath(GameObject[] panel, List<string> path , Tile[,] pboard)
	{
		if (PlayerPrefs.GetString ("TutorialScene") == "true")
			tutorialLogic.mouseClicked = false;
		else
			gameLogic.mouseClicked = true; // Prevent player from making a move whilst moving
        Debug.Log("MovePath Co-routine");
        for (int i = 0; i < (path.Count - 1); i++)
        {
            int index;
            cellindex.TryGetValue(path[i + 1], out index);
            float distance = Vector3.Distance(player.transform.localPosition, panel[index].transform.localPosition);
            float overTime = distance / multispeed;
            //set animation get pDirection
            string direction = moveDir.MoveDirection(path[i], path[i + 1]);
			Debug.LogWarning ("Moving player from " + path[i] + " to " + path[i+1]);
            SetAnimation(direction);
            string tempCoin = coinString;
            Debug.Log("tempCoin :" + tempCoin +" coinstring : "+coinString);

            if (coinUpdated != true )
            {
                coinString = path[i];
                coinUpdated = true;
                Debug.Log("coin updated");
            }

			Vector3 destPos = panel[index].transform.localPosition;
			destPos = new Vector3 (destPos.x, destPos.y - playerDisp, destPos.z);

			StartCoroutine(UpdatePlayerCoroutine(player.transform.localPosition, destPos, overTime));

            if (coinUpdated)
            {
                Debug.Log("animate coin");
                coinCont.UpdateCoins(-1, coinString);
                coinUpdated = false;
            }

            if (crRunning == true)
            {
                yield return new WaitForSeconds(overTime);
				//update logic of the player position per tile grid move allowing enemies to move. having this code here as this coroutine contains playerloc already
				if (PlayerPrefs.GetString ("TutorialScene") == "true")
					tutorialLogic.SetPlayerLoc (path [i + 1]);
				else
                	gameLogic.SetPlayerLoc (path [i + 1]);
            }

            //add logic to see if player is exiting, break from loop
			if (PlayerPrefs.GetString ("TutorialScene") == "true")
			{
				if (tutorialLogic.exiting)
				{
					break;
				}
				if (tutorialLogic.displayingEvent)
				{
					do
					{
						yield return null;
					} while (tutorialLogic.displayingEvent);
				}
			}
			else
			{
	            if (gameLogic.exiting)
	            {
					break;
				}
	            if (gameLogic.displayingEvent)
	            {
	                do
	                {
	                    yield return null;
	                } while (gameLogic.displayingEvent);
	            }
			}
        }
        yield return null;
		if (PlayerPrefs.GetString ("TutorialScene") == "true")
			tutorialLogic.mouseClicked = false;
		else
        	gameLogic.mouseClicked = false;

    }

    IEnumerator UpdatePlayerCoroutine(Vector3 start, Vector3 target, float overTime)
    {
        Debug.Log("UpdatePlayerCoroutine");

        crRunning = true;
        float startTime = Time.time;
        while (Time.time < (startTime + overTime))
        {
            //Debug.Log(Time.time + " :: " + (startTime + overTime) + "overTime : "+ overTime);
            player.transform.localPosition = Vector3.Lerp(start, target, (Time.time - startTime) / overTime);
            if (Vector3.Distance(player.transform.localPosition, target) < 3)
            {
                //set player to target and stop animation
                //disable animation
                SetAnimation("stop");
                player.transform.localPosition = target;
            }
            yield return null;
        }
        crRunning = false;
    }

    public void SetAnimation(string pDirection)
    {
        if (pDirection != "" && pDirection != "invalid move")
        {
            switch (pDirection)
            {
                case "up":
                    animator.SetInteger("Direction", 3); //3=climb
                    break;

                case "down":
                    animator.SetInteger("Direction", 3); //3=climb
                    break;

                case "left":
                    animator.SetInteger("Direction", 1); //1=left
                    break;

                case "right":
                    animator.SetInteger("Direction", 2); //2=right
                    break;
                case "stop":
                    animator.SetInteger("Direction", 0);
                    break;
            }
        }
    }

	public void PlayerGetsGem ()
	{
		animator.SetInteger ("Direction", 5); // Set animation to gem get
	}

    /// <summary>
    /// When the player clicks on the exit tile, this sets the target location
    /// as the area below the exit tile
    /// </summary>
    public void PlayerExits (GameObject panel)
	{
		pDirection = "down";
		GameObject exitPanel = panel;
		panelHeight = exitPanel.GetComponent <RectTransform> ().rect.height;
		Vector3 tempPos = exitPanel.transform.localPosition;
		tempPos = new Vector3 (tempPos.x, tempPos.y - playerDisp - panelHeight, tempPos.z);
		targetPosition = tempPos;
		initialPosition = player.transform.localPosition;
		//Debug.Log ("Initial Position: " + initialPosition);
		//Debug.Log ("Target Position: " + targetPosition);
		journeyLength = Vector3.Distance (initialPosition, targetPosition);
		startTime = Time.time;
		moving = true;
		exitingLevel = true;
		FixedUpdate ();
	}
}