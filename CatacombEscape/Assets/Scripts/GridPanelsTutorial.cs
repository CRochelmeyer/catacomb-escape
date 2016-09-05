using UnityEngine;
using System.Collections;

/// <summary>
/// Background tiles of the play area
/// </summary>
public class GridPanelsTutorial : MonoBehaviour
{
	public GameObject panel00;
	public GameObject panel01;
	public GameObject panel02;
	public GameObject panel10;
	public GameObject panel11;
	public GameObject panel12;
	public GameObject panel20;
	public GameObject panel21;
	public GameObject panel22;
	public GameObject panel30;
	public GameObject panel31;
	public GameObject panel32;

	GameObject[] gridPanels;

	public GameObject GetGridPanel (int i)
	{
		SetGridPanels ();
		return gridPanels[i];
	}

	private void SetGridPanels ()
	{
		gridPanels = new GameObject[12];
		gridPanels[0] = panel00;
		gridPanels[1] = panel01;
		gridPanels[2] = panel02;
		gridPanels[3] = panel10;
		gridPanels[4] = panel11;
		gridPanels[5] = panel12;
		gridPanels[6] = panel20;
		gridPanels[7] = panel21;
		gridPanels[8] = panel22;
		gridPanels[9] = panel30;
		gridPanels[10] = panel31;
		gridPanels[11] = panel32;
	}
}
