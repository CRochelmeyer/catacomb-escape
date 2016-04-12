using UnityEngine;
using System.Collections;

public class tempBtn : MonoBehaviour
{

    public GameLogic gameLogic;

    public void nextLevel()
    {

        Debug.Log("onClick temp");
        //grab gameLogic (gameManager?) into gamelogic
        gameLogic = FindObjectOfType<GameLogic>();
        //execute gamelogic function generate hand
        gameLogic.NextLevel();
    }
}