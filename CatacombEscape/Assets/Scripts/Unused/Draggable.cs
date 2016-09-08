﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //use to handle when the tile is drag in an invalid spot return to position...
    //not used at the moment 12/04  
    //public Transform parentToReturn = null;
    public Image id;
    public string imageID;
	public GameLogic gameLogic;

	private string cell;
	private Tile tile;
	private Vector3 locationToReturn;

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Player settings drag");
		if (PlayerPrefs.GetString ("Paused") != "true")
		{
			//Debug.Log("OnbeginDrag");
			//save the parent incase of returns from invalid drags
			locationToReturn = this.transform.position;
			//locationToReturn = this.transform.TransformPoint (Vector3.zero);
		}
    }

    public void OnDrag(PointerEventData eventData)
	{
		if (PlayerPrefs.GetString ("Paused") != "true")
		{
			//Debug.Log("MousePosition = " + Input.mousePosition);
			Vector3 inputPosition = Input.mousePosition;
			Vector3 ray = Camera.main.ScreenToWorldPoint (inputPosition);
			ray = new Vector3 (ray.x, ray.y, 0);
			//this.transform.position = temp;
			this.GetComponent<RectTransform>().position = ray;
			//this.transform.TransformPoint (eventData.position);
		}
	}

    public void OnEndDrag(PointerEventData eventData)
    {
		if (PlayerPrefs.GetString ("Paused") != "true")
		{
			//make use of the game object to pass
			gameLogic = GameObject.FindObjectOfType<GameLogic> ();
			// id to grab image source file to pass as a parameter for logic
			id = this.GetComponent<Image> ();
			//setting imageID
			imageID = id.sprite.name.ToString ();

			cell = gameLogic.MouseLocation;
			GameObject temp = GameObject.Find (cell);

			// If cell value is valid and cell is not already occupied
			if (cell != "" && temp.GetComponent<Image> ().sprite == null)
			{
				tile = new Tile (imageID, cell);
				gameLogic.UpdateDrag (tile, cell);
				Destroy (this.gameObject);
			}
	        else
            {
				this.gameObject.GetComponent<Transform> ().position = locationToReturn;
			}
		}
	}
}


