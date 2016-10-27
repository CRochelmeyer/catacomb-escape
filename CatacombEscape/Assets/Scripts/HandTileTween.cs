using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HandTileTween : MonoBehaviour
{
	//public Transform destination;
	public Transform[] path;
	public float moveTime;
	public iTween.EaseType easeType;

	private Hashtable moveHT = new Hashtable ();

	void Awake ()
	{
		moveHT.Add ("path", path);
		moveHT.Add ("time", moveTime);
		moveHT.Add ("easetype", easeType);
		moveHT.Add ("oncomplete", "DisableScript");
	}

	void OnEnable ()
	{
		MoveTile ();
	}

	public void MoveTile ()
	{
		iTween.MoveTo (gameObject, moveHT);
	}

	public void DisableScript ()
	{
		this.enabled = false;
	}
}
