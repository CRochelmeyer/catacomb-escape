using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuBehavior : MonoBehaviour
{	
	public GameObject highScorePanel;
	public GameObject noHighScorePanel;
	public Text highScoreValue;

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
}
