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
	private bool moving = false;


	public void Start()
	{

	}

	public void FixedUpdate()
	{
		if (moving) {
			GameLogic gameLogic = GameObject.FindObjectOfType<GameLogic> ();
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			
			if (pDirection != "" && pDirection != "invalid move") {
				switch (pDirection) {
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

				if (player.transform.localPosition == targetPosition) {
					animator.SetInteger ("Direction", 0);
					gameLogic.SetPlayerLoc ();
					moving = false;
				}
			}
		}
	}
	
	public void DrawPlayer(int pLoc, GameObject[] pGrid)
	{
		Vector3 tempPos = pGrid [pLoc].transform.localPosition;
		tempPos = new Vector3 (tempPos.x, tempPos.y - playerDisp, tempPos.z);
		player.transform.localPosition = tempPos;
    }

    public void UpdatePlayer(GameObject panel, string pdir)
    {
		pDirection = pdir;
		Vector3 tempPos = panel.transform.localPosition;
		tempPos = new Vector3 (tempPos.x, tempPos.y - playerDisp, tempPos.z);
		targetPosition = tempPos;
		initialPosition = player.transform.localPosition;
		journeyLength = Vector3.Distance(initialPosition, targetPosition);
		startTime = Time.time;
		moving = true;

		//Debug.Log("move end");  
	}
}