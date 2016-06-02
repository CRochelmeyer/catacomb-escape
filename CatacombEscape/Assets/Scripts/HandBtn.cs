using UnityEngine;
using System.Collections;

public class HandBtn : MonoBehaviour {
    public GameObject[] handTiles;
    public GameObject[] _hand;
    public Canvas ui;
    public GameLogic gameLogic;
    public void getHand()
    {

        //Debug.Log("onClick");
        //grab gameLogic (gameManager?) into gamelogic
        gameLogic = FindObjectOfType<GameLogic>();
        //execute gamelogic function generate hand
        gameLogic.NewHand();
      /*  _hand = GameObject.FindGameObjectsWithTag("handTile");
        Debug.Log("found with tag");
        RectTransform panel = GameObject.Find("BottomPanel");
        if (_hand.Length != 0)
        {
            for (int i = 0; i < _hand.Length; i++)
            {

                GameObject handChoice = handTiles[Random.Range(0, handTiles.Length)];
                Transform target = _hand[i].transform;
                Debug.Log(_hand[i].transform.parent);
                Debug.Log("target pos " + target.position);
                target.transform.SetParent(_hand[i].transform.parent);
                var test = Instantiate(handChoice, target.position, Quaternion.identity);
                Destroy(_hand[i]);
                Debug.Log("for loop" + i);
            }
        }   
        else { Debug.Log("_hand is empty"); }*/
    }
    public void TryAgain()
    {

    }

    public void Menu()
    {

    }
}
