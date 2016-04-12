using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    //use to handle when the tile is drag in an invalid spot return to position...
    //not used at the moment 12/04
    //public Transform parentToReturn = null;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnbeginDrag");
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
        //this.transform.SetParent(parentToReturn);
        //send mouse position and string of the sprite name to logic
    }
}
