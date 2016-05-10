using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour
{	
	//public GameObject GridPanelsPrefab;
    public GameObject GameManager;

    void Awake()
    {
        if (GameLogic.instance == null)
        {
            //Debug.Log("instantiating GridPanelsPrefab");
            //Instantiate(GridPanelsPrefab);

            Debug.Log("instantiating GameManager");
            Instantiate(GameManager);
        }
    }/*
    void Start()
    {
        if (GameLogic.instance == null)
        {
            //Debug.Log("instantiating GridPanelsPrefab");
            //Instantiate(GridPanelsPrefab);

            Debug.Log("instantiating GameManager");
            Instantiate(GameManager);
        }
    }*/
}
