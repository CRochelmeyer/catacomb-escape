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
