using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExtrasPanel : MonoBehaviour
{
	private AudioSource source;
	public AudioClip resumeClip;

	public Animator extrasPanel;

	public RectTransform moreArrows;

	// Use this for initialization
	void Start ()
	{
		source = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource> ();
		PlayerPrefs.SetString ("ExtrasPanelOpen", "false");
	}

	public void TogglePanel ()
	{
		bool extrasPanelHidden = extrasPanel.GetBool ("isHidden");

		if (PlayerPrefs.GetString ("SettingsPanelOpen") == "false")
		{
			if (extrasPanelHidden == true)
			{
				PlayerPrefs.SetString ("Paused", "true");
				PlayerPrefs.SetString ("ExtrasPanelOpen", "true");
				source.PlayOneShot (resumeClip);
				extrasPanel.SetBool ("isHidden", false);
				moreArrows.rotation = Quaternion.identity;
			}else
			{
				PlayerPrefs.SetString ("Paused", "false");
				PlayerPrefs.SetString ("ExtrasPanelOpen", "false");
				source.PlayOneShot (resumeClip);
				extrasPanel.SetBool ("isHidden", true);
				moreArrows.rotation = Quaternion.Euler (0, 0, 180);
			}
		}
	}
}
