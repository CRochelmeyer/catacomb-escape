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
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnbeginDrag");
        Debug.Log("this <image>: " + this.GetComponent<Image>());
        // id to grab image source file to pass as a parameter for logic
        id = this.GetComponent<Image>();
        Debug.Log("id.sprite " + id.sprite);

        //save the parent incase of returns
        // parentToReturn = this.transform.parent;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("onDrag");
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        //use these x,y to pass thru to logic to identify array cell block
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;

        Debug.Log("x : " + x + "y : " + y);
        //make use of the game object to pass
        //GameLogic Main = new GameLogic();
        imageID = id.sprite.ToString();
        gameLogic = GameObject.FindObjectOfType<GameLogic>();
        gameLogic.TestPassing(imageID, x, y);
        //this.transform.SetParent(parentToReturn);
        //send mouse position and string of the sprite name to logic
    }
}