using UnityEngine;
using System.Collections;

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

	private float panelHeight;

	private GameLogic gameLogic = null;
	private TutorialLogic tutorialLogic = null;

	void Start ()
	{
		if (PlayerPrefs.GetString ("TutorialScene") == "true")
			tutorialLogic = GameObject.FindObjectOfType<TutorialLogic> ();
		else
			gameLogic = GameObject.FindObjectOfType<GameLogic> ();
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
						//animator.Stop();
						animator.SetInteger ("Direction", 3); //3=climb
						//animator.Play ("Character_Climb");
						break;

					case "down":
						//animator.Stop();
						animator.SetInteger ("Direction", 3); //3=climb
						//animator.Play ("Character_Climb");
						break;

					case "left":
						//animator.Stop();
						animator.SetInteger ("Direction", 1); //1=left
						//animator.Play ("Character_Left_Walk");
						break;

					case "right":
						//animator.Stop();
						animator.SetInteger ("Direction", 2); //2=right
						//animator.Play ("Character_Right_Walk");
						break;
				}

				player.transform.localPosition = Vector3.Lerp (initialPosition, targetPosition, fracJourney);

				if (player.transform.localPosition == targetPosition)
				{
					//animator.Stop();
					animator.SetInteger ("Direction", 0); // Set animation to idle
					//animator.Play ("Character_Idle");
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
							gameLogic.SetPlayerLoc ();
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
		Debug.Log ("Initial Position: " + initialPosition);
		Debug.Log ("Target Position: " + targetPosition);
		journeyLength = Vector3.Distance (initialPosition, targetPosition);
		startTime = Time.time;
		moving = true;
		exitingLevel = true;
		FixedUpdate ();
	}
}