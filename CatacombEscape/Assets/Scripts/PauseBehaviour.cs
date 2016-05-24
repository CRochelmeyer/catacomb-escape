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
		source.PlayOneShot (pauseClip, 0.5f);
		PlayerPrefs.SetString ("Paused", "true");
		pauseUI.SetActive(true);
	}

	public void Resume()
	{
		source.PlayOneShot (unpauseClip, 0.5f);
		PlayerPrefs.SetString ("Paused", "false");
		pauseUI.SetActive(false);
	}
}
