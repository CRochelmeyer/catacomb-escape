using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles moving the player character.
/// </summary>
public class PlayerMove : MonoBehaviour
{
	public GameObject player;
	public int playerDisp = 4; // The y amount the player's center is away from the center of a tile

	public Animator animator;
	private GameObject tempobj;
	private Rigidbody2D rb2d;
	private string pDirection = "";
	Vector3 targetPosition;
	Vector3 initialPosition;

	public float speed = 0.01f;
	private float startTime;
	private float journeyLength;
	private bool moving = false;
	private bool enteringLevel = true;
	private bool exitingLevel = false;
    private bool crRunning = false;
    private bool crUpdatePlayer = false;
    //dictionary to match cell strings of 00-04 10-14 to an index from 0-29
    Dictionary<string, int> cellindex = new Dictionary<string, int>();
    private Direction moveDir;
    private float panelHeight;

	private GameLogic gameLogic = null;
	private TutorialLogic tutorialLogic = null;

	void Start ()
	{
		if (PlayerPrefs.GetString ("TutorialScene") == "true")
			tutorialLogic = GameObject.FindObjectOfType<TutorialLogic> ();
		else
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
				switch (pDirection)
				{
					case "up":
						animator.SetInteger ("Direction", 3); //3=climb
						break;

					case "down":
						animator.SetInteger ("Direction", 3); //3=climb
						break;

					case "left":
						animator.SetInteger ("Direction", 1); //1=left
						break;

					case "right":
						animator.SetInteger ("Direction", 2); //2=right
						break;
				}

				player.transform.localPosition = Vector3.Lerp (initialPosition, targetPosition, fracJourney);

				if (player.transform.localPosition == targetPosition)
				{
					animator.SetInteger ("Direction", 0); // Set animation to idle
					moving = false;
					pDirection = "";

					if (enteringLevel)
						enteringLevel = false;
					else if (exitingLevel)
					{
						exitingLevel = false;
						enteringLevel = true;

						if (PlayerPrefs.GetString ("TutorialScene") == "true")
							tutorialLogic.SetNextLevel = true;
						else
							gameLogic.SetNextLevel = true;
					}else
					{
						if (PlayerPrefs.GetString ("TutorialScene") == "true")
							tutorialLogic.SetPlayerLoc ();
						else
                        {
                            gameLogic.SetPlayerLoc();
                            //set UserInput to false
                            gameLogic.UserInput = false;
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
    public void UpdatePlayer(GameObject[] panel, List<string> path)
    {
        StartCoroutine(MovePath(panel, path));
    }
    /// <summary>
    /// First coroutine that handles data is passed thru from gameLogic, which than runs a loop to call a second coroutine
    /// **came to this solution due to functions/loops not waiting fro the end of a coroutine or lerp before beginning, this method could possible be polished/refined for performance.
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    IEnumerator MovePath(GameObject[] panel, List<string> path)
    {
        for (int i = 0; i < (path.Count - 1); i++)
        {
            int index;
            cellindex.TryGetValue(path[i + 1], out index);
            float distance = Vector3.Distance(player.transform.localPosition, panel[index].transform.localPosition);
            float overTime = distance / 150;
            //set animation get pDirection
            string direction = moveDir.MoveDirection(path[i], path[i + 1]);
            SetAnimation(direction);
            string loc = path[i + 1];
            StartCoroutine(UpdatePlayerCoroutine(player.transform.localPosition, panel[index].transform.localPosition, overTime));
            if (crRunning == true)
            {
                gameLogic.SetPlayerLoc(path[i + 1]);
                yield return new WaitForSeconds(overTime);
                //update logic of the player position per tile grid move allowing enemies to move. having this code here as this coroutine contains playerloc already
            }
        }
        yield return null;
        gameLogic.UserInput = false;
    }
    IEnumerator UpdatePlayerCoroutine(Vector3 start, Vector3 target, float overTime)
    {
        crRunning = true;
        float startTime = Time.time;
        while (Time.time < (startTime + overTime))
        {
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