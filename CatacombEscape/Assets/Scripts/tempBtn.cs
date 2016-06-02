using UnityEngine;
using System.Collections;

public class tempBtn : MonoBehaviour
{

    public GameLogic gameLogic;

    public void TryAgain()
    {

        Debug.Log("onClick NextLv");
        //grab gameLogic (gameManager?) into gamelogic
        gameLogic = FindObjectOfType<GameLogic>();
        //execute gamelogic function generate hand
        gameLogic.TryAgain();
    }
}