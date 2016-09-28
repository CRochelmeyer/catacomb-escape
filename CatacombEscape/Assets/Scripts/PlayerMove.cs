using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMove : MonoBehaviour
{
    //dictionary to match cell strings of 00-04 10-14 to an index from 0-29
    Dictionary<string, int> cellindex = new Dictionary<string, int>();
    public GameObject player;
	public int playerDisp = 4;
    public Direction moveDir;
	public Animator animator;
	private GameObject tempobj;
	private Rigidbody2D rb2d;
	private string pDirection = "";
	Vector3 targetPosition;
	Vector3 initialPosition;
    bool crRunning = false;

	public float speed = 0.01f;
	private float startTime;
	private float journeyLength;
	private bool moving = false;


	public void Start()
	{
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
        moveDir = new Direction();
    }

	public void FixedUpdate()
	{
		if (moving) {
            //Debug.Log("if moving");
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
                //Debug.Log("InitPos" + initialPosition + " Destination :" + targetPosition);
				player.transform.localPosition = Vector3.Lerp (initialPosition, targetPosition, fracJourney);

				if (player.transform.localPosition == targetPosition) {
					animator.SetInteger ("Direction", 0);
					gameLogic.SetPlayerLoc ();
                    moving = false;
                    Debug.Log("movement end fixedupdate");
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
    
    //overload to handle pathfinder multiple movements
    public void UpdatePlayer(GameObject[] panel, List<string> path)
    {
        //starting the path string 
        for (int i = 1; i < path.Count; i++)
        {
            while (crRunning == false)
            {
                int index;
                cellindex.TryGetValue(path[i], out index);
                Debug.Log("i = " + i + " " + path[i] + " :: ");
                pDirection = moveDir.MoveDirection(path[i - 1], path[i]);
                StartCoroutine("UpdatePlayerCoroutine", panel[index]);
            }
        }
    }

    IEnumerator UpdatePlayerCoroutine(GameObject target)
    {
        Debug.Log("Start Coroutine ");
        crRunning = true;
        this.UpdatePlayer(target, pDirection);
        yield return new WaitForSeconds(0f);
        Debug.Log("after yield set crRunning");

    }
    public bool isMoving()
    {
        return moving;
    }
}