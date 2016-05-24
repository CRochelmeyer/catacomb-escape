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

        //grabs the grid panels rectransform component
		RectTransform panel = this.gameObject.GetComponent<RectTransform>();

        //grabs the world corner of the individual grid panel into world corners
		panel.GetWorldCorners(worldCorners);

		//check mouse coord within the 4 corner of the gridpanel
		if (mousePosition.x >= worldCorners [0].x && mousePosition.x < worldCorners [2].x 
			&& mousePosition.y >= worldCorners [0].y && mousePosition.y < worldCorners [2].y)
		{
			gameLogic.MouseLocation = panel.name;
			//Debug.Log (panel.name);
			return true;
		} else
		{
			gameLogic.MouseLocation = "";
			return false;
		}
	}

    public string MouseClickPanel(Vector3 pClickloc)
    {
        string location = "invalid location";
        Vector3[] worldCorners = new Vector3[4];
        RectTransform panel = this.gameObject.GetComponent<RectTransform>();
        panel.GetWorldCorners(worldCorners);

        if (pClickloc.x >= worldCorners[0].x && pClickloc.x < worldCorners[2].x
           && pClickloc.y >= worldCorners[0].y && pClickloc.y < worldCorners[2].y)
        {
            location = panel.name;
            //Debug.Log(panel.name);
            //Debug.Log("location" + location);
        }
        else
        {
            location = "invalid location";
        }
        return location;
    }
}
