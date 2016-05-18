using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
	public GameObject player;
	public int playerDisp = 4;

	public Animator animator;
	private GameObject tempobj;
	private Rigidbody2D rb2d;
	private string pDirection = "";
	Vector3 targetPosition;
	Vector3 initialPosition;

	public float speed = 0.01f;
	private float startTime;
	private float journeyLength;


	public void Start()
	{

	}

	public void FixedUpdate()
	{
		GameLogic gameLogic = GameObject.FindObjectOfType<GameLogic> ();
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

			player.transform.position = Vector3.Lerp(initialPosition, targetPosition, fracJourney);

			if (player.transform.position == targetPosition)
			{
				animator.SetInteger ("Direction", 0);
				gameLogic.SetPlayerLoc();
			}
		}
	}
	
	public void DrawPlayer(int pLoc, GameObject[] pGrid)
	{
		Debug.Log("DrawPlayer");
		Vector3 tempPos = pGrid[pLoc].transform.position;
		tempPos = new Vector3 (tempPos.x, tempPos.y - playerDisp, tempPos.z);
		player.transform.position = tempPos;
		if (player.transform.position == tempPos)
			Debug.Log(tempPos);
    }

    public void UpdatePlayer(GameObject panel, string pdir)
    {
        Debug.Log("Moving player");
		pDirection = pdir;
		Vector3 tempPos = panel.transform.position;
		tempPos = new Vector3 (tempPos.x, tempPos.y - playerDisp, tempPos.z);
		targetPosition = tempPos;
		initialPosition = player.transform.position;
		journeyLength = Vector3.Distance(initialPosition, targetPosition);
		startTime = Time.time;

		//Debug.Log("move end");  
	}
}