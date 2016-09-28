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
    bool crUpdatePlayer = false;

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
                    default:
                        animator.SetInteger("Direction", 0);
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
    public void UpdatePlayer(GameObject[] panel, List<string> path )
    {
        StartCoroutine(MovePath(panel, path));
    }
    IEnumerator MovePath(GameObject[] panel, List<string> path)
    {
        for(int i=0; i<(path.Count-1);i++)
        {
            Debug.Log("While MovePath player");
            int index;
            cellindex.TryGetValue(path[i+1], out index);
            float distance = Vector3.Distance(player.transform.localPosition, panel[index].transform.localPosition);
            Debug.Log("distance :" + distance);
            float overTime = distance / 50;
            Debug.Log("overTime :" + overTime);
            //set animation get pDirection
            string direction = moveDir.MoveDirection(path[i], path[i+1]);
            Debug.Log("pdir :" + direction);
            SetAnimation(direction);
            StartCoroutine(UpdatePlayerCoroutine(player.transform.localPosition, panel[index].transform.localPosition, overTime));
            if (crRunning == true)
            {
                Debug.Log("move path CR " + crRunning);
                yield return new WaitForSeconds(overTime);
                //update logic of the player position per tile grid move allowing enemies to move. having this code here as this coroutine contains playerloc already
                GameLogic gameLogic = GameObject.FindObjectOfType<GameLogic>();
                gameLogic.SetPlayerLoc(path[i+1]);
            }
        }
        Debug.Log("MovePath end");
        yield return null;
    }
    IEnumerator UpdatePlayerCoroutine(Vector3 start, Vector3 target , float overTime)
    {
        Debug.Log("StartCoroutine UPC");
        crRunning = true;
        float startTime = Time.time;
        while (Time.time < (startTime + overTime))
        {
            //Debug.Log(Time.time + " :: " + (startTime + overTime) + "overTime : "+ overTime);
            //Debug.Log("While COroutine");
            player.transform.localPosition = Vector3.Lerp(start, target, (Time.time - startTime) / overTime);
            //Debug.Log("Distance between :: " + Vector3.Distance(player.transform.localPosition, target));
            if (Vector3.Distance(player.transform.localPosition,target) < 3)
            {
                Debug.Log("If distance in range");
                //set player to target and stop animation
                //disable animation
                Debug.Log("Stop animation");
                SetAnimation("stop");
                player.transform.localPosition = target;
            }
            yield return null;
        }
        crRunning = false;
        Debug.Log("crRunning == " + crRunning);
    }
    public void SetAnimation(string pDirection)
    {
        Debug.Log("SetAnimation");
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
}