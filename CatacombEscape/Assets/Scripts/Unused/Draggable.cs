using UnityEngine;
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
			//Debug.Log ("OnEndDrag");
			//use these x,y to pass thru to logic to identify array cell block
			//float x = Input.mousePosition.x;
			//float y = Input.mousePosition.y;

			//this section is for using endDrag to communicate mouse position for 
			//logic to determine cell area
			//make use of the game object to pass
			gameLogic = GameObject.FindObjectOfType<GameLogic> ();
			// id to grab image source file to pass as a parameter for logic
			id = this.GetComponent<Image> ();
			//setting imageID
			imageID = id.sprite.name.ToString ();
			//this.transform.SetParent(parentToReturn);
			//send mouse position and string of the sprite name to logic
			//testing arrayhandler
			//Debug.Log("return cell: "+gameLogic.GetComponent<ArrayHandler>().FindLocation(new Vector2(x, y)) );
			//assign cell 
			//cell = gameLogic.GetComponent<ArrayHandler> ().FindLocation (new Vector2 (x, y));
			//check if its a valid placement based on player location.
			//cal update drag from gamelogic with tile and cell index
			//check cell value isnt "" and that the cell isnt already an exit etc...

			cell = gameLogic.MouseLocation;
			//Debug.Log ("Draggable cell :" + cell);
			GameObject temp = GameObject.Find (cell);

			// If cell value is valid and cell is not already occupied
			if (cell != "" && temp.GetComponent<Image> ().sprite == null)
			{
				tile = new Tile (imageID, cell);
				//Debug.Log ("Destroy handtile");
				gameLogic.UpdateDrag (tile, cell);
				Destroy (this.gameObject);
			}
	        /*
	        if (gameLogic.ValidDrag(tile, cell))
	        {
	            Debug.Log("Destroy handtile");
	            gameLogic.UpdateDrag(tile, cell);
	            Destroy(this.gameObject);
	        }
	        */
	        else {
	            
				//Debug.Log ("Return handtile " + cell);
				this.gameObject.GetComponent<Transform> ().position = locationToReturn;
			}
		}
	}
}


