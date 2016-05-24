using UnityEngine;
using System.Collections;

public class BackGroundMusic : MonoBehaviour
{
	public AudioSource source;
	public AudioClip bgSound1;
	//public AudioClip bgSound2;

	// Use this for initialization
	void Awake ()
	{
		source = GetComponent<AudioSource> ();
		/*
		if (source.clip == bgSound2)
			source.clip = bgSound1;
		else
			source.clip = bgSound2;
		*/
		source.Play ();
	}

	public void ResetScript()
	{
		this.Awake ();
	}
}
