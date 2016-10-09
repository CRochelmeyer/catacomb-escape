using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour
{
	public GameObject resetPanel;

	[SerializeField]
	private int index = 0;

	public void Reset1()
	{
		index = 1;
		StartCoroutine (ResetIndex ());
	}

	public void Reset2()
	{
		if (index == 1)
			index = 2;
	}

	public void Reset3()
	{
		if (index == 2)
			index = 3;
	}

	public void Reset4()
	{
		if (index == 3)
		{
			resetPanel.SetActive (true);
		}
	}

	IEnumerator ResetIndex()
	{
		yield return new WaitForSeconds (5);
		index = 0;
	}

	public void ResetPlayerPrefs()
	{
		PlayerPrefs.SetString ("FirstPlay", "true");
		PlayerPrefs.SetString ("HighScore", "");
		PlayerPrefs.SetInt ("Diamonds", 0);
		SceneManager.LoadScene ("menu");
	}
}
