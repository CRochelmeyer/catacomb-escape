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

	#if UNITY_ANDROID
	private bool checkPackageAppIsPresent(string package)
	{
		AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

		//take the list of all packages on the device
		AndroidJavaObject appList = packageManager.Call<AndroidJavaObject>("getInstalledPackages",0);
		int num = appList.Call<int>("size");

		for(int i = 0; i < num; i++)
		{
			AndroidJavaObject appInfo = appList.Call<AndroidJavaObject>("get", i);
			string packageNew = appInfo.Get<string>("packageName");
			if(packageNew.CompareTo(package) == 0)
			{
				return true;
			}
		}
		return false;
	}
	#endif
}
