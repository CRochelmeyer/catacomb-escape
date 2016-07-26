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
		PlayerPrefs.SetString ("Paused", "true");

		for (int i=0; i < tutorialPanels.Length; i++)
		{
			DisplayClickPanel (tutorialPanels [i]);
			Debug.Log ("dipslay click panel");
		}

		PlayerPrefs.SetString ("Paused", "false");
	}

	private void DisplayClickPanel (GameObject panel)
	{
		panel.SetActive (true);
		StartCoroutine (ClickToClose (panel));
	}

	IEnumerator ClickToClose (GameObject panel)
	{
		while (!Input.GetMouseButtonUp (0))
		{
			yield return null;
		}
		panel.SetActive (false);
		source.PlayOneShot (resumeClip);
	}
}
