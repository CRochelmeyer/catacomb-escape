using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour
{
	// Coins arrays: index0 is gold coin, index 1-10 are silver coins
	public Animator animatorGold1;
	public GameObject[] gold1;

	public Animator animatorGold2;
	public GameObject[] gold2;

	public Animator animatorGold3;
	public GameObject[] gold3;

	public Animator animatorGold4;
	public GameObject[] gold4;

	public Animator animatorGold5;
	public GameObject[] gold5;

	public Animator animatorGold6;
	public GameObject[] gold6;

	private GameLogic gameLogic;
	private int previousStam = 0;

	// Use this for initialization
	void Start ()
	{
		GameObject[] tensArray;
		for (int i = 0; i < 6; i++)
		{
			tensArray = GetArray (i);
			foreach (GameObject obj in tensArray)
				obj.SetActive (false);
		}
	}

	private GameObject[] GetArray (int tens)
	{
		switch (tens)
		{
		case 0:
			return gold1;
		case 1:
			return gold2;
		case 2:
			return gold3;
		case 3:
			return gold4;
		case 4:
			return gold5;
		case 5:
			return gold6;
		}
		return gold1;
	}

	/// <summary>
	/// Updates the display of coins in the UI
	/// </summary>
	/// <param name="coinAmount">Coin amount</param>
	public void UpdateCoins (int newStamina)
	{
		int oldTens;
		int oldOnes;
		int newTens;
		int newOnes;
		GameObject[] tensArray;
		int tempStam = previousStam;

		while (tempStam != newStamina)
		{
			oldTens = (int) Mathf.Floor (tempStam / 10);
			oldOnes = tempStam % 10;

			if (previousStam > newStamina) // Decrease the number of coins
			{
				newTens = (int) Mathf.Floor ((tempStam - 1) / 10);
				newOnes = (tempStam - 1) % 10;

				if (oldTens == newTens && newOnes != 0) // ie. old stamina was 37 and new stamina is 36
				{
					tensArray = GetArray (newTens);
					SetCoinActive (tensArray, oldOnes, false);
				}
				else if (oldTens - newTens == 1 && oldOnes == 0) // Old stamina was 40 and new stamina is 37
				{
					tensArray = GetArray (newTens);
					oldOnes += 10;
					SetCoinActive (tensArray, oldOnes, false);
				}
				else // Old stamina was 31 and new stamina is 30
				{
					tensArray = GetArray (oldTens);
					SetCoinActive (tensArray, oldOnes, false);

					tensArray = GetArray (newTens - 1);
					tensArray [0].SetActive (false);
					SwitchSilverActive (tensArray, true);
					// play gold coin to silver animation
				}
				tempStam--;
			}
			else if (previousStam < newStamina) // Increase the number of coins
			{
				newTens = (int) Mathf.Floor ((tempStam + 1) / 10);
				newOnes = (tempStam + 1) % 10;

				if (oldTens == newTens && oldOnes != 0) // ie. old stamina was 36 and new stamina is 37
				{
					tensArray = GetArray (newTens);
					tensArray [newOnes].SetActive (true);
				}
				else if (oldTens - newTens == -1 && newOnes == 0) // Old stamina was 39 and new stamina is 40
				{
					tensArray = GetArray (oldTens);
					newOnes += 10;
					tensArray [newOnes].SetActive (true);
				}
				else // Old stamina was 30 and new stamina is 31
				{
					if (oldTens > 0)
					{
						tensArray = GetArray (oldTens - 1);
						tensArray [0].SetActive (true);
						SwitchSilverActive (tensArray, false);
						// play silver coin to gold animation
					}

					tensArray = GetArray (newTens);
					SetCoinActive (tensArray, newOnes, true);
				}
				tempStam++;
			}
		}
		previousStam = newStamina;
	}

	private void SwitchSilverActive (GameObject[] array, bool status)
	{
		for (int i = 1; i < 11; i++)
		{
			array[i].SetActive (status);
		}
	}

	private void SetCoinActive (GameObject[] tensArray, int index, bool active)
	{
		tensArray [index].SetActive (active);
		// play particle effect
		// play sound
	}
}

