﻿using UnityEngine;
using System.Collections;

public class MovingPlayer : MonoBehaviour {
    GameObject temp;

	public void MovePlayerTo(Vector3 pDestination)
    {
        temp = this.gameObject;


    }

    void Update()
    {
        //testing update  
        Debug.Log("movingplayerscript");
    }
}
