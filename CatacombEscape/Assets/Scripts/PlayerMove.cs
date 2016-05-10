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
        player = Instantiate(Resources.Load("CharacterSprite")) as GameObject;
        player.transform.localPosition = (new Vector3(v3location.x, v3location.y, 1));
        player.transform.localScale.Set(55, 55, 0);
        tempobj = GameObject.FindGameObjectWithTag("Grid");
        player.transform.SetParent(tempobj.transform, false);
    }
    public void UpdatePlayer(int pLoc, GameObject[] pGrid)
    {
    
        player = GameObject.FindGameObjectWithTag("Player");
        Vector3 current = player.transform.localPosition;
        int speed = 2;
        player.transform.localPosition = Vector3.MoveTowards(current, pGrid[pLoc].transform.localPosition, (speed * Time.deltaTime));
    }
}