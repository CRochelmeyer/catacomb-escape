using UnityEngine;
using System.Collections;

public class GemController : MonoBehaviour
{
	public GameObject gemPrefab;
	public Transform gemSource;

	public float gemMoveTime;
	public float gemRiseTime;
	public float gemRiseDist;
	private bool gottenGem = false;

	public AudioSource source;
	public AudioClip gemGetClip;

	public bool GemGetFinished
	{
		get {return gottenGem;}
	}

	public void AddGem(Vector3 startLoc)
	{
		gottenGem = false;
		GameObject gem = (GameObject) Instantiate (gemPrefab);
		gem.transform.SetParent (gemSource, false);
		gem.transform.position = startLoc;

		Vector3 gemRisePos = new Vector3 (startLoc.x, startLoc.y + gemRiseDist, startLoc.z);
		StartCoroutine (LerpGemRise (gem, Time.time, startLoc, gemRisePos));
	}

	IEnumerator LerpGemRise (GameObject gem, float startTime, Vector3 initialPosition, Vector3 targetPosition)
	{
		float elapsedTime = 0;

		while (gem.transform.position != targetPosition)
		{
			elapsedTime += Time.deltaTime;

			gem.transform.position = Vector3.Lerp (initialPosition, targetPosition, elapsedTime / gemRiseTime);
			yield return null;
		}
		StartCoroutine (LerpGemToSource (gem, Time.time, gem.transform.position, gemSource.position));
	}

	IEnumerator LerpGemToSource (GameObject gem, float startTime, Vector3 initialPosition, Vector3 targetPosition)
	{
		float elapsedTime = 0;
		source.PlayOneShot (gemGetClip);

		while (gem.transform.position != targetPosition)
		{
			elapsedTime += Time.deltaTime;

			gem.transform.position = Vector3.Lerp (initialPosition, targetPosition, elapsedTime / gemMoveTime);
			yield return null;
		}
		Destroy (gem);
		gottenGem = true;
	}
}
