using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoinController : MonoBehaviour
{
	public GameObject[] gold;
	public GameObject[] silver;

	public Animator[] animatorsGold;
	public Animator animatorSilver;

	private AudioSource source;
	public AudioClip coinShortClip;
	public AudioClip coinLongClip;

	private GameLogic gameLogic;
	private int previousStam = 0;
	private bool isAnimating = false;

	private int oldTens;
	private int oldOnes;
	private int newTens;
	private int newOnes;
	private int tempStam;

	private List<int> coinUpdateStack = new List<int>();
	private bool updateDone = true;

	public void Awake()
	{
		source = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<AudioSource> ();
	}

	public void Update()
	{
		if (coinUpdateStack.Count > 0 && updateDone)
		{
			updateDone = false;
			tempStam = previousStam;
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
	public void UpdateCoins (int newStamina)
	{
		coinUpdateStack.Add (newStamina);
		Update();
	}

	IEnumerator ManageCoins (int newStamina)
	{
		while (tempStam != newStamina)
		{
			if (!isAnimating) // if no coin animations are occurring 
			{
				oldTens = (int) Mathf.Floor (tempStam / 10);
				oldOnes = tempStam % 10;

				if (previousStam > newStamina) // Decrease the number of coins
				{
					newTens = (int) Mathf.Floor ((tempStam - 1) / 10);
					newOnes = (tempStam - 1) % 10;

					if (oldTens == newTens && newOnes != 0) // ie. old stamina was 37 and new stamina is 36
					{
						yield return StartCoroutine (SetCoinActive (silver, oldOnes, false));
					} 
					else if (oldTens - newTens == 1 && oldOnes == 0) // Old stamina was 40 and new stamina is 39
					{
						oldOnes += 10;
						yield return StartCoroutine (SetCoinActive (silver, oldOnes, false));
					}
					else // Old stamina was 31 and new stamina is 30
					{
						yield return StartCoroutine (SetCoinActive (silver, oldOnes, false));

						if (newTens > 0) // ie. if new stamina is not 00
						{
							yield return StartCoroutine (AnimateFromGold (newTens)); // play gold coin to silver animation
						}
						else
							break;
					}
					if (!isAnimating)
						tempStam--;
				}
				else if (previousStam < newStamina) // Increase the number of coins
				{
					newTens = (int) Mathf.Floor ((tempStam + 1) / 10);
					newOnes = (tempStam + 1) % 10;

					if (oldTens == newTens && oldOnes != 0) // ie. old stamina was 36 and new stamina is 37
					{
						yield return StartCoroutine (SetCoinActive (silver, newOnes, true));
					}
					else if (oldTens - newTens == -1 && newOnes == 0) // Old stamina was 39 and new stamina is 40
					{
						newOnes += 10;
						yield return StartCoroutine (SetCoinActive (silver, newOnes, true));
					}
					else // Old stamina was 30 and new stamina is 31
					{
						if (oldTens > 0)
						{
							//AnimateToGold (oldTens);
							yield return StartCoroutine (AnimateToGold (oldTens)); // play silver coin to gold animation
						}

						yield return StartCoroutine (SetCoinActive (silver, newOnes, true));
					}
					if (!isAnimating)
						tempStam++;
				}
			}
			else
				yield return null;
		}
		previousStam = newStamina;
		updateDone = true;
		coinUpdateStack.RemoveAt (0);
	}

	private void SwitchSilverActive (GameObject[] array, bool status)
	{
		source.PlayOneShot (coinLongClip);

		for (int i = 0; i < 10; i++)
		{
			array[i].SetActive (status);
		}
	}

	IEnumerator SetCoinActive (GameObject[] tensArray, int index, bool active)
	{
		tensArray [index-1].SetActive (active);
		// play particle effect
		isAnimating = true;
		yield return StartCoroutine (SetActiveWait ());
	}

	IEnumerator SetActiveWait ()
	{
		yield return new WaitForSeconds (0.05f);
		isAnimating = false;
	}

	IEnumerator AnimateToGold (int index)
	{
		isAnimating = true;
		animatorSilver.SetBool ("silverAnim", true);

		yield return StartCoroutine (ToGoldSilverFinish (index));
		yield return StartCoroutine (ToGoldGoldFinish (index));
	}

	IEnumerator ToGoldSilverFinish (int index)
	{
		yield return new WaitForSeconds (0.33f);
		animatorSilver.SetBool ("silverAnim", false);
		gold [index - 1].SetActive (true);
		SwitchSilverActive (silver, false);
		animatorsGold [index-1].SetBool ("goldAnim", false);
		animatorsGold [index-1].SetBool ("goldAnimRev", true);
	}

	IEnumerator ToGoldGoldFinish (int index)
	{
		yield return new WaitForSeconds (0.33f);
		isAnimating = false;
	}

	IEnumerator AnimateFromGold (int index)
	{
		isAnimating = true;
		animatorsGold [index-1].SetBool ("goldAnimRev", false);
		animatorsGold [index-1].SetBool ("goldAnim", true);

		yield return StartCoroutine (FromGoldGoldFinish (index));
		yield return StartCoroutine (FromGoldSilverFinish (index));
	}

	IEnumerator FromGoldGoldFinish (int index)
	{
		yield return new WaitForSeconds (0.33f);
		SwitchSilverActive (silver, true);
		gold [index - 1].SetActive (false);
		animatorSilver.SetBool ("silverAnimRev", true);
	}

	IEnumerator FromGoldSilverFinish (int index)
	{
		yield return new WaitForSeconds (0.33f);
		animatorSilver.SetBool ("silverAnimRev", false);
		isAnimating = false;
	}
}

