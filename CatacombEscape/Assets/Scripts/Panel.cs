using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Panel : MonoBehaviour, IPointerEnterHandler
{ 
	
	public void OnPointerEnter (PointerEventData eventData)
	{	
		Debug.Log ("MouseLocation: " + this.transform.name);
		GameLogic gameLogic = GameObject.FindObjectOfType<GameLogic> ();
		gameLogic.MouseLocation = this.transform.name;
	}
}
