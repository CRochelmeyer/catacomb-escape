using UnityEngine;
using System.Collections;

public class GridSizeModifier : MonoBehaviour
{
	public GameObject grid;
	public GameObject panelsParent;
	public GameObject ui;
	public GameObject topPanel;
	public GameObject bottomPanel;
	public int gridHeight;
	public int gridWidth;


	void Awake()
	{
		float finalGridHeight;
		float finalGridWidth;

		RectTransform uiRectTrans = ui.GetComponent<RectTransform>();
		float uiHeight = (float) uiRectTrans.rect.height;
		float uiWidth = (float) uiRectTrans.rect.width;
		Debug.Log ("UI Height: " + uiHeight);
		Debug.Log ("UI Width: " + uiWidth);

		RectTransform topPanelRectTrans = topPanel.GetComponent<RectTransform>();
		float topPanelHeight = (float) topPanelRectTrans.rect.height;
		Debug.Log ("topPanel Height: " + topPanelHeight);
		
		RectTransform bottomPanelRectTrans = bottomPanel.GetComponent<RectTransform>();
		float bottomPanelHeight = (float) bottomPanelRectTrans.rect.height;
		Debug.Log ("bottomPanel Height: " + bottomPanelHeight);

		finalGridHeight = uiHeight - topPanelHeight - bottomPanelHeight;

		finalGridWidth = (finalGridHeight / gridHeight) * gridWidth;

		if (finalGridWidth > uiWidth)
		{
			finalGridWidth = uiWidth;
			finalGridHeight = (finalGridWidth / gridWidth) * gridHeight;

		}

		RectTransform gridRectTrans = grid.GetComponent<RectTransform>();
		RectTransform panelsParentRectTrans = panelsParent.GetComponent<RectTransform>();

		gridRectTrans.sizeDelta = new Vector2(finalGridWidth, finalGridHeight);
		panelsParentRectTrans.sizeDelta = new Vector2(finalGridWidth, finalGridHeight);
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
