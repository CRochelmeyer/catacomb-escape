using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {

    public GameObject GameManager;

    void Awake()
    {
        if (GameLogic.instance == null)
        {
            Debug.Log("instantiating GameManager");
            Instantiate(GameManager);
        }
    }
}
