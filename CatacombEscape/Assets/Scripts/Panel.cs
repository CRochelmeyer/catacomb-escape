using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Panel : MonoBehaviour//, IPointerEnterHandler
{ 
	public bool MouseOverPanel()
	{
		GameLogic gameLogic = GameObject.FindObjectOfType<GameLogic> ();
		Vector2 mousePosition = Input.mousePosition;
		Vector3[] worldCorners = new Vector3[4];
		RectTransform panel = this.gameObject.GetComponent<RectTransform>();

		panel.GetWorldCorners(worldCorners);
		
		if (mousePosition.x >= worldCorners [0].x && mousePosition.x < worldCorners [2].x 
			&& mousePosition.y >= worldCorners [0].y && mousePosition.y < worldCorners [2].y)
		{
			gameLogic.MouseLocation = panel.name;
			//Debug.Log (panel.name);
			return true;
		} else
		{
			return false;
		}
	}
}
