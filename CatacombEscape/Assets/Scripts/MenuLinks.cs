using UnityEngine;
using System.Collections;

public class MenuLinks : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Facebook ()
	{
		#if UNITY_ANDROID
		if(checkPackageAppIsPresent("com.facebook.android"))
		{
		Application.OpenURL("fb://page/513996132121291"); //there is Facebook app installed so let's use it
		}
		else
		{
		Application.OpenURL("https://www.facebook.com/BuriedArtefact"); // no Facebook app - use built-in web browser
		}
		#else
		Application.OpenURL("https://www.facebook.com/BuriedArtefact"); // open Facebook in browser
		#endif
	}

	public void Twitter ()
	{
		#if UNITY_ANDROID
		if(checkPackageAppIsPresent("com.twitter.android"))
		{
		Application.OpenURL("twitter:///user?screen_name=EXcavator"); //there is Twitter app installed so let's use it
		}
		else
		{
		Application.OpenURL("https://mobile.twitter.com/BuriedArtefact"); // no Twitter app - use built-in web browser
		}
		#else
		Application.OpenURL("https://twitter.com/BuriedArtefact"); // open Twitter in browser
		#endif
	}

	public void Survey ()
	{
		Application.OpenURL("https://goo.gl/forms/yR3EA6tfLgcoxhCs1"); // open Google Forms in browser
	}
}
