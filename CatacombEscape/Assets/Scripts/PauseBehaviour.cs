using UnityEngine;
using System.Collections;

public class PauseBehaviour : MonoBehaviour
{
	public GameObject pauseUI;

	private AudioSource source;
	public AudioClip pauseClip;
	public AudioClip unpauseClip;

	public void Start ()
	{
		source = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource> ();
	}

	public void LoadScene(string sceneName)
	{
		GameObject gameManager = GameObject.FindGameObjectWithTag ("GameController");
		Destroy (gameManager);
		PlayerPrefs.SetString ("Paused", "false");
		//PlayerPrefs.SetString ("GeneratedBoard", "false");
		Application.LoadLevel(sceneName);
	}

	public void Pause()
	{
		if (PlayerPrefs.GetString ("Paused") == "false")
		{
			source.PlayOneShot (pauseClip, 0.5f);
			PlayerPrefs.SetString ("Paused", "true");
			pauseUI.SetActive (true);
		} else
		{
			PlayerPrefs.SetString ("Paused", "false");
		}
	}

	public void Resume()
	{
		source.PlayOneShot (unpauseClip, 0.5f);
		PlayerPrefs.SetString ("Paused", "false");
		pauseUI.SetActive(false);
	}

	public void RestartGame(string sceneName)
	{
		Application.LoadLevel (sceneName);
	}
}
