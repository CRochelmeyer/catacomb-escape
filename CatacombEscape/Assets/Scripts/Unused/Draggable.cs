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
	public TutorialLogic tutorialLogic;

	private string cell;
	private Tile tile;
	private Vector3 locationToReturn;

	void Start()
	{
		//make use of the game object to pass
		if (PlayerPrefs.GetString ("TutorialScene") == "true")
			tutorialLogic = GameObject.FindObjectOfType<TutorialLogic> ();
		else
			gameLogic = GameObject.FindObjectOfType<GameLogic> ();
	}

    public void OnBeginDrag(PointerEventData eventData)
    {
		bool deletingTile = false;
		if (PlayerPrefs.GetString ("TutorialScene") != "true")
			deletingTile = gameLogic.DeletingTile;

        //Debug.Log("Player settings drag");
		if (PlayerPrefs.GetString ("Paused") != "true" && !deletingTile)
		{
			//Debug.Log("OnbeginDrag");
			// Save the parent incase of returns from invalid drags
			locationToReturn = this.transform.position;
			// Change tile index in the hierarchy
			int siblingNumber = this.transform.parent.childCount;
			this.transform.SetSiblingIndex (siblingNumber - 1);
		}
    }

    public void OnDrag(PointerEventData eventData)
	{
		bool deletingTile = false;
		if (PlayerPrefs.GetString ("TutorialScene") != "true")
			deletingTile = gameLogic.DeletingTile;

		if (PlayerPrefs.GetString ("Paused") != "true" && !deletingTile)
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
		bool deletingTile = false;
		if (PlayerPrefs.GetString ("TutorialScene") != "true")
			deletingTile = gameLogic.DeletingTile;

		if (PlayerPrefs.GetString ("Paused") != "true" && !deletingTile)
		{			
			// id to grab image source file to pass as a parameter for logic
			id = this.GetComponent<Image> ();
			//setting imageID
			imageID = id.sprite.name.ToString ();

			if (PlayerPrefs.GetString ("TutorialScene") == "true")
				cell = tutorialLogic.MouseLocation;
			else
				cell = gameLogic.MouseLocation;
			
			GameObject temp = GameObject.Find (cell);

			if (PlayerPrefs.GetString ("TutorialScene") == "true" && !tutorialLogic.PlacementValid(cell))
			{
				cell = "";
			}

			// If cell value is valid and cell is not already occupied
			if (cell != "" && temp.GetComponent<Image> ().sprite == null)
			{
				tile = new Tile (imageID, cell);

				if (PlayerPrefs.GetString ("TutorialScene") == "true")
					tutorialLogic.UpdateDrag (tile, cell);
				else
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


