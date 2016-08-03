using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuBehavior : MonoBehaviour
{	
	public GameObject highScorePanel;
	public GameObject noHighScorePanel;
	public Text highScoreValue;

	public GameObject statsPanel;
	public Text highscore;
	public Text lvlNoCleared;
	public Text lvlPointTot;
	public Text greenCollected;
	public Text greenPointTot;
	public Text redAvoided;
	public Text redPointTot;
	public Text tileNoPlaced;
	public Text tileNoTot;
	public Text pointTot;

	// Use this for initialization
	void Start ()
	{
		string hs = PlayerPrefs.GetString ("HighScore");

		if (hs != null && hs != "") //if player has a highscore
		{
			noHighScorePanel.SetActive (false);
			highScorePanel.SetActive (true);
			highScoreValue.text = hs;
		}
		//Screen.SetResolution (650, 1040, false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene (sceneName);
		PlayerPrefs.SetString ("PlayFromMenu", "true");
	}
	
	public void Quit()
	{
		Application.Quit ();
	}
	
	public void ResetPlayerPrefs()
	{
		PlayerPrefs.SetString ("FirstPlay", "true");
		PlayerPrefs.SetString ("HighScore", "");
		SceneManager.LoadScene ("menu");
	}

	public void DisplayHighscoreStats()
	{
		statsPanel.SetActive (true);
		highscore.text = PlayerPrefs.GetString ("HighScore");
		lvlNoCleared.text = PlayerPrefs.GetString ("LvlNoCleared");
		lvlPointTot.text = PlayerPrefs.GetString ("LvlPointTot");
		greenCollected.text = PlayerPrefs.GetString ("GreenCollected");
		greenPointTot.text = PlayerPrefs.GetString ("GreenPointTot");
		redAvoided.text = PlayerPrefs.GetString ("RedAvoided");
		redPointTot.text = PlayerPrefs.GetString ("RedPointTot");
		tileNoPlaced.text = PlayerPrefs.GetString ("TileNoPlaced");
		tileNoTot.text = PlayerPrefs.GetString ("TileNoTot");
		pointTot.text = PlayerPrefs.GetString ("HighScore");

		StartCoroutine (ClickToClose (statsPanel));
	}

	IEnumerator ClickToClose (GameObject panel)
	{
		do
		{
			yield return null;
		}while (!Input.GetMouseButtonUp (0));
		panel.SetActive (false);
	}
}
