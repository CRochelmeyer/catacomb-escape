///
/// Class not in use.
///

using UnityEngine;
using System.Collections;

public class tempBtn : MonoBehaviour
{

    public GameLogic gameLogic;

    /// <summary>
    /// Unused (11/03/16)
    /// </summary>
    public void TryAgain()
    {

        Debug.Log("onClick NextLv");
        //grab gameLogic (gameManager?) into gamelogic
        gameLogic = FindObjectOfType<GameLogic>();
        //execute gamelogic function generate hand
        //gameLogic.TryAgain();
    }
}