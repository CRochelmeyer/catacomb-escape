using UnityEngine;
using System.Collections;

public class PauseBehaviour : MonoBehaviour
{
	public GameObject pauseUI;

	public void LoadScene(string sceneName)
	{
		GameObject gameManager = GameObject.FindGameObjectWithTag ("GameController");
		Destroy (gameManager);
		PlayerPrefs.SetString ("Paused", "false");
		PlayerPrefs.SetString ("GeneratedBoard", "false");
		Application.LoadLevel(sceneName);
	}

	public void Pause()
	{
		PlayerPrefs.SetString ("Paused", "true");
		pauseUI.SetActive(true);
	}

	public void Resume()
	{
		PlayerPrefs.SetString ("Paused", "false");
		pauseUI.SetActive(false);
	}
}
