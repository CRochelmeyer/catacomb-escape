using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TransitionTween : MonoBehaviour
{
	//public Transform destination;
	public Transform[] path;
	public float moveTime;
	public iTween.EaseType easeType;
	public GameObject disablePanel;
	public GameLogic gameLogic;

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
		gameObject.transform.position = path[0].position;
		MoveTile ();
	}

	public void MoveTile ()
	{
		iTween.MoveTo (gameObject, moveHT);
	}

	public void DisableScript ()
	{
		disablePanel.SetActive (false);
		gameLogic.SetNextLevel = true;
	}
}
