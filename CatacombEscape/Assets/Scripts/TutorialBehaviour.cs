using UnityEngine;
using System.Collections;

public class TutorialBehaviour : MonoBehaviour
{
	public GameObject[] tutorialPanels;

	private AudioSource source;
	public AudioClip resumeClip;

	// Use this for initialization
	void Start ()
	{
		source = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource> ();	
	}

	public void RunTutorial ()
	{
		bool gameWasPaused = false;
		if (PlayerPrefs.GetString ("Paused") == "true")
			gameWasPaused = true;
		else
			PlayerPrefs.SetString ("Paused", "true");

		DisplayClickPanel (0);

		if (!gameWasPaused)
			PlayerPrefs.SetString ("Paused", "false");
	}

	private void DisplayClickPanel (int index)
	{
		if (index < tutorialPanels.Length)
		{
			tutorialPanels [index].SetActive (true);
			StartCoroutine (ClickToClose (index));
		}
	}

	IEnumerator ClickToClose (int index)
	{
		do
		{
			yield return null;
		}while (!Input.GetMouseButtonUp (0));

		tutorialPanels [index].SetActive (false);
		source.PlayOneShot (resumeClip);
		DisplayClickPanel (index + 1);
	}
}
