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
	public Text greenCollected;
	public Text redAvoided;
	public Text tileNoPlaced;

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

    // Not in use 18/08/16
	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene (sceneName);
		PlayerPrefs.SetString ("PlayFromMenu", "true");
	}

    // Not in use 18/08/16
    public void Quit()
	{
		Application.Quit ();
	}

    // Not in use 18/08/16
    public void ResetPlayerPrefs()
	{
		PlayerPrefs.SetString ("FirstPlay", "true");
		PlayerPrefs.SetString ("HighScore", "");
		SceneManager.LoadScene ("menu");
	}

    // Not in use 18/08/16
    public void DisplayHighscoreStats()
	{
		statsPanel.SetActive (true);
		highscore.text = PlayerPrefs.GetString ("HighScore");
		lvlNoCleared.text = PlayerPrefs.GetString ("LvlNoCleared");
		greenCollected.text = PlayerPrefs.GetString ("GreenCollected");
		redAvoided.text = PlayerPrefs.GetString ("RedAvoided");
		tileNoPlaced.text = PlayerPrefs.GetString ("TileNoPlaced");

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
