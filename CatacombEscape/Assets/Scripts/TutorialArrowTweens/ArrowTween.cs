using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ArrowTween : MonoBehaviour
{
	public Image arrowImage;
	public Transform[] path;
	public float moveTime;
	public float fadeTime;

	private Hashtable moveHT = new Hashtable ();
	private Hashtable fadeHT = new Hashtable ();

	void Awake ()
	{
		moveHT.Add ("path", path);
		moveHT.Add ("time", moveTime);
		moveHT.Add ("easetype", "easeInOutQuad");
		moveHT.Add ("oncomplete", "FadeArrow");

		fadeHT.Add ("alpha", 0f);
		fadeHT.Add ("time", fadeTime);
		fadeHT.Add ("oncomplete", "MoveArrow");
	}

	void OnEnable ()
	{
		MoveArrow ();
	}

	public void MoveArrow ()
	{
		gameObject.transform.position = path[0].position;
		arrowImage.color = new Color (arrowImage.color.r, arrowImage.color.g, arrowImage.color.b, 255f);
		iTween.MoveTo (gameObject, moveHT);
	}

	public void FadeArrow ()
	{
		iTween.FadeTo (gameObject, fadeHT);
	}
}
