using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct LocAndAmount
{
	public int amount;
	public string location;

	public LocAndAmount (int amt, string loc)
	{
		amount = amt;
		location = loc;
	}
}

public class CoinController : MonoBehaviour
{
	public GameLogic gameLogic;
	public TutorialLogic tutLogic;
	public GameObject silverPrefab;
	public GameObject goldPrefab;
	public Transform coinSource;
	public Transform newHandButtonTransform;
	private Vector3 newHandPosition;

	public float coinWait;
	public float coinTotTime;

	private AudioSource source;
	public AudioClip coinShortClip;
	//public AudioClip coinLongClip;

	private List<LocAndAmount> coinUpdateStack = new List<LocAndAmount>();
	private bool updateDone = true;

	void Awake()
	{
		source = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<AudioSource> ();
		newHandPosition = newHandButtonTransform.position;
	}

	void Update()
	{
		if (coinUpdateStack.Count > 0 && updateDone)
		{
			updateDone = false;
			if (source == null)
				Awake();
			source.PlayOneShot (coinShortClip);
			StartCoroutine (ManageCoins (coinUpdateStack[0]));
		}
	}

	/// <summary>
	/// Updates the display of coins in the UI
	/// </summary>
	/// <param name="coinAmount">Coin amount</param>
	public void UpdateCoins (int amt, string loc)
	{
		LocAndAmount newValue = new LocAndAmount (amt, loc);
		coinUpdateStack.Add (newValue);
		Update();
	}

	IEnumerator ManageCoins (LocAndAmount locAmt)
	{
		Vector3 location;
		string strLoc = locAmt.location;
		bool gainCoins = false;
		if (locAmt.amount > 0)
			gainCoins = true;

		if (gainCoins)
		{
			if (PlayerPrefs.GetString ("TutorialScene") == "true")
				location = tutLogic.GetGridPanelPosition (strLoc);
			else
				location = gameLogic.GetGridPanelPosition (strLoc);
			AddCoin (location);
		}
		else
		{
			for (int i = 0; i < Mathf.Abs (locAmt.amount); i++)
			{
				if (gainCoins) // Animate from location to silverCoinsSource
				{
					// strLoc will always be a tile when gaining coins
					if (PlayerPrefs.GetString ("TutorialScene") == "true")
						location = tutLogic.GetGridPanelPosition (strLoc);
					else
						location = gameLogic.GetGridPanelPosition (strLoc);
					AddCoin (location);
				}
				else // Animate from silverCoinsSource to location
				{
					if (strLoc == "newHandButton")
					{
						location = newHandPosition;
					}
					else // every other location will be a tile
					{
						if (PlayerPrefs.GetString ("TutorialScene") == "true")
							location = tutLogic.GetGridPanelPosition (strLoc);
						else
							location = gameLogic.GetGridPanelPosition (strLoc);
					}

					RemoveCoin (location);
				}
				yield return new WaitForSeconds (coinWait);
			}
		}

		updateDone = true;
		if (PlayerPrefs.GetString ("TutorialScene") != "true")
			gameLogic.UpdateUI();
		coinUpdateStack.RemoveAt (0);
	}

	private void AddCoin (Vector3 startLoc)
	{
		GameObject coin = (GameObject) Instantiate (goldPrefab);
		coin.transform.SetParent (coinSource, false);
		coin.transform.position = startLoc;
		StartCoroutine (LerpCoin (coin, Time.time, startLoc, coinSource.position));
	}

	private void RemoveCoin (Vector3 destLoc)
	{
		GameObject coin = (GameObject) Instantiate (silverPrefab);
		coin.transform.SetParent (coinSource, false);
		coin.transform.position = coinSource.position;
		StartCoroutine (LerpCoin (coin, Time.time, coinSource.position, destLoc));
	}

	IEnumerator LerpCoin (GameObject coin, float startTime, Vector3 initialPosition, Vector3 targetPosition)
	{
		//float journeyLength = Vector3.Distance (initialPosition, targetPosition);
		float elapsedTime = 0;

		while (coin.transform.position != targetPosition)
		{
			//float distCovered = (Time.time - startTime) * coinSpeed;
			//float fracJourney = distCovered / journeyLength;

			elapsedTime += Time.deltaTime;

			coin.transform.position = Vector3.Lerp (initialPosition, targetPosition, elapsedTime / coinTotTime);
			/*coin.transform.position = Vector3.Lerp (coin.transform.position, targetPosition, coinSpeed * 1.5f * Time.deltaTime);

			if (Vector3.Distance (coin.transform.position, targetPosition) < 0.1f)
			{
				break;
			}*/
			yield return null;
		}
		Destroy (coin);
	}
}

