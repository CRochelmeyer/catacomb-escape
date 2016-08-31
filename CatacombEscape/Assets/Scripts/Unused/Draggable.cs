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

    /// <summary>
    /// Records the pickup location of a tile in the event of an invalid placement, the tile can be returned.
    /// Called as soon as a player starts to drag a tile.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag()");
		if (PlayerPrefs.GetString ("Paused") != "true")
		{
			// Save the parent incase of returns from invalid drags
            // This is the location the tile is picked up from.
			locationToReturn = this.transform.position;
		}
    }

    /// <summary>
    /// Updates tile position.
    /// Called when a player is dragging a tile around.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
	{
		if (PlayerPrefs.GetString ("Paused") != "true")
		{
			Vector3 inputPosition = Input.mousePosition;

            // Sets ray to location of tile being dragged. i.e. the player's finger position.
			Vector3 ray = Camera.main.ScreenToWorldPoint (inputPosition);
			ray = new Vector3 (ray.x, ray.y, 0);

            // Update the position of the tile, to that of the ray.
			this.GetComponent<RectTransform>().position = ray;
		}
	}

    /// <summary>
    /// Called when player releases mouse, if they're holding a tile.
    /// </summary>
    /// <param name="eventData"></param>
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
			
            // Set the cell to the one that the mouse is over.
			cell = gameLogic.MouseLocation;

			GameObject temp = GameObject.Find (cell);

            // If valid, place the tile.
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

