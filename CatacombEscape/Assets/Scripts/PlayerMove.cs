using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
	public GameObject player;

	private Animator anim;
	private GameObject tempobj;
	private Rigidbody2D rb2d;
	private int distToMove = 0;
	private string pDirection = "";

	public float speed = 10.0f;
	public float threshold = 0.5f;


	public void Start()
	{
		anim = player.GetComponent<Animator> ();
	}

	public void FixedUpdate()
	{
		Vector3 targetPosition;
		Vector3 direction;


		if (pDirection != "" && pDirection != "invalid move")
		{
			switch (pDirection)
			{
				case "up": //---------------------------------------------------------------------
				{
				//Debug.Log("position diff :" + (moveto.y - player.transform.localPosition.y));
				anim.SetInteger ("Direction", 3); //3=climb
				targetPosition = new Vector3 (player.transform.position.x, distToMove, player.transform.position.z);
				direction = targetPosition - player.transform.position;
				if(direction.magnitude > threshold)
				{
					direction.Normalize();
					player.transform.position = player.transform.position + direction * speed * Time.deltaTime;
				}else
				{ 
					// Without this game object jumps around target and never settles
					player.transform.position = targetPosition;
				}
				anim.SetInteger ("Direction", 0);
				break;
				}
				case "down": //---------------------------------------------------------------------
				{
				anim.SetInteger ("Direction", 3); //3=climb
				targetPosition = new Vector3 (player.transform.position.x, -distToMove, player.transform.position.z);
				direction = targetPosition - player.transform.position;
				Debug.Log ("targetPosition: " + targetPosition);
				Debug.Log ("player.transform.position: " + player.transform.position);
				Debug.Log ("direction: " + direction);
				if(direction.magnitude > threshold)
				{
					direction.Normalize();
					player.transform.position = player.transform.position + direction * speed * Time.deltaTime;
				}else
				{ 
					// Without this game object jumps around target and never settles
					player.transform.position = targetPosition;
				}
				anim.SetInteger ("Direction", 0);
				break;
				}
				case "left": //---------------------------------------------------------------------
				{
				//Debug.Log("position diff :" + (moveto.x - player.transform.localPosition.x));
				anim.SetInteger ("Direction", 1); //1=left
				targetPosition = new Vector3 (-distToMove, player.transform.position.y, player.transform.position.z);
				direction = targetPosition - player.transform.position;
				Debug.Log ("direction: " + direction);
				if(direction.magnitude > threshold)
				{
					direction.Normalize();
					player.transform.position = player.transform.position + direction * speed * Time.deltaTime;
				}else
				{ 
					// Without this game object jumps around target and never settles
					player.transform.position = targetPosition;
				}
				//player.transform.Translate(-(120),0, 0);
				anim.SetInteger ("Direction", 0);
				break;
				}
				case "right": //---------------------------------------------------------------------
				{
				//Debug.Log("position diff :" + (moveto.x - player.transform.localPosition.x));
				anim.SetInteger ("Direction", 2); //2=right
				targetPosition = new Vector3 (distToMove, player.transform.position.y, player.transform.position.z);
				direction = player.transform.position - targetPosition;
				Debug.Log ("direction: " + direction);
				if(direction.magnitude > threshold)
				{
					direction.Normalize();
					player.transform.position = player.transform.position + direction * speed * Time.deltaTime;
				}else
				{ 
					// Without this game object jumps around target and never settles
					player.transform.position = targetPosition;
				}
				//player.transform.Translate(120,0, 0);
				anim.SetInteger ("Direction", 0);
				break;
			}
			}
		}
	}
	
	public void DrawPlayer(int pLoc, GameObject[] pGrid)
	{
		Debug.Log("DrawPlayer");
		Vector3 v3location = pGrid[pLoc].transform.localPosition;
		player.transform.localPosition = (new Vector3(v3location.x, v3location.y, 1));
        player.transform.localScale.Set(55, 55, 0);
		tempobj = GameObject.FindGameObjectWithTag("Grid");
		//player.transform.SetParent(tempobj.transform, false);
    }

    public void UpdatePlayer(int pLoc, GameObject[] pGrid , string pdir)
    {
        Debug.Log("Moving player");
		distToMove = 120;
		pDirection = pdir;

		/*
        player = GameObject.Find("Character");
        //Vector3 current = player.transform.localPosition;
        //Vector3 moveto = pGrid[pLoc].transform.localPosition;
        //int speed = 100;
        //Debug.Log("Current x:" + player.transform.localPosition.x+ " next x :"+moveto.x);
        //Debug.Log("Current y:" + player.transform.localPosition.y + " next y :" + moveto.y);
        //Debug.Log("pdir"+pdir);
        //if the movement is
        if (pdir != "" && pdir != "invalid move")
        {
            switch (pdir)
            {
                case "up":
                {
					//Debug.Log("position diff :" + (moveto.y - player.transform.localPosition.y));
					anim.SetInteger ("Direction", 3); //3=climb
					int distToMove = 120;
					for (int i=0; i<distToMove; i+=5)
					{
						Debug.Log("move up");
					player.GetComponent<RectTransform>().transform.Translate(0, i, 0);
					}
						anim.SetInteger ("Direction", 0);
					break;
                }
                case "down":
                {
                    //Debug.Log("position diff :" + (moveto.y - player.transform.localPosition.y));
                    //Debug.Log(Mathf.Abs(moveto.y - player.transform.localPosition.y) >= 0);
                    //float distance = Mathf.Abs(moveto.y - player.transform.localPosition.y);
                    /*while(distance >= 0)
                    {
                        Debug.Log("down");
                        player.transform.Translate(0, -(speed * Time.deltaTime), 0);
                        distance = Mathf.Abs(moveto.y - player.transform.localPosition.y);
                    }
					anim.SetInteger ("Direction", 3); //3=climb
					int distToMove = 120;
					for (int i=0; i<distToMove; i+=5)
					{
						Debug.Log("move down");
					player.GetComponent<RectTransform>().transform.Translate(0, -i, 0);
					}
					anim.SetInteger ("Direction", 0);
					break;
                    }
                case "left":
                {
					//Debug.Log("position diff :" + (moveto.x - player.transform.localPosition.x));
					anim.SetInteger ("Direction", 1); //1=left
					int distToMove = 120;
					for (int i=0; i<distToMove; i+=5)
					{
						Debug.Log("move left");
					player.GetComponent<RectTransform>().transform.Translate(-i, 0, 0);
					}
					//player.transform.Translate(-(120),0, 0);
					anim.SetInteger ("Direction", 0);
					break;
				}
				case "right":
				{
					//Debug.Log("position diff :" + (moveto.x - player.transform.localPosition.x));
					anim.SetInteger ("Direction", 2); //2=right
					int distToMove = 120;
					for (int i=0; i<distToMove; i+=5)
					{
						Debug.Log("move right");
					player.GetComponent<RectTransform>().transform.Translate(i, 0, 0);
					}
					//player.transform.Translate(120,0, 0);
					anim.SetInteger ("Direction", 0);
					break;
				}
			}
		}
		*/

		//Debug.Log("move end");  
	}
}