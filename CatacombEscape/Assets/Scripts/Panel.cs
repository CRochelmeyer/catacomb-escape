using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Panel : MonoBehaviour, IPointerEnterHandler
{ 
	
	public void OnPointerEnter (PointerEventData eventData)
	{	
		if (eventData.eligibleForClick) {
			Debug.Log ("MouseLocation: " + this.transform.name);
			GameLogic gameLogic = GameObject.FindObjectOfType<GameLogic> ();
			gameLogic.MouseLocation = this.transform.name;
		}
	}
}
