using UnityEngine;
using System.Collections;

public class GridPanels : MonoBehaviour
{
	public GameObject panel00;
	public GameObject panel01;
	public GameObject panel02;
	public GameObject panel03;
	public GameObject panel04;
	public GameObject panel10;
	public GameObject panel11;
	public GameObject panel12;
	public GameObject panel13;
	public GameObject panel14;
	public GameObject panel20;
	public GameObject panel21;
	public GameObject panel22;
	public GameObject panel23;
	public GameObject panel24;
	public GameObject panel30;
	public GameObject panel31;
	public GameObject panel32;
	public GameObject panel33;
	public GameObject panel34;
	public GameObject panel40;
	public GameObject panel41;
	public GameObject panel42;
	public GameObject panel43;
	public GameObject panel44;
	public GameObject panel50;
	public GameObject panel51;
	public GameObject panel52;
	public GameObject panel53;
	public GameObject panel54;

	GameObject[] gridPanels;

	public GameObject GetGridPanel (int i)
	{
		SetGridPanels ();
		return gridPanels[i];
	}

	private void SetGridPanels ()
	{
		gridPanels = new GameObject[30];
		gridPanels[0] = panel00;
		gridPanels[1] = panel01;
		gridPanels[2] = panel02;
		gridPanels[3] = panel03;
		gridPanels[4] = panel04;
		gridPanels[5] = panel10;
		gridPanels[6] = panel11;
		gridPanels[7] = panel12;
		gridPanels[8] = panel13;
		gridPanels[9] = panel14;
		gridPanels[10] = panel20;
		gridPanels[11] = panel21;
		gridPanels[12] = panel22;
		gridPanels[13] = panel23;
		gridPanels[14] = panel24;
		gridPanels[15] = panel30;
		gridPanels[16] = panel31;
		gridPanels[17] = panel32;
		gridPanels[18] = panel33;
		gridPanels[19] = panel34;
		gridPanels[20] = panel40;
		gridPanels[21] = panel41;
		gridPanels[22] = panel42;
		gridPanels[23] = panel43;
		gridPanels[24] = panel44;
		gridPanels[25] = panel50;
		gridPanels[26] = panel51;
		gridPanels[27] = panel52;
		gridPanels[28] = panel53;
		gridPanels[29] = panel54;
	}
}
