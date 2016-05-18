using UnityEngine;
using System.Collections;

public class MenuBehavior : MonoBehaviour
{	
	public AudioSource source;
	public AudioClip startGameClip;

	// Use this for initialization
	void Start ()
	{
		Screen.SetResolution (569, 910, false);

		source = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadScene(string sceneName)
	{
		if (sceneName == "main_game")
		{
			source.PlayOneShot (startGameClip, 0.5f);
			System.Threading.Thread.Sleep (2000);
		}
		Application.LoadLevel(sceneName);
	}
	
	public void Quit()
	{
		Application.Quit ();
	}
	
	public void ResetPlayerPrefs()
	{
		PlayerPrefs.SetString ("FirstPlay", "true");
	}
}
