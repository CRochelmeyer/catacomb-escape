using UnityEngine;
using System.Collections;

public class MenuBehavior : MonoBehaviour
{	
	// Use this for initialization
	void Start ()
	{
		//Screen.SetResolution (650, 1040, false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadScene(string sceneName)
	{
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
