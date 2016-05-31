using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Panel : MonoBehaviour//, IPointerEnterHandler
{ 
	public bool MouseOverPanel()
	{
		GameLogic gameLogic = GameObject.FindObjectOfType<GameLogic> ();
		Vector3 inputPosition = Input.mousePosition;
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint (inputPosition);
		mousePosition = new Vector3 (mousePosition.x, mousePosition.y, 0);
		Vector3[] worldCorners = new Vector3[4];
        //Matrix4x4 test = new Matrix4x4();

        //grabs the grid panels rectransform component
		RectTransform panel = this.gameObject.GetComponent<RectTransform>();

        //grabs the world corner of the individual grid panel into world corners
		panel.GetWorldCorners(worldCorners);

		//Dont use this, it puts way too much pressure on the system and delays response time for about 5 seconds!!
		/*
        Debug.Log(panel.name);
        Debug.Log("wc0 :: " + worldCorners[0]);
        Debug.Log("wc1 :: " + worldCorners[1]);
        Debug.Log("wc2 :: " + worldCorners[2]);
        Debug.Log("wc3 :: " + worldCorners[3]);
        this.gameObject.GetComponent<Panel>().GetComponent<RectTransform>().GetWorldCorners(worldCorners);
        Debug.Log(panel.name + " :::::::");
        Debug.Log("wc0 :: " + worldCorners[0]);
        Debug.Log("wc1 :: " + worldCorners[1]);
        Debug.Log("wc2 :: " + worldCorners[2]);
        Debug.Log("wc3 :: " + worldCorners[3]);
		*/

        //check mouse coord within the 4 corner of the gridpanel
        if (mousePosition.x >= worldCorners [0].x && mousePosition.x < worldCorners [2].x 
			&& mousePosition.y >= worldCorners [0].y && mousePosition.y < worldCorners [2].y)
		{
			gameLogic.MouseLocation = panel.name;
			Debug.Log ("Mouse in panel " + panel.name);
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
