using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    public GameObject player;
    public GameObject tempobj;
    public Rigidbody2D rb2d;

    public void DrawPlayer(int pLoc, GameObject[] pGrid)
    {
        Debug.Log("DrawPlayer");
        Vector3 v3location = pGrid[pLoc].transform.localPosition;
        player = GameObject.Find("Character");
        player.transform.localPosition = (new Vector3(v3location.x, v3location.y, 1));
        player.transform.localScale.Set(55, 55, 0);
        tempobj = GameObject.FindGameObjectWithTag("Grid");
        player.transform.SetParent(tempobj.transform, false);
    }
    public void UpdatePlayer(int pLoc, GameObject[] pGrid , string pdir)
    {
        Debug.Log("Moving player");
        player = GameObject.Find("Character");
        Vector3 current = player.transform.localPosition;
        Vector3 moveto = pGrid[pLoc].transform.localPosition;
        int speed = 100;
        Debug.Log("Current x:" + player.transform.localPosition.x+ " next x :"+moveto.x);
        Debug.Log("Current y:" + player.transform.localPosition.y + " next y :" + moveto.y);
        Debug.Log("pdir"+pdir);
        //if the movement is
        if (pdir != "" && pdir != "invalid move")
        {
            switch (pdir)
            {
                case "up":
                    {
                        Debug.Log("move up");
                        Debug.Log("position diff :" + (moveto.y - player.transform.localPosition.y));
                        player.transform.Translate(0, (120), 0);
                        break;
                    }
                case "right":
                    {
                        Debug.Log("move right");
                        Debug.Log("position diff :" + (moveto.x - player.transform.localPosition.x));
                        player.transform.Translate(120,0, 0);
                        break;
                    }
                case "down":
                    {
                        Debug.Log("move down");
                        Debug.Log("position diff :" + (moveto.y - player.transform.localPosition.y));
                        Debug.Log(Mathf.Abs(moveto.y - player.transform.localPosition.y) >= 0);
                        float distance = Mathf.Abs(moveto.y - player.transform.localPosition.y);
                        /*while(distance >= 0)
                        {
                            Debug.Log("down");
                            player.transform.Translate(0, -(speed * Time.deltaTime), 0);
                            distance = Mathf.Abs(moveto.y - player.transform.localPosition.y);
                        }*/
                        player.transform.Translate(0, -(120), 0);
                        break;
                    }
                case "left":
                    {
                        Debug.Log("move left");
                        Debug.Log("position diff :" + (moveto.x - player.transform.localPosition.x));
                        player.transform.Translate(-(120),0, 0);
                        break;
                    }
            }
        }   
        Debug.Log("move end");  
    }
}