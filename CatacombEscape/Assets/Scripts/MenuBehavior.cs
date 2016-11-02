using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuBehavior : MonoBehaviour
{	
	public GameObject highScorePanel;
	public GameObject noHighScorePanel;
	public Text highScoreValue;
	public Text diamondAmount;

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

		if (PlayerPrefs.HasKey ("Diamonds"))
			diamondAmount.text = "x " + PlayerPrefs.GetInt ("Diamonds").ToString();
	}

	public void LoadGame()
	{
		// If player hasn't played before or game has been reset
		if (!PlayerPrefs.HasKey ("FirstPlay") || PlayerPrefs.GetString ("FirstPlay") == "true")
		{
			PlayerPrefs.SetString ("FirstPlay", "false");
			SceneManager.LoadScene ("tutorial");
		}
		else
			SceneManager.LoadScene ("main_game");
	}

    public void Quit()
	{
		Application.Quit ();
	}

    public void DisplayHighscoreStats()
	{
		statsPanel.SetActive (true);
		highscore.text = PlayerPrefs.GetString ("HighScore");
		lvlNoCleared.text = PlayerPrefs.GetString ("LvlNoCleared");
		greenCollected.text = PlayerPrefs.GetString ("GreenCollected");
		redAvoided.text = PlayerPrefs.GetString ("RedAvoided");
		tileNoPlaced.text = PlayerPrefs.GetString ("TileNoPlaced");

		//StartCoroutine (ClickToClose (statsPanel));
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
