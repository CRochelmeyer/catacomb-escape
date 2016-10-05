using UnityEngine;
using System.Collections;

public class ManageRemoveTile : MonoBehaviour
{
	public GameObject topPanelOverlay;
	public GameObject bottomPanelOverlay;
	public Transform gridPanelsOverlayParent;

	private int[] currentPanelIndex;
	private GameObject[] gridPanelsOverlay;

	void Start()
	{
		int childCount = gridPanelsOverlayParent.childCount;
		gridPanelsOverlay = new GameObject[childCount];
		for (int i = 0; i < childCount; i++)
		{
			gridPanelsOverlay [i] = gridPanelsOverlayParent.GetChild (i).gameObject;
		}
	}
	
	public void DisplayOverlays (string[] invalidTileNames)
	{
		topPanelOverlay.SetActive (true);
		bottomPanelOverlay.SetActive (true);

		int count = invalidTileNames.Length;
		currentPanelIndex = new int[count];
		for (int i = 0; i < count; i++)
		{
			for (int j = 0; j < gridPanelsOverlay.Length; j++)
			{
				if (gridPanelsOverlay[j].name == invalidTileNames[i])
				{
					gridPanelsOverlay[j].SetActive (true);
					currentPanelIndex[i] = j;
				}
			}
		}
	}

	public void HidePanelOverlays ()
	{
		topPanelOverlay.SetActive (false);
		bottomPanelOverlay.SetActive (false);

		for (int i = 0; i < currentPanelIndex.Length; i++)
		{
			gridPanelsOverlay [currentPanelIndex[i]].SetActive (false);
		}
	}
}
