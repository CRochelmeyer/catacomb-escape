using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Does not appear to be in use currently. 18/08/16
/// </summary>
public class PauseBehaviour : MonoBehaviour
{
	public GameObject pauseUI;

	private AudioSource source;
	public AudioClip pauseClip;
	public AudioClip unpauseClip;

	public void Start ()
	{
		source = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource> ();
		PlayerPrefs.SetString ("SettingsPanelOpen", "false");
	}

	public void LoadScene(string sceneName)
	{
		GameObject gameManager = GameObject.FindGameObjectWithTag ("GameController");
		Destroy (gameManager);
		PlayerPrefs.SetString ("Paused", "false");
		//PlayerPrefs.SetString ("GeneratedBoard", "false");
		SceneManager.LoadScene (sceneName);
	}

	public void Pause()
	{
		if (pauseUI.activeInHierarchy == false)
		{
			source.PlayOneShot (pauseClip);

			PlayerPrefs.SetString ("Paused", "true");
			PlayerPrefs.SetString ("SettingsPanelOpen", "true");
			pauseUI.SetActive (true);
		} else
		{
			source.PlayOneShot (unpauseClip);

			if (PlayerPrefs.GetString ("ExtrasPanelOpen") == "false")
			{
				PlayerPrefs.SetString ("Paused", "false");
			}

			PlayerPrefs.SetString ("SettingsPanelOpen", "false");
			pauseUI.SetActive (false);
		}
	}

	public void Resume()
	{
		source.PlayOneShot (unpauseClip);

		if (PlayerPrefs.GetString ("ExtrasPanelOpen") == "false")
		{
			PlayerPrefs.SetString ("Paused", "false");
		}

		PlayerPrefs.SetString ("SettingsPanelOpen", "false");
		pauseUI.SetActive(false);
	}

	public void RestartGame(string sceneName)
	{
		SceneManager.LoadScene (sceneName);
	}
}
