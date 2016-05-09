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
			locationToReturn = this.transform.localPosition;
		}
    }

    public void OnDrag(PointerEventData eventData)
	{
		if (PlayerPrefs.GetString ("Paused") != "true")
		{
			//Debug.Log("onDrag");
			this.transform.position = eventData.position;
		}
	}

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        //use these x,y to pass thru to logic to identify array cell block
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;

        //this section is for using endDrag to communicate mouse position for 
        //logic to determine cell area
        //make use of the game object to pass
        //GameLogic Main = new GameLogic();
        // id to grab image source file to pass as a parameter for logic
        id = this.GetComponent<Image>();
        //setting imageID
        imageID = id.sprite.name.ToString();
        gameLogic = GameObject.FindObjectOfType<GameLogic>();
        //this.transform.SetParent(parentToReturn);
        //send mouse position and string of the sprite name to logic
        //testing arrayhandler
        //Debug.Log("return cell: "+gameLogic.GetComponent<ArrayHandler>().FindLocation(new Vector2(x, y)) );
        //assign cell 
        cell = gameLogic.GetComponent<ArrayHandler>().FindLocation(new Vector2(x, y));
        tile = new Tile(imageID, cell);
        tile.test();
        //check if its a valid placement based on player location.
        //cal update drag from gamelogic with tile and cell index
        if (cell != null )
        {
            Debug.Log("Destroy handtile");
            gameLogic.UpdateDrag(tile, cell);
            Destroy(this.gameObject);
        }
        /*
        if (gameLogic.ValidDrag(tile, cell))
        {
            Debug.Log("Destroy handtile");
            gameLogic.UpdateDrag(tile, cell);
            Destroy(this.gameObject);
        }
        */
        else
        {
            
            Debug.Log("Return handtile " + cell);
            this.gameObject.GetComponent<Transform>().localPosition = locationToReturn;
        }
	}
}


